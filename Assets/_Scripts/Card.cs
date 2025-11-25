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
    public int cardIndex {  get; set; }
    public bool isSelected { get; private set; }
    public bool isMatched { get; private set; }


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

        AudioManager.Instance.PlayCardFlip();
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

        AudioManager.Instance.PlayCardFlip();
    }

    public void SetCardType(CardType ct) => cardType = ct;
    public void SetCardSprite(Sprite cs) => revealedSprite = cs;
    public void SetSelected() => isSelected = true;
    public void SetIndex(int index) => cardIndex = index;
    public void MarkMatched()
    {
        isMatched = true;
        isSelected = false;
        cardButton.interactable = false;
        iconImage.color = new Color(0.7f, 0.7f, 0.7f, 0.8f);
    }
    

}
