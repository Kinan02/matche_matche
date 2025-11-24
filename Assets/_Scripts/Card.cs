using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private CardType cardType;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button cardButton;

    private bool isSelected;

    void Start()
    {
        if (cardButton != null)
            cardButton.onClick.AddListener(() => { Show();});
    }

    void Update()
    {
        
    }

    private void Show()
    {
        
    }
}
