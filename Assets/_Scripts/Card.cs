using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Button cardButton;

    [SerializeField] private Sprite hiddenSprite;
    public Sprite revealedSprite { private get; set; }
    public CardType cardType { private get; set; }

    private bool isSelected;

    void Awake()
    {
        if (cardButton != null)
            cardButton.onClick.AddListener(() => {
                Show(); 
            });
    }

    private void Start()
    {
        
    }

    void Update()
    {

    }

    private void Show()
    {
        iconImage.sprite = revealedSprite;
        isSelected = true;
    }

    public void Hide()
    {
        iconImage.sprite = hiddenSprite;
        isSelected = false;
    }

    public void SetCardType(CardType ct) => cardType = ct;
    public void SetCardSprite(Sprite cs) => revealedSprite = cs;
    

}
