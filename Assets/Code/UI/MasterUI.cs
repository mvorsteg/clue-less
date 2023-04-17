using System.Collections.Generic;
using UnityEngine;

public class MasterUI : MonoBehaviour
{
    public StatusUI statusUI;
    public CardsTabUI cardsUI;
    public GuessUI guessUI;
    public RevealUI revealUI;
    public TurnUI turnUI;
    public WinLoseUI winLoseUI;
    public ActionUI actionUI;
    
    public GuestEngine engine;

    public void SetTurn(string player, TurnAction action, bool isActiveTurn)
    {
        statusUI.SetTurn(player, action);
        actionUI.SetTurn(action, isActiveTurn);
    }   

    public void SetCards(List<ClueCard> cards)
    {
        cardsUI.SetCards(cards);
    }

    public void PromptGuess(bool isFinal)
    {
        guessUI.PromptGuess(isFinal, engine.player.currentRoom);
    }

    public void PromptReveal()
    {
        revealUI.PromptReveal(engine.GetCardsToReveal(), engine.GetPlayerName(engine.state.turn));
    }

    public void NotifyReveal(ClueCard card, string name)
    {
        revealUI.AcceptReveal(card, name);
    }

    public void PromptEnd()
    {
        turnUI.PromptEndTurn();
    }

    public void NotifyWinLose(string player, bool win)
    {
        winLoseUI.NotifyWinLose(player, win);
    }
}