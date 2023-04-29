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
        state = new GameState(0);
    }

    public virtual bool StartGame()
    {
        state = new GameState(players.Keys.Count);
        deck.Initialize();
        return true;
    }

    public virtual void SetPlayerReady(int playerID, bool isReady)
    {
        
    }

    public virtual void SetTurn(int turn, TurnAction action)
    {
        if (state.turn != turn)
        {
            if (players.TryGetValue(turn, out PlayerState player))
            {
                Log(String.Format("It is {0}'s turn", player.playerName));
            }
        }
        state.turn = turn;
        state.action = action;
    }

    public virtual bool AddPlayer(int playerID, string name, CharacterType assignedCharacter)
    {
        // intentionally blank for now
        return false;
    }

    public virtual bool Guess(int playerID, bool isFinal, CharacterType character, WeaponType weapon, RoomType room)
    {
        return false;
    }

    public virtual bool Reveal(int sendID, int recvID, ClueType clueType, CharacterType character, WeaponType weapon, RoomType room)
    {
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