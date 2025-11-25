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
        ValidateGridSize();
        SetupGridLayout();
        GenerateCards();

        totalPairs = (int)(gridSize.x * gridSize.y) / 2;

        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(ShowAllCardsAtStart());
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
            card.HideWithDelay(0f);
        }

        canSelect = true;
    }


    public void GenerateCards()
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

            if (matchCount >= totalPairs)
            {
                Debug.Log("Game Won! All matches found!");
            }
        }
        else
        {
            firstCard.HideWithDelay(0.5f);
            secondCard.HideWithDelay(0.5f);
            score--;
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


}
