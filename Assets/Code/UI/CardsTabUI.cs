using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardsTabUI : MonoBehaviour
{
    public Transform cardParent;
    public GameObject cardPrefab;

    public void SetCards(List<ClueCard> cards)
    {
        foreach (ClueCard card in cards)
        {
            GameObject newCard = Instantiate(cardPrefab, cardParent);
            Text clueText = newCard.GetComponentInChildren<Text>();
            if (clueText != null)
            {
                clueText.text = card.cardName;
            }
        }
    }
}