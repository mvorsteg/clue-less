using System;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public Text text;
    public GameObject displayScreen;

    private void Start()
    {
        displayScreen.SetActive(false);
    }

    public void NotifyGameOver(string player, GameOverType type)
    {
        string plural = "s";
        if (player.ToLower() == "you")
        {
            plural = "";
        }
        if (type == GameOverType.Lose)
        {
            text.text = String.Format("{0} lose{2}!", player, plural);
        }
        else if (type == GameOverType.Win)
        {
            text.text = String.Format("{0} win{2}!", player, plural);
        }
        else if (type == GameOverType.Error)
        {
            text.text = String.Format("Unfortunately an error has occurred");
        }
        displayScreen.SetActive(true);
    }
}