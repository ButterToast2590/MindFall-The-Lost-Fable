using System;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public Vector3 playerPosition; 

    public GameData()
    {
        playerPosition = Vector3.zero;
    }
}
