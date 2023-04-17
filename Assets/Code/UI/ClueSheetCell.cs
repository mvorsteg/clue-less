using UnityEngine;
using UnityEngine.UI;

public class ClueSheetCell : MonoBehaviour
{
    public ClueMark mark;
    public Text buttonText;

    private void Start()
    {
        mark = ClueMark.Blank;
        buttonText.text = "";
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
}