using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BoardUI : MonoBehaviour
{
    private BoardRoom[] rooms;
    public Board board;
    public Transform roomParent;

    private void Start()
    {
        rooms = roomParent.GetComponentsInChildren<BoardRoom>(true);    
        Debug.Log(rooms.Length);
        DisableAllRooms();
    }

    public void MoveIconToNewRoom(CharacterType character, RoomType oldRoom, RoomType newRoom)
    {
        BoardRoom oldRoomUI = rooms.First(x => x.type == oldRoom);
        BoardRoom newRoomUI = rooms.First(x => x.type == newRoom);

        if (oldRoomUI != null && newRoomUI != null)
        {
            oldRoomUI.SetCharacterInRoom(character, false);
            newRoomUI.SetCharacterInRoom(character, true);
        }
    }

    public void DisableAllRooms()
    {
        foreach (BoardRoom room in rooms)
        {
            room.GetComponent<Button>().interactable = false;
        }
    }

    public void EnableMoveSelection(RoomType startingRoom)
    {
        DisableAllRooms();
        foreach (BoardRoom room in rooms)
        {
            Button roomButton = room.GetComponent<Button>();
            if (board.IsValidMove(startingRoom, room.type))
            {
                roomButton.interactable = true;
            }
        }
    }
}