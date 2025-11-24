using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    [SerializeField] private Card cardPrefab;

    [SerializeField] private Transform gridTransform;
    [SerializeField] private GridLayoutGroup gridLayout;
    [SerializeField] private Vector2 gridSize = new Vector2(4, 4);



    [SerializeField] private List<CardMatch> cardMatches = new List<CardMatch>();
    private List<Card> allCards = new List<Card>();


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
    }

    void Start()
    {
        
    }

    void Update()
    {
        
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

        for (int i = 0; i < pairsNeeded; i++)
        {
            Card card1 = CreateCard(cardMatches[i].CardType, cardMatches[i].CardSprite);
            allCards.Add(card1);

            Card card2 = CreateCard(cardMatches[i].CardType, cardMatches[i].CardSprite);
            allCards.Add(card2);
        }

        ShuffleCards();
    }

    private void ShuffleCards()
    {
        for (int i = 0; i < allCards.Count; i++)
        {
            Card temp = allCards[i];
            int randomIndex = UnityEngine.Random.Range(i, allCards.Count);
            allCards[i] = allCards[randomIndex];
            allCards[randomIndex] = temp;
        }
    }

    private Card CreateCard(CardType cardType, Sprite cardSprite)
    {
        Card newCard = Instantiate(cardPrefab, gridTransform);
        newCard.SetCardType(cardType);
        newCard.SetCardSprite(cardSprite);
        return newCard;
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
