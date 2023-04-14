using System;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour
{
    public Text turnText;

    public void SetTurnActionText(string player, TurnAction action)
    {
        string actionText = "";
        switch(action)
        {
            case TurnAction.MoveRoom :
            {
                actionText = "moving rooms";
                break;
            }
            case TurnAction.MakeGuess :
            {
                actionText = "making a guess";
                break;
            }
            case TurnAction.RevealCards :
            {
                actionText = "waiting for cards to be revealed";
                break;
            }
            case TurnAction.Idle :
            {
                actionText = "idle";
                break;
            }
            default:
            {
                break;
            }
        }
        turnText.text = String.Format("{0} is {1}...", player, actionText);
    }
}