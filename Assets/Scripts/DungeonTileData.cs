using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "DungeonTileData", menuName = "BagOfDungeon/Create Dungeon Tile Data")]
public class DungeonTileData : ScriptableObject
{

    [FormerlySerializedAs("DungeonTileType")] public DungeonTileType TileType;
    [FormerlySerializedAs("ConnectionCodeString")] public string OutputsCodeString = "0000";
    public GameObject TilePrefab;

    private byte OutputsCode = 0b0000_0000;

    public void Initialise()
    {
        OutputsCode = Convert.ToByte(OutputsCodeString, 2);
    }

    /// <summary>
    /// Get the byte code for which directions this tile connects to when rotated in 90 degree increments of "rotationSteps".
    /// </summary>
    public byte GetRotatedOutputsCode(int rotationSteps)
    {
        rotationSteps %= 4;

        if (rotationSteps < 0)
            rotationSteps = 4 + rotationSteps;

        if (rotationSteps == 0)
            return OutputsCode;

        int inverseRotation = 4 - rotationSteps;

        byte mask = (byte)(Math.Pow(2, rotationSteps) - 1);
        byte baseMasked = (byte)(OutputsCode & mask);
        byte baseMaskedLeftShifted = (byte)(baseMasked << inverseRotation);
        byte baseRightShifted = (byte)(OutputsCode >> rotationSteps);
        byte baseRotated = (byte)(baseRightShifted | baseMaskedLeftShifted);

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