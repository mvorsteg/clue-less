using System;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    public Text text;
    public Image image;

    public void Initialize(ClueCard card)
    {
        text.text = card.cardName;
        if (card.sprite != null)
        {
            image.sprite = card.sprite;
            image.preserveAspect = true;
        }
    }
}