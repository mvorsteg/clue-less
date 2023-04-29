using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabbedButtons : MonoBehaviour
{
    public List<Button> tabs;
    private void Start()
    {
        ClickTab(0);
        tabs[0].onClick.Invoke();
    }

    public void ClickTab(int selectedTab)
    {
        foreach(Button tab in tabs)
        {
            tab.interactable = true;
        }
        tabs[selectedTab].interactable = false;
    }
}