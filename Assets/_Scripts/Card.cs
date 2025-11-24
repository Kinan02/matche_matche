using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class Card : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Button cardButton;

    [SerializeField] private Sprite hiddenSprite;
    public Sprite revealedSprite { private get; set; }
    public CardType cardType { get; set; }

    public bool isSelected { get; private set; }

    void Start()
    {
        if (cardButton != null)
            cardButton.onClick.AddListener(() => {
                OnCardClick();
            });
    }

    private void OnCardClick()
    {
        CardManager.Instance.SetSelected(this);
    }

    public void Show()
    {
        iconImage.sprite = revealedSprite;
        isSelected = true;
    }

    public void HideWithDelay(float delay)
    {
        StartCoroutine(HideRoutine(delay));
    }

    private IEnumerator HideRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        Hide();
    }

    private void Hide()
    {
        iconImage.sprite = hiddenSprite;
        isSelected = false;
    }

    public void SetCardType(CardType ct) => cardType = ct;
    public void SetCardSprite(Sprite cs) => revealedSprite = cs;
    public void SetSelected() => isSelected = true;
    public void MarkMatched()
    {
        isSelected = false;
        cardButton.interactable = false;
        iconImage.color = new Color(0.7f, 0.7f, 0.7f, 0.8f);
    }
    

}
