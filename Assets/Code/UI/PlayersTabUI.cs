using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayersTabUI : MonoBehaviour
{
    public GameObject playerEntryPrefab;
    public Transform playerEntryParent;
    private Dictionary<int, PlayerStatusUI> entries = new Dictionary<int, PlayerStatusUI>();

    public List<CharacterSpriteMapping> spriteMappings;
    private Dictionary<CharacterType, Sprite> characterIcons;

    private bool isReady = false;
    public Text readyButtonText;
    public Button charButton, readyButton; 

    public GuestEngine engine;
    public ClientNetworkInterface netInterface;

    private void Awake()
    {
        characterIcons = new Dictionary<CharacterType, Sprite>();
        foreach (CharacterSpriteMapping mapping in spriteMappings)
        {
            if (!characterIcons.TryAdd(mapping.character, mapping.sprite))
            {
                // uhh error
            }
        }
        SetReady(false);
    }

    public void StartGame()
    {
        charButton.interactable = false;
        readyButton.interactable = false;
    }

    public void AddPlayer(int playerID, CharacterType character, string name)
    {
        PlayerStatusUI newEntry = Instantiate(playerEntryPrefab, playerEntryParent).GetComponent<PlayerStatusUI>();
        if (playerID == engine.ID)
        {
            newEntry.SetName(name + " (you)");
        }
        else
        {
            newEntry.SetName(name);
        }

        entries.Add(playerID, newEntry);

        SetCharacter(playerID, character);
        SetStatus(playerID, PlayerStatus.NotReady);
    }

    public void SetCharacter(int playerID, CharacterType character)
    {
        if (entries.TryGetValue(playerID, out PlayerStatusUI entry))
        {
            if (characterIcons.TryGetValue(character, out Sprite sprite))
            {
                entry.SetCharacter(sprite);
            }
        }
    }

    public void SetStatus(int playerID, PlayerStatus status)
    {
        if (entries.TryGetValue(playerID, out PlayerStatusUI entry))
        {
            entry.SetStatus(status);
        }
        
        // if the local player readying or unreadying, we need to toggle the button in addition to setting status
        if (status == PlayerStatus.Ready && playerID == engine.ID)
        {
            SetReady(true);
        }
        else if (status == PlayerStatus.NotReady && playerID == engine.ID)
        {
            SetReady(false);
        }

        // if game was just won, set all other players to lost
        else if (status == PlayerStatus.Won)
        {
            foreach (int key in entries.Keys)
            {
                if (key != playerID)
                {
                    SetStatus(key, PlayerStatus.Lost);
                }
            }
        }
    }

    public void GameStart()
    {
        charButton.interactable = false;
        readyButton.interactable = false;
    }

    public void ReadyButtonClick()
    {
        ReadyPacket pkt = new ReadyPacket(true, engine.ID, !isReady);
        netInterface.SendMessage(pkt);
    }

    public void SetReady(bool value)
    {
        isReady = value;
        if (isReady)
        {
            readyButtonText.text = "Unready";
        }
        else
        {
            readyButtonText.text = "Ready";
        }
    }
}