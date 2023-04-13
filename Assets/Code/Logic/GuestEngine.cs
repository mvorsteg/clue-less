using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GuestEngine : BaseEngine
{
    public PlayerState player;

    public int ID { get => player.playerID; }

    public void AssignFromServer(int playerID, CharacterType assignedCharacter)
    {
        player = new PlayerState(playerID, "test name", assignedCharacter);
        Log(String.Format("Joined server as {0} (Assigned ID is {1})", assignedCharacter, playerID));
    }

    public override bool AddPlayer(int playerID, string name, CharacterType assignedCharacter)
    {
        if (players.TryAdd(playerID, new PlayerState(playerID, name, assignedCharacter)))
        {
            Log(String.Format("{0} Joined server as {1} (Assigned ID is {2})", name, assignedCharacter, playerID));
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
        if (playerID == ID)
        {
            Log(String.Format("Moved to {0}", newRoom));
            return true;
        }
        else if (players.TryGetValue(playerID, out PlayerState otherPlayer))
        {
            Log(String.Format("{0} moved to {1}", otherPlayer.playerName, newRoom));
            return true;
        }
        Log(String.Format("Error moving player {0} to {1}", playerID, newRoom));
        return false;
    }

    public bool AssignClueCards(List<CharacterType> characterClues, List<WeaponType> weaponClues, List<RoomType> roomClues)
    {
        player.cards = deck.GetCardsFromClues(characterClues, weaponClues, roomClues);
        Log(String.Format("{0}'s cards are {1}", player.playerName, String.Join(", ", player.cards.Select(x => x.cardName))));
        return true;
    }

    public bool Guess(int playerID, bool isFinal, CharacterType character, WeaponType weapon, RoomType room)
    {
        if (playerID == ID)
        {
            Log(String.Format("Guessed {0} used the {1} in the {2}", character, weapon, room));
            return true;
        }
        else if (players.TryGetValue(playerID, out PlayerState otherPlayer))
        {
            Log(String.Format("{0} guessed {1} used the {2} in the {3}", GetPlayerName(playerID), character, weapon, room));
            return true;
        }
        Log("Error with guess");
        return false;
    }
}