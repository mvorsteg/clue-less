using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardsTabUI : MonoBehaviour
{
    public Transform cardParent;
    public CardUI cardPrefab;
    public GameObject initialScreen;

    private void Start()
    {
        
    }

    public void StartGame()
    {
        initialScreen.SetActive(false);
    }

    public void SetCards(List<ClueCard> cards)
    {
        foreach (ClueCard card in cards)
        {
            cardPrefab.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            CardUI newCard = Instantiate(cardPrefab.gameObject, cardParent).GetComponent<CardUI>();
            newCard.Initialize(card);
        }
        
    }
}