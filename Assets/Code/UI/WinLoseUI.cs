using System;
using UnityEngine;
using UnityEngine.UI;

public class WinLoseUI : MonoBehaviour
{
    public Text text;
    public GameObject displayScreen;

    private void Start()
    {
        displayScreen.SetActive(false);
    }

    public void NotifyWinLose(string player, bool win)
    {
        string plural = "s";
        string action;
        if (player.ToLower() == "you")
        {
            plural = "";
        }
        if (win)
        {
            action = "win";
        }
        else
        {
            action = "lose";
        }
        text.text = String.Format("{0} {1}{2}!", player, action, plural);
        displayScreen.SetActive(true);
    }
}