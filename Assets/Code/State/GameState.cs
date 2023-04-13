using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public int turn;    // which player's turn it is
    public TurnAction action;

    private int numPlayers;

    public GameState(int numPlayers)
    {
        this.numPlayers = numPlayers;
    }
}