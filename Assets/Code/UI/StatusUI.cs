using System;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour
{
    public Text turnText, characterText, roomText;

    public void SetTurn(string player, TurnAction action)
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
        string verb = player.ToLower() == "you" ? "are" : "is";
        turnText.text = String.Format("{0} {1} {2}...", player, verb, actionText);
    }

    public void SetCharacter(CharacterType character)
    {
        characterText.text = String.Format("You are {0}", character.ToString());
    }

    public void SetRoom(RoomType room)
    {
        string roomStr = room < RoomType.HW_StudyHall ? room.ToString() : "a Hallway";
        roomText.text = String.Format("You are in {0}", roomStr);
    }
}