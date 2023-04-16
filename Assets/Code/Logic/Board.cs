using System;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public RoomType[,] rooms;
    public Dictionary<RoomType, RoomType> secretPassages;
    public Dictionary<CharacterType, RoomType> startingRooms;

    private void Awake()
    {
        rooms = new RoomType[,]
        {
            { RoomType.Study, RoomType.HW_StudyHall, RoomType.Hall, RoomType.HW_HallLounge, RoomType.Lounge },
            { RoomType.HW_StudyLib, RoomType.Invalid, RoomType.HW_HallBill, RoomType.Invalid, RoomType.HW_LoungeDining },
            { RoomType.Library, RoomType.HW_LibBill, RoomType.BilliardRoom, RoomType.HW_BillDining, RoomType.DiningRoom },
            { RoomType.HW_LibCons, RoomType.Invalid, RoomType.HW_BillBall, RoomType.Invalid, RoomType.HW_DiningKitchen },
            { RoomType.Conservatory, RoomType.HW_ConsBall, RoomType.Ballroom, RoomType.HW_BallKitchen, RoomType.Kitchen } 
        };
        secretPassages = new Dictionary<RoomType, RoomType> 
        {
            { RoomType.Study, RoomType.Kitchen },
            { RoomType.Lounge, RoomType.Conservatory },
            { RoomType.Conservatory, RoomType.Lounge },
            { RoomType.Kitchen, RoomType.Study }
        };

        startingRooms = new Dictionary<CharacterType, RoomType>
        {
            { CharacterType.Plum, RoomType.HW_StudyLib },
            { CharacterType.Scarlet, RoomType.HW_HallLounge },
            { CharacterType.Mustard, RoomType.HW_LoungeDining },
            { CharacterType.Peacock, RoomType.HW_LibCons },
            { CharacterType.Green, RoomType.HW_ConsBall },
            { CharacterType.White, RoomType.HW_BallKitchen },
        };
    }



    public RoomType GetStartingRoom(CharacterType character)
    {
        if (startingRooms.TryGetValue(character, out RoomType room))
        {
            return room;
        }
        // error!
        Debug.Log(String.Format("Undefined starting room for {0}", character));
        return RoomType.Study;
    }

    public bool IsValidMove(RoomType startRoom, RoomType destRoom)
    {
        // get index of start room
        int row = -1, col = -1;
        for (int i = 0; i < rooms.GetLength(0); i++)
        {
            for (int j = 0; j < rooms.GetLength(1); j++)
            {
                if (rooms[i, j] == startRoom)
                {
                    row = i;
                    col = j;
                }
            }
        }

        // check row to left
        if (row > 0 && rooms[row - 1, col] == destRoom)
        {
            return true;
        }
        if (row < rooms.GetLength(0) - 1 && rooms[row + 1, col] == destRoom)
        {
            return true;
        }
        if (col > 0 && rooms[row, col - 1] == destRoom)
        {
            return true;
        }
        if (col < rooms.GetLength(1) - 1 && rooms[row, col + 1] == destRoom)
        {
            return true;
        }
        if (secretPassages.TryGetValue(startRoom, out RoomType secretRoom))
        {
            return secretRoom == destRoom;
        }
        return false;
    }

    public bool IsGuessRoom(RoomType room)
    {
        return room <= RoomType.Kitchen;
    }
}