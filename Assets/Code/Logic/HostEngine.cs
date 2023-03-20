using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HostEngine : MonoBehaviour
{
    public Dictionary<int, PlayerState> players;
    public Board board;
    
    private void Awake()
    {
        players = new Dictionary<int, PlayerState>();
    }

    public bool AddPlayer(int playerID, string name, out CharacterType assignedCharacter)
    {
        // find available character to assign
        Dictionary<CharacterType, bool> availableChars = new Dictionary<CharacterType, bool>();
        foreach (CharacterType character in Enum.GetValues(typeof(CharacterType)))
        {
            availableChars.Add(character, true);
        }
        foreach (PlayerState player in players.Values)
        {
            availableChars[player.character] = false;
        }
        foreach (CharacterType character in availableChars.Keys)
        {
            if (availableChars[character])
            {
                assignedCharacter = character;
                PlayerState newPlayer = new PlayerState(playerID, name, assignedCharacter);
                if (players.TryAdd(playerID, newPlayer))
                {
                    return true;
                }
                break;
            }
        }
        
        // error adding player
        Debug.Log(string.Format("Error adding player {0}", playerID));
        assignedCharacter = CharacterType.Mustard;  // value does not matter at all if we fail. I just like col. Mustard the best
        return false;
    }

    public bool UpdateCharacter(int playerID, CharacterType newCharacter)
    {
        foreach (PlayerState player in players.Values)
        {
            if (player != null && player.character == newCharacter)
            {
                return false;
            }
        }
        players[playerID].character = newCharacter;
        return true;
    }

    public bool MovePlayer(int playerID, RoomType destRoom)
    {
        if (players.TryGetValue(playerID, out PlayerState playerState))
        {
            if (board.IsValidMove(playerState.currentRoom, destRoom))
            {
                // do move
                playerState.currentRoom = destRoom;
                return true;
            }
        }
        return false;
    }

}