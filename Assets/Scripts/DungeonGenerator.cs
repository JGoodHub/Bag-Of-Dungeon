using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class DungeonGenerator : MonoBehaviour
{

    [SerializeField] private int _seed;
    [SerializeField] private AnimationCurve _compactnessCurve;
    [SerializeField] private TileStack _tileStack;
    [Space]
    [SerializeField] private Transform _tilesContainer;
    [SerializeField] private List<DungeonTileData> _tiles;

    private Dungeon _dungeon;

    private void Start()
    {
        _seed = _seed < 0 ? DateTime.Now.GetHashCode() : _seed;

        _tileStack.Initialise(_seed);

        foreach (DungeonTileData tile in _tiles)
        {
            tile.Initialise();
        }

        _dungeon = GenerateDungeonInstance(_seed, _tileStack);

        // List<DungeonTile> shortestPath = _dungeon.Pathfinding.GetShortestPath(_dungeon.GetStartTile(), _dungeon.GetRandomTile());
        //
        // for (int i = 0; i < shortestPath.Count - 1; i++)
        // {
        //     Debug.DrawLine(shortestPath[i].transform.position + Vector3.up, shortestPath[i + 1].transform.position + Vector3.up, Color.red, 50f);
        // }
    }

    private Dungeon GenerateDungeonInstance(int seed, TileStack stack)
    {
        Random random = new Random(seed);

        // Create the instance object

        GameObject dungeonInstanceObject = new GameObject("DungeonInstance");
        Dungeon dungeon = dungeonInstanceObject.AddComponent<Dungeon>();

        List<Dungeon.Cell> dungeonCells = new List<Dungeon.Cell>();

        // Create the start tile

        dungeonCells.Add(new Dungeon.Cell
        {
            TileData = GetDataForTileType(DungeonTileType.Start),
            Position = Vector3Int.zero,
            Rotation = 2
        });

        List<Vector3Int> occupiedSpaces = new List<Vector3Int> { Vector3Int.zero };
        List<Vector3Int> availableSpaces = new List<Vector3Int>();

        availableSpaces.AddRange(dungeonCells[0].GetOutputSpaces());

        int loopCounter = 0;

        while (stack.IsEmpty() == false && loopCounter++ < 1000)
        {
            Dictionary<Dungeon.Cell, List<Vector3Int>> connectionsList = new Dictionary<Dungeon.Cell, List<Vector3Int>>();

            foreach (Dungeon.Cell cell in dungeonCells)
            {
                connectionsList.Add(cell, cell.GetOutputSpaces());
            }

            DungeonTileType peekedTileType = stack.PeekTile();
            DungeonTileData peekedTileData = _tiles.Find(tile => tile.TileType == peekedTileType);

            // Check if theres a valid space for the tile
            List<Vector3Int> validSpaces = new List<Vector3Int>();
            Dictionary<Vector3Int, List<int>> validRotationsForValidSpaces = new Dictionary<Vector3Int, List<int>>();

            foreach (Vector3Int availableSpace in availableSpaces)
            {
                byte tileInputsCode = (byte)GetTileInputsCodeForSpace(availableSpace, connectionsList);
                byte walledInputsCode = (byte)GetWalledInputsCodeForSpace(availableSpace, occupiedSpaces, connectionsList);

                for (int rotIndex = 0; rotIndex < 4; rotIndex++)
                {
                    byte rotatedConnectionsCode = peekedTileData.GetRotatedOutputsCode(rotIndex);

                    // This rotation doesn't connect to a tile
                    if ((rotatedConnectionsCode & tileInputsCode) == 0)
                        continue;

                    // One of the drawn tile connections faces a wall
                    if ((rotatedConnectionsCode & walledInputsCode) > 0)
                        continue;

                    // TODO Check out walled outputs code against the spaces tile inputs to make sure we're not walling off an input

                    if (validRotationsForValidSpaces.ContainsKey(availableSpace))
                    {
                        validRotationsForValidSpaces[availableSpace].Add(rotIndex);
                    }
                    else
                    {
                        validRotationsForValidSpaces.Add(availableSpace, new List<int> { rotIndex });
                    }
                }

                if (validRotationsForValidSpaces.ContainsKey(availableSpace) == false)
                    continue;

                validSpaces.Add(availableSpace);
            }

            // There are currently no valid spaces for this type of tile, move it to the bottom and try again
            if (validSpaces.Count == 0)
            {
                stack.PopToBottom();
                continue;
            }

            // Keep the dungeon fairly compact but not fully round
            // Dictionary<float, Vector3Int> spacesByChooseChance = validSpaces.ToDictionary(space => _compactnessCurve.Evaluate(space.magnitude), space => space);

            Vector3Int chosenSpace = validSpaces[0];
            float chosenSpaceDist = float.MaxValue;

            foreach (Vector3Int validSpace in validSpaces)
            {
                float distToSpace = Vector3.Distance(validSpace, Vector3Int.zero);

                if (distToSpace < chosenSpaceDist)
                {
                    chosenSpace = validSpace;
                    chosenSpaceDist = distToSpace;
                }
            }

            // Create the tile object
            stack.PopTile();

            Dungeon.Cell newDungeonCell = new Dungeon.Cell
            {
                TileData = peekedTileData,
                Position = chosenSpace,
                Rotation = validRotationsForValidSpaces[chosenSpace][random.Next(0, validRotationsForValidSpaces[chosenSpace].Count)]
            };
            
            dungeonCells.Add(newDungeonCell);

            // Update the space lists
            availableSpaces.Remove(chosenSpace);
            occupiedSpaces.Add(chosenSpace);

            // Add the newly available positions (that aren't already occupied) to the search area
            List<Vector3Int> potentialAvailablePositions = newDungeonCell.GetOutputSpaces();
            potentialAvailablePositions.RemoveAll(potentialPosition => occupiedSpaces.Contains(potentialPosition));
            potentialAvailablePositions.RemoveAll(potentialPosition => availableSpaces.Contains(potentialPosition));

            availableSpaces.AddRange(potentialAvailablePositions);
        }
        
        // Cap all the open corridors
        foreach (Vector3Int availableSpace in availableSpaces)
        {
            Dictionary<Dungeon.Cell, List<Vector3Int>> connectionsList = new Dictionary<Dungeon.Cell, List<Vector3Int>>();

            foreach (Dungeon.Cell cell in dungeonCells)
            {
                connectionsList.Add(cell, cell.GetOutputSpaces());
            }

            DungeonTileData peekedTileData = GetDataForTileType(DungeonTileType.Cap);
            
            byte tileInputsCode = (byte)GetTileInputsCodeForSpace(availableSpace, connectionsList);
            byte walledInputsCode = (byte)GetWalledInputsCodeForSpace(availableSpace, occupiedSpaces, connectionsList);

            int matchingRotation = 0;

            for (int rotIndex = 0; rotIndex < 4; rotIndex++)
            {
                byte rotatedConnectionsCode = peekedTileData.GetRotatedOutputsCode(rotIndex);

                // This rotation doesn't connect to a tile
                if ((rotatedConnectionsCode & tileInputsCode) == 0)
                    continue;

                // One of the drawn tile connections faces a wall
                if ((rotatedConnectionsCode & walledInputsCode) > 0)
                    continue;

                // TODO Check our walled outputs code against the space's tile inputs to make sure we're not walling off an input

                matchingRotation = rotIndex;
                break;
            }
         
            Dungeon.Cell newDungeonCell = new Dungeon.Cell
            {
                TileData = peekedTileData,
                Position = availableSpace,
                Rotation = matchingRotation
            };
            
            dungeonCells.Add(newDungeonCell);
        }

        // Create all the tile game objects from the dungeon model

        foreach (Dungeon.Cell cell in dungeonCells)
        {
            GameObject tilePrefab = GetPrefabForTileType(cell.TileData.TileType);
            DungeonTile dungeonTile = Instantiate(tilePrefab, cell.Position, Quaternion.Euler(0, 90 * cell.Rotation, 0), _tilesContainer).GetComponent<DungeonTile>();
            dungeonTile.Initialise(cell.Position);
        }

        availableSpaces.Clear();

        // Hide all the tiles
        // foreach (DungeonTile tile in tiles)
        // {
        //     dungeon.AddTile(tile);
        //
        //     if (tile.DungeonTileType != TileBag.DungeonTileType.Start)
        //         tile.Hide();
        //
        //     // Get a list of the all the tiles connected to this one
        //     List<Vector3Int> connectedTilePositions = tile.GetAdjacentTilePositions(true);
        //
        //     foreach (DungeonTile otherTile in tiles)
        //     {
        //         if (tile == otherTile)
        //             continue;
        //
        //         foreach (Vector3Int connectedTilePosition in connectedTilePositions)
        //         {
        //             if ((otherTile.transform.position - connectedTilePosition).sqrMagnitude >= 0.01f)
        //                 continue;
        //
        //             tile.ConnectedTiles.Add(otherTile);
        //             break;
        //         }
        //     }
        // }

        dungeon.FinaliseSetup();

        return dungeon;
    }

    private DungeonTileData GetDataForTileType(DungeonTileType tileType)
    {
        return _tiles.Find(tile => tile.TileType == tileType);
    }

    private GameObject GetPrefabForTileType(DungeonTileType tileType)
    {
        return _tiles.Find(tile => tile.TileType == tileType)?.TilePrefab;
    }

    private int GetConnectionsRequired(DungeonTileType tileType)
    {
        switch (tileType)
        {
            case DungeonTileType.Start:
            case DungeonTileType.End:
            case DungeonTileType.Cap:
                return 1;
            case DungeonTileType.Corner:
            case DungeonTileType.Straight:
                return 2;
            case DungeonTileType.Cross:
                return 4;
        }

        return 0;
    }

    private List<Vector3Int> GetAdjacentPositions(Vector3Int centre)
    {
        return new List<Vector3Int>
        {
            centre + Vector3Int.forward,
            centre + Vector3Int.right,
            centre + Vector3Int.back,
            centre + Vector3Int.left
        };
    }

    /// <summary>
    /// Returns a byte code representing which directions have a connection to another tile. <br/>
    /// E.g. 0110 (UP/RIGHT/DOWN/LEFT) means a tile to the right and a tile below have incoming connections to this one
    /// </summary>
    private int GetTileInputsCodeForSpace(Vector3Int availableSpace, Dictionary<Dungeon.Cell, List<Vector3Int>> connectionsList)
    {
        List<Vector3Int> positionsLeadingToSpace = new List<Vector3Int>();

        foreach ((Dungeon.Cell cell, List<Vector3Int> connectedSpaces) in connectionsList)
        {
            if (connectedSpaces.Contains(availableSpace))
            {
                positionsLeadingToSpace.Add(cell.Position);
            }
        }

        byte inputCode = 0b0000_0000;

        foreach (Vector3Int adjacentPosition in GetAdjacentPositions(availableSpace))
        {
            inputCode <<= 1;

            if (positionsLeadingToSpace.Contains(adjacentPosition))
            {
                inputCode |= 0b000_0001;
            }
        }

        return inputCode;
    }

    /// <summary>
    /// Returns a byte code representing which directions have a wall against this tile  <br/>
    /// E.g. 0101 (UP/RIGHT/DOWN/LEFT) means the tiles to the right and left have walls against this one.
    /// </summary>
    private int GetWalledInputsCodeForSpace(Vector3Int availableSpace, List<Vector3Int> occupiedSpaces, Dictionary<Dungeon.Cell, List<Vector3Int>> connectionsList)
    {
        List<Vector3Int> positionsLeadingToSpace = new List<Vector3Int>();

        foreach ((Dungeon.Cell cell, List<Vector3Int> connectedSpaces) in connectionsList)
        {
            if (connectedSpaces.Contains(availableSpace))
            {
                positionsLeadingToSpace.Add(cell.Position);
            }
        }

        byte inputCode = 0b0000_0000;

        foreach (Vector3Int adjacentPosition in GetAdjacentPositions(availableSpace))
        {
            inputCode <<= 1;

            if (occupiedSpaces.Contains(adjacentPosition) && positionsLeadingToSpace.Contains(adjacentPosition) == false)
            {
                inputCode |= 0b000_0001;
            }
        }

        return inputCode;
    }

}