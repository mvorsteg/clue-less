using System.Collections.Generic;
using UnityEngine;

public class MasterUI : MonoBehaviour
{
    public StatusUI statusUI;
    public CardsTabUI cardsUI;
    public GuessUI guessUI;

    public void SetTurn(string player, TurnAction action)
    {
        statusUI.SetTurnActionText(player, action);
    }   

    public void SetCards(List<ClueCard> cards)
    {
        cardsUI.SetCards(cards);
    }

    public void PromptGuess(bool isFinal, RoomType room)
    {
        guessUI.PromptGuess(isFinal, room);
    }
}