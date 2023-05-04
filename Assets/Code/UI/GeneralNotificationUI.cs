using System;
using UnityEngine;
using UnityEngine.UI;

public class GeneralNotificationUI : MonoBehaviour
{
    public Text titleText, bodyText;
    public GameObject displayScreen;

    public void ShowModal(string title, string body)
    {
        titleText.text = title;
        bodyText.text = body;

        displayScreen.SetActive(true);
    }

}