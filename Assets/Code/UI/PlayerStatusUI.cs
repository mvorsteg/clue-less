using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    public Text statusText, nameText;
    public Image image;

    public void SetName(string name)
    {
        nameText.text = name;
    }

    public void SetCharacter(Sprite characterSprite)
    {
        image.sprite = characterSprite;
    }
    
    public void SetStatus(PlayerStatus status)
    {
        statusText.text = status.ToString();
    }
}