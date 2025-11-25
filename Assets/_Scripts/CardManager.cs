using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    [SerializeField] private Card cardPrefab;

    [SerializeField] private Transform gridTransform;
    [SerializeField] private GridLayoutGroup gridLayout;
    [SerializeField] private Vector2 gridSize = new Vector2(4, 4);
    [SerializeField] private Text scoreText;



    [SerializeField] private List<CardMatch> cardMatches = new List<CardMatch>();
    private List<Card> allCards = new List<Card>();

    private Queue<Card> selectedCards = new Queue<Card>();

    private int matchCount = 0;
    private int score = 0;
    private int totalPairs;

    private bool canSelect = false;


    [System.Serializable]
    private class CardMatch
    {
        public CardType CardType;
        public Sprite CardSprite;
    }

    private void Awake()
    {
        allCards.Clear();
        matchCount = 0;

        if (PlayerPrefs.HasKey("GridX") && PlayerPrefs.HasKey("GridY"))
        {
            gridSize = new Vector2(
                PlayerPrefs.GetFloat("GridX"),
                PlayerPrefs.GetFloat("GridY")
            );
        }

        ValidateGridSize();
        SetupGridLayout();

        if (PlayerPrefs.HasKey("LAYOUT"))
            GenerateEmptyCards();
        else
            GenerateCards();


        totalPairs = (int)(gridSize.x * gridSize.y) / 2;

        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        LoadGame();
        scoreText.text = $"Score : {score}";
        StartCoroutine(ShowAllCardsAtStart());
    }

    private void GenerateEmptyCards()
    {
        int totalCards = (int)(gridSize.x * gridSize.y);

        for (int i = 0; i < totalCards; i++)
        {
            Card newCard = Instantiate(cardPrefab, gridTransform);
            newCard.SetIndex(i);
            allCards.Add(newCard);
        }
    }

    private IEnumerator ShowAllCardsAtStart()
    {
        canSelect = false;

        foreach (Card card in allCards)
        {
            card.Show();
        }

        yield return new WaitForSeconds(2f);

        foreach (Card card in allCards)
        {
            if (!card.isMatched)
                card.HideWithDelay(0f);
        }

        canSelect = true;
    }


    private void GenerateCards()
    {
        int totalCards = (int)(gridSize.x * gridSize.y);
        int pairsNeeded = totalCards / 2;

        if (pairsNeeded > cardMatches.Count)
        {
            Debug.LogError($"Not enough card types! Need {pairsNeeded} but only have {cardMatches.Count}");
            return;
        }

        List<CardMatch> cardPool = new List<CardMatch>();
        for (int i = 0; i < pairsNeeded; i++)
        {
            cardPool.Add(cardMatches[i]);
            cardPool.Add(cardMatches[i]);
        }

        ShuffleCards(cardPool);

        foreach (CardMatch cardMatch in cardPool)
        {
            Card newCard = CreateCard(cardMatch.CardType, cardMatch.CardSprite);
            allCards.Add(newCard);
        }
    }

    private void ShuffleCards(List<CardMatch> pool)
    {
        for (int i = 0; i < pool.Count; i++)
        {
            CardMatch temp = pool[i];
            int randomIndex = Random.Range(i, pool.Count);
            pool[i] = pool[randomIndex];
            pool[randomIndex] = temp;
        }
    }

    private Card CreateCard(CardType cardType, Sprite cardSprite)
    {
        Card newCard = Instantiate(cardPrefab, gridTransform);
        newCard.SetCardType(cardType);
        newCard.SetCardSprite(cardSprite);
        newCard.SetIndex(allCards.Count);
        return newCard;
    }

    public void SetSelected(Card card)
    {
        if (!canSelect) return;

        if (card.isSelected) return;

        card.Show();
        selectedCards.Enqueue(card);

        if (selectedCards.Count >= 2)
            CheckMatching();
    }

    private void CheckMatching()
    {
        Card firstCard = selectedCards.Dequeue();
        Card secondCard = selectedCards.Dequeue();

        if (firstCard.cardType == secondCard.cardType)
        {
            firstCard.MarkMatched();
            secondCard.MarkMatched();
            matchCount++;
            score++;
            scoreText.text = $"Score : {score}";

            AudioManager.Instance.PlayCorrectChoice();

            SaveGame();

            if (matchCount >= totalPairs)
            {
                ResetGameData();
                AudioManager.Instance.PlayWin();
                Debug.Log("Game Won! All matches found!");
            }
        }
        else
        {
            firstCard.HideWithDelay(0.5f);
            secondCard.HideWithDelay(0.5f);
            score--;
            scoreText.text = $"Score : {score}";

            SaveGame();

            AudioManager.Instance.PlayWrongChoice();
        }
    }

    private void ValidateGridSize()
    {
        int totalCells = (int)(gridSize.x * gridSize.y);

        if (totalCells % 2 != 0)
        {
            Debug.LogWarning("Grid size must create even number of cards. Adjusting...");
            gridSize.x = Mathf.Max(2, gridSize.x);
            gridSize.y = Mathf.Max(2, gridSize.y);
            totalCells = (int)(gridSize.x * gridSize.y);
            if (totalCells % 2 != 0)
            {
                gridSize.x += 1;
            }
        }

        int requiredPairs = totalCells / 2;
        if (cardMatches.Count < requiredPairs)
        {
            Debug.LogError($"Not enough cards! Need {requiredPairs} cards but only have {cardMatches.Count}");
        }
    }

    private void SetupGridLayout()
    {
        if (gridLayout != null)
        {
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = (int)gridSize.x;
        }
        else
        {
            Debug.LogError("No GridLayoutGroup found!");
            return;
        }
    }

    private const string SAVE_KEY = "CARD_SAVE";

    private void SaveGame()
    {
        List<string> matchedIndexes = new List<string>();
        List<string> layoutData = new List<string>();

        foreach (Card card in allCards)
        {
            layoutData.Add(card.cardIndex + ":" + card.cardType.ToString());

            if (card.isMatched)
            {
                matchedIndexes.Add(card.cardIndex.ToString());
            }
        }

        PlayerPrefs.SetString("MATCHED", string.Join(",", matchedIndexes));
        PlayerPrefs.SetString("LAYOUT", string.Join("|", layoutData));

        PlayerPrefs.SetFloat("GridX", gridSize.x);
        PlayerPrefs.SetFloat("GridY", gridSize.y);
        PlayerPrefs.SetInt("SCORE", score);

        PlayerPrefs.Save();
    }


    private void LoadGame()
    {
        if (!PlayerPrefs.HasKey("LAYOUT"))
            return;

        string layout = PlayerPrefs.GetString("LAYOUT");
        string[] cardData = layout.Split('|');

        foreach (string data in cardData)
        {
            string[] parts = data.Split(':');
            int index = int.Parse(parts[0]);
            CardType type = (CardType)System.Enum.Parse(typeof(CardType), parts[1]);

            if (index < allCards.Count)
            {
                Card card = allCards[index];
                card.SetCardType(type);

                Sprite sprite = GetSpriteForType(type);
                card.SetCardSprite(sprite);
            }
        }

        if (PlayerPrefs.HasKey("MATCHED"))
        {
            string[] matchedIndexes = PlayerPrefs.GetString("MATCHED").Split(',');
            int loadedCards = 0;

            foreach (string indexStr in matchedIndexes)
            {
                if (int.TryParse(indexStr, out int index))
                {
                    if (index >= 0 && index < allCards.Count)
                    {
                        Card card = allCards[index];
                        card.MarkMatched();
                        card.Show();
                        loadedCards++;
                    }
                }
            }

            matchCount = loadedCards / 2;
            score = PlayerPrefs.GetInt("SCORE");
            
        }

    }
    private Sprite GetSpriteForType(CardType type)
    {
        foreach (var match in cardMatches)
        {
            if (match.CardType == type)
                return match.CardSprite;
        }
        return null;
    }

    private void ResetGameData()
    {
        PlayerPrefs.DeleteKey("GridX");
        PlayerPrefs.DeleteKey("GridY");
        PlayerPrefs.DeleteKey("LAYOUT");
        PlayerPrefs.DeleteKey("MATCHED");
        PlayerPrefs.DeleteKey("CARD_SAVE");
        PlayerPrefs.DeleteKey("SCORE");

        PlayerPrefs.Save();

        Debug.Log("All game data reset!");
    }




}
