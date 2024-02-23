using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonTileData", menuName = "BagOfDungeon/Create Dungeon Tile Data")]
public class DungeonTileData : ScriptableObject
{
    
    public DungeonTileType DungeonTileType;
    public string ConnectionCodeString = "0000";
    public GameObject TilePrefab;
    
    
    private byte ConnectionsCode = 0b0000_0000;

    public void Initialise()
    {
        ConnectionsCode = Convert.ToByte(ConnectionCodeString, 2);
    }

    public byte GetRotatedConnectionsCode(int rotationSteps)
    {
        rotationSteps %= 4;

        if (rotationSteps < 0)
            rotationSteps = 4 + rotationSteps;

        if (rotationSteps == 0)
            return ConnectionsCode;

        int inverseRotation = 4 - rotationSteps;

        byte mask = (byte) (Math.Pow(2, rotationSteps) - 1);
        byte baseMasked = (byte) (ConnectionsCode & mask);
        byte baseMaskedLeftShifted = (byte) (baseMasked << inverseRotation);
        byte baseRightShifted = (byte) (ConnectionsCode >> rotationSteps);
        byte baseRotated = (byte) (baseRightShifted | baseMaskedLeftShifted);
        
        return baseRotated;
    }
}

public enum DungeonTileType
{
    Start,
    End,
    Corner,
    Straight,
    Junction,
    Cross,
    Cap
}