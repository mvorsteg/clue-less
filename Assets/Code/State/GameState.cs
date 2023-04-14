using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public int turn;    // which player's turn it is
    public TurnAction action;

    public int numPlayers;

    public GameState(int numPlayers)
    {
        this.numPlayers = numPlayers;
        this.turn = -1;
    }
}