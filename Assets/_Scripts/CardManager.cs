using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField] private List<CardMatch> cardMatches = new List<CardMatch>();

    [System.Serializable]
    private class CardMatch
    {
        public CardType CardType;
        public Sprite CardSprite;
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
