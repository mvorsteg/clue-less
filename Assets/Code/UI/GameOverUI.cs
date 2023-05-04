using System;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public Text titleText, bodyText;
    public GameObject displayScreen;
    public GameObject okButton, quitButton;

    private void Start()
    {
        displayScreen.SetActive(false);
    }

    public void NotifyGameOver(string player, GameOverType type)
    {
        bool isYou = player.ToLower() == "you";
        if (type == GameOverType.Lose)
        {
            if (isYou)
            {
                titleText.text = String.Format("You Lose");
                bodyText.text = String.Format("Your guess was incorrect.\nYou are out for the rest of the game.\nBetter luck next time!");
            }
            else
            {
                titleText.text = String.Format("{0} Loses", player);
                bodyText.text = String.Format("{0}'s guess was incorrect.\nThey are out for the rest of the game.", player);
            }
            okButton.SetActive(true);
            quitButton.SetActive(false);
        }
        else if (type == GameOverType.Win)
        {
            if (isYou)
            {
                titleText.text = String.Format("You Win");
                bodyText.text = String.Format("Your guess was correct.\nYou win!.\nThanks for playing!");
            }
            else
            {
                titleText.text = String.Format("{0} Wins", player);
                bodyText.text = String.Format("{0}'s guess was correct.\nThey win!\nThanks for playing!", player);
            }
            okButton.SetActive(false);
            quitButton.SetActive(true);
        }
        else if (type == GameOverType.Error)
        {
            titleText.text = String.Format("Error");
            bodyText.text = String.Format("Unfortunately a communication error has occurred.\nThe game will not be able to continue.\nPlease ensure everyone playing has an internet connection.");
            okButton.SetActive(false);
            quitButton.SetActive(true);
        }
        displayScreen.SetActive(true);
    }
}