using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HostEngine : BaseEngine
{
    public ServerNetworkInterface netInterface;
    public override bool StartGame()
    {
        base.StartGame();
        // send cards to players
        int cardsPerPlayer = Mathf.CeilToInt((float)(deck.TotalCards - 3) / (float)players.Keys.Count);
        foreach (PlayerState player in players.Values)
        {
            player.cards = deck.GetCards(cardsPerPlayer);

            RoomType initialRoom = board.GetStartingRoom(player.character);
            MoveToRoomPacket roomPkt = new MoveToRoomPacket(false, player.playerID, initialRoom);
            netInterface.Broadcast(NetworkConstants.SERVER_ID, roomPkt);
            if (CardDeck.GetCluesFromCards(player.cards, out List<CharacterType> characters, out List<WeaponType> weapons, out List<RoomType> rooms))
            {
                GameStartPacket gamePkt = new GameStartPacket(false, player.playerID, characters, weapons, rooms);
                netInterface.SendMessage(player.playerID, gamePkt);
            }
        }

        return true;        
    }
    public bool AddPlayer(int playerID, string name, out CharacterType assignedCharacter)
    {
        // find available character to assign
        Dictionary<CharacterType, bool> availableChars = new Dictionary<CharacterType, bool>();
        foreach (CharacterType character in Enum.GetValues(typeof(CharacterType)))
        {
            availableChars.Add(character, true);
        }
        foreach (PlayerState player in players.Values)
        {
            availableChars[player.character] = false;
        }
        foreach (CharacterType character in availableChars.Keys)
        {
            if (availableChars[character])
            {
                assignedCharacter = character;
                PlayerState newPlayer = new PlayerState(playerID, name, assignedCharacter);
                if (players.TryAdd(playerID, newPlayer))
                {
                    Log(string.Format("Adding player {0} to session", playerID));
                    return true;
                }
                break;
            }
        }
        
        // error adding player
        Log(string.Format("Error adding player {0}", playerID));
        assignedCharacter = CharacterType.Mustard;  // value does not matter at all if we fail. I just like col. Mustard the best
        return false;
    }

    public List<Tuple<int, string, CharacterType>> GetAllPlayerInfo()
    {
        List<Tuple<int, string, CharacterType>> allPlayerList = new List<Tuple<int, string, CharacterType>>();
        foreach (PlayerState ps in players.Values)
        {
            if (ps != null)
            {
                allPlayerList.Add(new Tuple<int, string, CharacterType>(ps.playerID, ps.playerName, ps.character));
            }
        }
        return allPlayerList;
    }

    public bool UpdateCharacter(int playerID, CharacterType newCharacter)
    {
        foreach (PlayerState player in players.Values)
        {
            if (player != null && player.character == newCharacter)
            {
                Log(String.Format("Cannot change {0} to {1}", playerID, newCharacter));
                return false;
            }
        }
        players[playerID].character = newCharacter;
        Log(String.Format("Changed {0} to {1}", playerID, newCharacter));
        return true;
    }

    public bool MovePlayer(int playerID, RoomType destRoom)
    {
        if (players.TryGetValue(playerID, out PlayerState playerState))
        {
            if (board.IsValidMove(playerState.currentRoom, destRoom))
            {
                // do move
                playerState.currentRoom = destRoom;
                Log(String.Format("Moved Client{0} to {1}", playerID, destRoom.ToString()));
                return true;
            }
        }
        Log("Illegal move");
        return false;
    }
}