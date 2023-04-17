using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardsTabUI : MonoBehaviour
{
    public Transform cardParent;
    public CardUI cardPrefab;

    private void Start()
    {
        
    }

    public void SetCards(List<ClueCard> cards)
    {
        foreach (ClueCard card in cards)
        {
            CardUI newCard = Instantiate(cardPrefab.gameObject, cardParent).GetComponent<CardUI>();
            newCard.Initialize(card);
        }
        
    }
}