using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseEngine : MonoBehaviour
{
    public Board board;
    public Dictionary<int, PlayerState> players;
    public GameState state;
    public CardDeck deck;
    public ConsoleLogger logger;    

    protected virtual void Awake()
    {
        players = new Dictionary<int, PlayerState>();
    }

    public virtual bool StartGame()
    {
        deck.Initialize();
        return true;
    }

    public virtual bool AddPlayer(int playerID, string name, CharacterType assignedCharacter)
    {
        // intentionally blank for now
        return false;
    }

    public string GetPlayerName(int playerID)
    {
        if (players.TryGetValue(playerID, out PlayerState player))
        {
            return player.playerName;
        }
        return string.Empty;
    }

    protected void Log(string message)
    {
        if (logger != null)
        {
            logger.Log(message, SubsystemType.Engine);
        }
    }
}