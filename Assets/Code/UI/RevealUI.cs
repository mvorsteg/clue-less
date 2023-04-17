using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RevealUI : MonoBehaviour
{
    public Text sendText, receiveText;
    public Transform sendCardParent;
    public CardUI cardPrefab;
    public CardUI receivedCardDisplay;
    public GameObject sendRevealScreen, receiveRevealScreen;
    private ClientNetworkInterface netInterface;
    private GuestEngine engine;

    private void Start()
    {
        netInterface = FindObjectOfType<ClientNetworkInterface>();
        engine = FindObjectOfType<GuestEngine>();
        sendRevealScreen.SetActive(false);
        receiveRevealScreen.SetActive(false);
    }

    public void PromptReveal(List<ClueCard> cards, string playerName)
    {
        foreach (ClueCard card in cards)
        {
            CardUI newCard = Instantiate(cardPrefab.gameObject, sendCardParent).GetComponent<CardUI>();
            newCard.Initialize(card);
            Button button = newCard.GetComponentInChildren<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => {SelectReveal(card);});
            }
        }
        sendText.text = String.Format("You must reveal a card to {0}", playerName);
        sendRevealScreen.SetActive(true);
    }
    
    public void AcceptReveal(ClueCard card, string playerName)
    {
        receivedCardDisplay.Initialize(card);

        receiveText.text = String.Format("{0} has revealed {1}", playerName, card.cardName);
        receiveRevealScreen.SetActive(true);
    }

    public void SelectReveal(ClueCard card)
    {
        RevealPacket pkt;
        if (card.TryGetCharacterType(out CharacterType character))
        {
            pkt = new RevealPacket(true, engine.ID, engine.state.turn, ClueType.Character, character, 0, 0);
        }
        else if (card.TryGetWeaponType(out WeaponType weapon))
        {
            pkt = new RevealPacket(true, engine.ID, engine.state.turn, ClueType.Weapon, 0, weapon, 0);
        }
        else if (card.TryGetRoomType(out RoomType room))
        {
            pkt = new RevealPacket(true, engine.ID, engine.state.turn, ClueType.Room, 0, 0, room);
        }
        else
        {
            //error
            return;
        }
        netInterface.SendMessage(pkt);
        sendRevealScreen.SetActive(false);
        for (int i = 0; i < sendCardParent.childCount; i++)
        {
            Destroy(sendCardParent.GetChild(i).gameObject);
        }
    }
}