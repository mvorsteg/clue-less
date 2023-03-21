using System;
using System.Collections.Generic;
using UnityEngine;

public class GuestEngine : MonoBehaviour
{
    public PlayerState player;
    public Dictionary<int, PlayerState> otherPlayers;
    public Board board;

    private void Awake()
    {
        otherPlayers = new Dictionary<int, PlayerState>();
    }

    public void AssignFromServer(int playerID, CharacterType assignedCharacter)
    {
        player = new PlayerState(playerID, "test name", assignedCharacter);
    }

    public bool AddPlayer(int playerID, string name, CharacterType assignedCharacter)
    {
        if (otherPlayers.TryAdd(playerID, new PlayerState(playerID, name, assignedCharacter)))
        {
            return true;
        }
        
        // error adding player
        Debug.Log(string.Format("Error adding player {0}", playerID));
        return false;
    }

    public string GetPlayerName(int playerID)
    {
        if (otherPlayers.TryGetValue(playerID, out PlayerState player))
        {
            return player.playerName;
        }
        return string.Empty;
    }
}