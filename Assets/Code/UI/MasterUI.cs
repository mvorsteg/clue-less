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
    public GameOverUI gameOverUI;
    public ActionUI actionUI;
    public GeneralNotificationUI genaralNotificationUI;
    
    public GuestEngine engine;

    private CharacterType playerChar;

    public void StartGame(List<string> playerNames)
    {
        clueSheetUI.StartGame(playerNames);
        cardsUI.StartGame();
        playersUI.StartGame();
    }

    public void AddPlayer(int playerID, CharacterType startingCharacter, string name)
    {
        playersUI.AddPlayer(playerID, startingCharacter, name);
        charUpdateUI.UpdateChoices(startingCharacter, false);
    }

    public void RemovePlayer(int playerID, CharacterType freeCharacter)
    {
        playersUI.RemovePlayer(playerID);
        charUpdateUI.UpdateChoices(freeCharacter, true);
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
        foreach (ClueCard card in cards)
        {
            clueSheetUI.EnqueueMark(card, engine.player.playerName);
        }
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

    public void NotifyGameOver(string player, GameOverType type)
    {
        gameOverUI.NotifyGameOver(player, type);
    }

    public void ShowGeneralNotification(string title, string body, bool isOk)
    {
        genaralNotificationUI.ShowModal(title, body, isOk);
    }
}