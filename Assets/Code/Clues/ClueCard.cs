using System;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ClueCard", menuName = "")]
public class ClueCard : ScriptableObject
{
    public ClueType type;
    public string cardName;
    public Sprite sprite;

    public bool TryGetCharacterType(out CharacterType character)
    {
        if (Enum.TryParse<CharacterType>(name, out character))
        {
            return true;
        }
        return false;
    }

    public bool TryGetWeaponType(out WeaponType weapon)
    {
        if (Enum.TryParse<WeaponType>(name, out weapon))
        {
            return true;
        }
        return false;
    }

    public bool TryGetRoomType(out RoomType room)
    {
        if (Enum.TryParse<RoomType>(name, out room))
        {
            return true;
        }
        return false;
    }
}