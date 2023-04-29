using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUpdateUI : MonoBehaviour
{
    public Dropdown characterDropdown;
    public GameObject visualsObj;
    private Dictionary<CharacterType, bool> choices;

    public GuestEngine engine;
    public ClientNetworkInterface netInterface;

    private void Awake()
    {
        choices = new Dictionary<CharacterType, bool>() { { CharacterType.Plum, true },
                                                          { CharacterType.Scarlet, true },
                                                          { CharacterType.Mustard, true },
                                                          { CharacterType.Peacock, true },
                                                          { CharacterType.Green, true },
                                                          { CharacterType.White, true } };
    }

    private void Start()
    {
        visualsObj.SetActive(false);
    }

    public void UpdateChoices(CharacterType character, bool isAvailable)
    {
        if (choices.ContainsKey(character))
        {
            choices[character] = isAvailable;
        }
    }
    
    public void PopulateChoices()
    {
        characterDropdown.ClearOptions();
        characterDropdown.AddOptions(choices.Where(x => x.Value).Select(x => x.Key.ToString()).ToList());
    }

    public void SubmitUpdate()
    {
        if (CharacterType.TryParse(characterDropdown.options[characterDropdown.value].text, true, out CharacterType character))
        {
            CharUpdatePacket pkt = new CharUpdatePacket(true, engine.ID, character);
            netInterface.SendMessage(pkt);
        }
    }
}