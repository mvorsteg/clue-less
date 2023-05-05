using System;
using UnityEngine;
using UnityEngine.UI;

public class GeneralNotificationUI : MonoBehaviour
{
    public Text titleText, bodyText;
    public GameObject displayScreen;
    public GameObject okButton, quitButton;

    public void ShowModal(string title, string body, bool isOk)
    {
        titleText.text = title;
        bodyText.text = body;

        if (isOk)
        {
            okButton.SetActive(true);
            quitButton.SetActive(false);
        }
        else
        {
            okButton.SetActive(false);
            quitButton.SetActive(true);
        }

        displayScreen.SetActive(true);
    }

}