using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GuestEngine : BaseEngine
{
    public PlayerState player;
    public StatusUI statusUI;

    public int ID { get => player.playerID; }

    public override bool StartGame()
    {
        base.StartGame();
        return true;
    }

    public override void SetTurn(int turn, TurnAction action)
    {
        base.SetTurn(turn, action);
        if (player.playerID == turn)
        {
            statusUI.SetTurnActionText(player.playerName, action);
            Log(String.Format("It is your turn"));
        }
        else if (players.TryGetValue(turn, out PlayerState player))
        {
            statusUI.SetTurnActionText(player.playerName, action);
        }
    }

    public void AssignFromServer(int playerID, string name, CharacterType assignedCharacter)
    {
        player = new PlayerState(playerID, name, assignedCharacter);
        Log(String.Format("Joined server as {0} (Assigned ID is {1})", assignedCharacter, playerID));
    }

    public bool AssignClueCards(List<CharacterType> characterClues, List<WeaponType> weaponClues, List<RoomType> roomClues)
    {
        player.cards = deck.GetCardsFromClues(characterClues, weaponClues, roomClues);
        Log(String.Format("{0}'s cards are {1}", player.playerName, String.Join(", ", player.cards.Select(x => x.cardName))));
        return true;
    }

    public override bool AddPlayer(int playerID, string name, CharacterType assignedCharacter)
    {
        if (players.TryAdd(playerID, new PlayerState(playerID, name, assignedCharacter)))
        {
            Log(String.Format("{0} Joined server as {1} (Assigned ID is {2})", name, assignedCharacter, playerID));
            state = new GameState(players.Keys.Count);
            return true;
        }
        
        // error adding player
        Log(string.Format("Error adding player {0}", playerID));
        return false;
    }

    public bool UpdateCharacter(int playerID, CharacterType newCharacter)
    {
        if (playerID == ID)
        {
            player.character = newCharacter;
            Log(String.Format("Changed character to {0}", newCharacter));
            return true;
        }
        else if (players.TryGetValue(playerID, out PlayerState otherPlayer))
        {
            otherPlayer.character = newCharacter;
            Log(String.Format("{0} changed character to {1}", otherPlayer.playerName, newCharacter));
            return true;
        }
        Log(String.Format("Error changing player {0} character to {1}", playerID, newCharacter));
        return false;
    }

    public bool MovePlayer(int playerID, RoomType newRoom)
    {
        bool status = false;
        if (playerID == ID)
        {
            Log(String.Format("Moved to {0}", newRoom));
            status = true;
        }
        else if (players.TryGetValue(playerID, out PlayerState otherPlayer))
        {
            Log(String.Format("{0} moved to {1}", otherPlayer.playerName, newRoom));
            status = true;
        }
        else
        {
            Log(String.Format("Error moving player {0} to {1}", playerID, newRoom));
        }
        return status;
    }

    public override bool Guess(int playerID, bool isFinal, CharacterType character, WeaponType weapon, RoomType room)
    {
        bool status = false;
        if (playerID == ID)
        {
            Log(String.Format("Guessed {0} used the {1} in the {2}", character, weapon, room));
            status = true;
        }
        else if (players.TryGetValue(playerID, out PlayerState otherPlayer))
        {
            Log(String.Format("{0} guessed {1} used the {2} in the {3}", GetPlayerName(playerID), character, weapon, room));
            status = true;
        }
        else
        {
            Log("Error with guess");
        }
        return status;
    }
}