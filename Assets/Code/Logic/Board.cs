using System;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public RoomType[,] rooms;
    public Dictionary<RoomType, RoomType> secretPassages;

    private void Awake()
    {
        rooms = new RoomType[,]
        {
            { RoomType.Study, RoomType.Hall, RoomType.Lounge },
            { RoomType.Library, RoomType.BilliardRoom, RoomType.DiningRoom },
            { RoomType.BilliardRoom, RoomType.Ballroom, RoomType.Kitchen} 
        };
        secretPassages = new Dictionary<RoomType, RoomType> 
        {
            { RoomType.Study, RoomType.Kitchen },
            { RoomType.Lounge, RoomType.BilliardRoom },
            { RoomType.BilliardRoom, RoomType.Lounge },
            { RoomType.Kitchen, RoomType.Study }
        };
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
        if (row < rooms.GetLength(0) && rooms[row + 1, col] == destRoom)
        {
            return true;
        }
        if (col > 0 && rooms[row, col - 1] == destRoom)
        {
            return true;
        }
        if (col < rooms.GetLength(1) && rooms[row, col + 1] == destRoom)
        {
            return true;
        }
        if (secretPassages.TryGetValue(startRoom, out RoomType secretRoom))
        {
            return secretRoom == destRoom;
        }
        return false;
    }
}