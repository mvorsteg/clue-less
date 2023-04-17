using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionUI : MonoBehaviour
{
    public Button moveButton, guessButton, revealButton, accuseButton, endButton;

    public GuestEngine engine;


    private void Start()
    {
        engine = FindObjectOfType<GuestEngine>();

        moveButton.interactable = false;
        guessButton.interactable = false;
        revealButton.interactable = false;
        accuseButton.interactable = false;
        endButton.interactable = false;
    }

    public void SetTurn(TurnAction action, bool isActiveTurn)
    {
        moveButton.interactable = false;
        guessButton.interactable = false;
        revealButton.interactable = false;
        accuseButton.interactable = false;
        endButton.interactable = false;
        if (isActiveTurn)
        {
            accuseButton.interactable = true;
            switch (action)
            {
                case TurnAction.MoveRoom :
                    moveButton.interactable = true;
                    break;
                case TurnAction.MakeGuess :
                    guessButton.interactable = true;
                    break;
                case TurnAction.Idle :
                    endButton.interactable = true;
                    break;
            }
        }
        else
        {
            switch (action)
            {
                case TurnAction.RevealCards :
                    revealButton.interactable = engine.GetCardsToReveal().Count > 0;
                    break;
            }
        }
    }
}