using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClueSheet : MonoBehaviour
{
    public Transform clueLabelParent, playerLabelParent;
    public GameObject[] columns;

    private bool initialized = false;
    
    private void Start()
    {
        if (!initialized)
        {
            int i = 0;
            foreach (CharacterType character in Enum.GetValues(typeof(CharacterType)))
            {
                Text text = clueLabelParent.GetChild(i).GetComponent<Text>();
                text.text = character.ToString();
                i++;
            }
            foreach (WeaponType weapon in Enum.GetValues(typeof(WeaponType)))
            {
                Text text = clueLabelParent.GetChild(i).GetComponent<Text>();
                text.text = weapon.ToString();
                i++;
            }
            foreach (RoomType room in Enum.GetValues(typeof(RoomType)))
            {
                Text text = clueLabelParent.GetChild(i).GetComponent<Text>();
                text.text = room.ToString();
                i++;
                if (i >= clueLabelParent.childCount)
                {
                    break;
                }
            }

            foreach (GameObject col in columns)
            {
                col.SetActive(false);
            }
            for (int j = 0; j < playerLabelParent.childCount; j++)
            {
                playerLabelParent.GetChild(j).gameObject.SetActive(false);
            }

            initialized = true;
        }
    }

    public void StartGame(List<string> otherPlayers)
    {
        // quick and dirty fix to get around initialization issue (if StartGame called before Start we disable all cols)
        if (!initialized)
        {
            Start();
        }
        for (int i = 0; i < otherPlayers.Count; i++)
        {
            Text text = playerLabelParent.GetChild(i).GetComponent<Text>();
            text.text = otherPlayers[i];
            text.gameObject.SetActive(true);
            columns[i].SetActive(true);
        }
    }

    // public void Mark(int row, int col)
}