using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon : MonoBehaviour
{

    public class Cell
    {

        public DungeonTileData TileData;
        public Vector3Int Position;
        public int Rotation;

        public List<Vector3Int> GetOutputSpaces()
        {
            List<Vector3Int> outputSpaces = new List<Vector3Int>();
            byte rotatedOutputsCode = TileData.GetRotatedOutputsCode(Rotation);

            for (int rotIndex = 0; rotIndex < 4; rotIndex++)
            {
                if ((rotatedOutputsCode & 0b0000_1000) > 0)
                {
                    Vector3 space = Position + (Quaternion.Euler(0, 90 * rotIndex, 0) * Vector3.forward);
                    Vector3Int spaceRounded = new Vector3Int(Mathf.RoundToInt(space.x), Mathf.RoundToInt(space.y), Mathf.RoundToInt(space.z));

                    outputSpaces.Add(spaceRounded);
                }

                rotatedOutputsCode <<= 1;
            }

            return outputSpaces;
        }

    }

    private List<DungeonTile> _tiles;

    private DungeonPathfinding _pathfinding;

    public DungeonPathfinding Pathfinding => _pathfinding;

    public void AddTile(DungeonTile newTile)
    {
        _tiles ??= new List<DungeonTile>();
        _tiles.Add(newTile);
    }

    public void FinaliseSetup()
    {
        _pathfinding = new DungeonPathfinding(_tiles);
    }

    public DungeonTile GetStartTile()
    {
        return _tiles[0];
    }

    public DungeonTile GetRandomTile()
    {
        return _tiles[Random.Range(0, _tiles.Count)];
    }

}