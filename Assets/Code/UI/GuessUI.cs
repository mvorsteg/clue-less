using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GuessUI : MonoBehaviour
{
    public Dropdown characterDropdown, weaponDropdown, roomDropdown;
    public GameObject guessObj, warningObj;
    private ClientNetworkInterface netInterface;
    private GuestEngine engine;
    
    private bool isFinal = false; 

    private void Awake()
    {
        characterDropdown.AddOptions(new List<string> { CharacterType.Plum.ToString(), CharacterType.Scarlet.ToString(), CharacterType.Mustard.ToString(),
                                                        CharacterType.Peacock.ToString(), CharacterType.Green.ToString(), CharacterType.White.ToString() });
        weaponDropdown.AddOptions(new List<string> { WeaponType.Rope.ToString(), WeaponType.Candle.ToString(), WeaponType.Knife.ToString(),
                                                        WeaponType.Wrench.ToString(), WeaponType.Pipe.ToString(), WeaponType.Revolver.ToString() });
        roomDropdown.AddOptions(new List<string> { RoomType.Study.ToString(), RoomType.Hall.ToString(), RoomType.Lounge.ToString(),
                                                        RoomType.Library.ToString(), RoomType.BilliardRoom.ToString(), RoomType.DiningRoom.ToString(),
                                                        RoomType.Conservatory.ToString(), RoomType.Ballroom.ToString(), RoomType.Kitchen.ToString() });
    }

    private void Start()
    {
        netInterface = FindObjectOfType<ClientNetworkInterface>();
        engine = FindObjectOfType<GuestEngine>();
        guessObj.SetActive(false);
        warningObj.SetActive(false);
    }

    public void PromptGuess(bool isFinal, RoomType currentRoom)
    {
        if (!isFinal)
        {
            bool isInValidRoom = false;
            roomDropdown.interactable = false;
            for (int i = 0; i < roomDropdown.options.Count; i++)
            {
                if (roomDropdown.options[i].text == currentRoom.ToString())
                {
                    roomDropdown.value = i;
                    isInValidRoom = true;
                    break;
                }
            }
            if (!isInValidRoom)
            {
                // not really an error I guess
                return;
            }
        }
        else
        {
            roomDropdown.interactable = true;
        }
        this.isFinal = isFinal;
        if (!isFinal)
        {
            guessObj.SetActive(true);
        }
        else
        {
            warningObj.SetActive(true);
        }
    }

    public void SubmitGuess()
    {
        if (CharacterType.TryParse(characterDropdown.options[characterDropdown.value].text, true, out CharacterType character) &&
            WeaponType.TryParse(weaponDropdown.options[weaponDropdown.value].text, true, out WeaponType weapon) &&
            RoomType.TryParse(roomDropdown.options[roomDropdown.value].text, true, out RoomType room))
        {
            GuessPacket pkt = new GuessPacket(true, engine.ID, isFinal, character, weapon, room);
            netInterface.SendMessage(pkt);
        }
        guessObj.SetActive(false);
    }
}