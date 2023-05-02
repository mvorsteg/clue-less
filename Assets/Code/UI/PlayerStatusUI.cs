using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    public Text statusText, nameText;
    public Image image;
    void Start()
    { 
        Font font = Resources.Load("FredokaOne-Regular") as Font;
        statusText.font = font;
        nameText.font = font;
    }

    public void SetName(string name)
    {
        nameText.text = name;
    }

    public void SetCharacter(Sprite characterSprite)
    {
        image.sprite = characterSprite;
        image.preserveAspect = true;
    }
    
    public void SetStatus(PlayerStatus status)
    {
        statusText.text = status.ToString();

    }
}