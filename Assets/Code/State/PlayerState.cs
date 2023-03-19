using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    public int playerID;
    public string playerName;
    public CharacterType character;
    public List<ClueCard> cards;
    public RoomType currentRoom;
    // Start is called before the first frame update

    public PlayerState(int playerID, string playerName, CharacterType character)
    {
        this.playerID = playerID;
        this.playerName = playerName;
        this.character = character;
    }
}
