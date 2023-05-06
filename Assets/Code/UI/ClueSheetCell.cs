using UnityEngine;
using UnityEngine.UI;

public class ClueSheetCell : MonoBehaviour
{
    public ClueMark mark = ClueMark.Blank;
    public Text buttonText;

    private void Start()
    {
        if (buttonText.text == "Button")
        {
            buttonText.text = "";
        }
    }

    public void Mark()
    {
        switch (mark)
        {
            case ClueMark.Blank:
            {
                mark = ClueMark.X;
                buttonText.text = "X";
                break;
            }
            case ClueMark.X:
            {
                mark = ClueMark.Blank;
                buttonText.text = "";
                break;
            }
        }
    }

    public void Mark(bool value)
    {
        if (value)
        {
            mark = ClueMark.X;
            buttonText.text = "X";
        }
        else
        {
            mark = ClueMark.Blank;
            buttonText.text = "";
            
        }
    }
}