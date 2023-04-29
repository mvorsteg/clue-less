using System.Collections.Generic;
using UnityEngine;

public class MasterUI : MonoBehaviour
{
    public StatusUI statusUI;
    public PlayersTabUI playersUI;
    public CharacterUpdateUI charUpdateUI;
    public CardsTabUI cardsUI;
    public BoardUI boardUI;
    public ClueSheet clueSheetUI;
    public GuessUI guessUI;
    public RevealUI revealUI;
    public TurnUI turnUI;
    public WinLoseUI winLoseUI;
    public ActionUI actionUI;
    
    public GuestEngine engine;

    private CharacterType playerChar;

    public void StartGame(List<string> otherPlayers)
    {
        clueSheetUI.StartGame(otherPlayers);
    }

    public void AddPlayer(int playerID, CharacterType startingCharacter, string name)
    {
        playersUI.AddPlayer(playerID, startingCharacter, name);
        charUpdateUI.UpdateChoices(startingCharacter, false);
    }

    public void UpdatePlayerCharacter(int playerID, CharacterType newCharacter, CharacterType oldCharacter)
    {
        playersUI.SetCharacter(playerID, newCharacter);
        charUpdateUI.UpdateChoices(oldCharacter, true);
        charUpdateUI.UpdateChoices(newCharacter, false);
    }

    public void UpdatePlayerStatus(int playerID, PlayerStatus newStatus)
    {
        playersUI.SetStatus(playerID, newStatus);
    }

    public void SetCharacter(CharacterType newChar)
    {
        playerChar = newChar;
        statusUI.SetCharacter(playerChar);
    }

    public void SetTurn(string player, TurnAction action, bool isActiveTurn)
    {
        statusUI.SetTurn(player, action);
        actionUI.SetTurn(action, isActiveTurn);
    }   

    public void SetCards(List<ClueCard> cards)
    {
        cardsUI.SetCards(cards);
    }

    public void EnableMoveSelection()
    {
        boardUI.EnableMoveSelection(engine.player.currentRoom);
    }

    public void MoveCharacterToRoom(CharacterType character, RoomType oldRoom, RoomType newRoom)
    {
        boardUI.MoveIconToNewRoom(character, oldRoom, newRoom);
        if (character == playerChar)
        {
            statusUI.SetRoom(newRoom);
        }
    }   

    public void DisableRoomSelection()
    {
        boardUI.DisableAllRooms();
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