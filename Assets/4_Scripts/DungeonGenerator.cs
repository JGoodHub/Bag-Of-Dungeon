using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GoodHub.Core.Runtime.Utils;
using UnityEngine;
using Random = System.Random;

public class DungeonGenerator : MonoBehaviour
{

    [SerializeField] private int _seed;
    [SerializeField] private AnimationCurve _compactnessCurve;
    [SerializeField] private TileStack _tileStack;
    [Space]
    [SerializeField] private List<DungeonTileData> _tiles;

    private DungeonGraph _dungeonGraph;

    private void Start()
    {
        _seed = _seed < 0 ? DateTime.Now.GetHashCode() : _seed;

        _tileStack.Initialise(_seed);

        foreach (DungeonTileData tile in _tiles)
        {
            tile.Initialise();
        }

        _dungeonGraph = GenerateDungeonInstance(_seed, _tileStack);

        List<DungeonNode> shortestPath = _dungeonGraph.GetShortestPath(_dungeonGraph.GetStartNode(), _dungeonGraph.GetEndNode());

        for (int i = 0; i < shortestPath.Count - 1; i++)
        {
            Debug.DrawLine(shortestPath[i].Position + Vector3.up * 0.5f, shortestPath[i + 1].Position + Vector3.up * 0.5f, Color.cyan, 5f);
        }
    }

    private DungeonGraph GenerateDungeonInstance(int seed, TileStack stack)
    {
        Random random = new Random(seed);

        // Create the instance object

        DungeonGraph dungeonGraph = new DungeonGraph();

        // Create the start tile

        DungeonNode startNode = new DungeonNode
        {
            TileData = GetDataForTileType(DungeonTileType.Start),
            Position = Vector3Int.zero,
            Rotation = 2
        };

        dungeonGraph.AddNode(startNode);

        List<Vector3Int> occupiedSpaces = new List<Vector3Int> { startNode.Position };
        List<Vector3Int> availableSpaces = new List<Vector3Int>(startNode.GetOutputSpaces());

        Dictionary<DungeonNode, List<Vector3Int>> adjacencyDict;

        while (stack.IsEmpty() == false)
        {
            adjacencyDict = GetAdjacencyDict(dungeonGraph);

            DungeonTileType peekedTileType = stack.PeekTile();
            DungeonTileData peekedTileData = _tiles.Find(tile => tile.TileType == peekedTileType);

            // Check if theres a valid space for the tile
            List<Vector3Int> validSpaces = new List<Vector3Int>();
            Dictionary<Vector3Int, List<int>> validRotationsForValidSpaces = new Dictionary<Vector3Int, List<int>>();

            foreach (Vector3Int availableSpace in availableSpaces)
            {
                byte tileInputsCode = (byte)GetTileInputsCodeForSpace(availableSpace, adjacencyDict);
                byte walledInputsCode = (byte)GetWalledInputsCodeForSpace(availableSpace, occupiedSpaces, adjacencyDict);

                for (int rotIndex = 0; rotIndex < 4; rotIndex++)
                {
                    byte rotatedConnectionsCode = peekedTileData.GetRotatedOutputsCode(rotIndex); // The code representing the output connections of the tile we're placing when rotated
                    byte rotateWallOutputsCode = (byte)(~rotatedConnectionsCode & 0b0000_1111); // The code representing the output walls of the tile we're placing when rotated, basically the invert of the above

                    // This rotation doesn't connect to a tile
                    if ((rotatedConnectionsCode & tileInputsCode) == 0)
                        continue;

                    // One of the drawn tile output connections faces an existing wall
                    if ((rotatedConnectionsCode & walledInputsCode) > 0)
                        continue;

                    // One of the drawn tiles output walls faces a input connection
                    if ((rotateWallOutputsCode & tileInputsCode) > 0)
                        continue;

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

            // Pick a valid space from the pool based on how compact we want the dungeon to be

            validSpaces = validSpaces.OrderBy(space => space.magnitude).ToList();

            int chosenSpaceIndex = Mathf.RoundToInt(_compactnessCurve.Evaluate(random.NextFloat()) * (validSpaces.Count - 1));
            Vector3Int chosenSpace = validSpaces[chosenSpaceIndex];

            // Create the tile object
            stack.PopTile();

            DungeonNode newDungeonNode = new DungeonNode
            {
                TileData = peekedTileData,
                Position = chosenSpace,
                Rotation = validRotationsForValidSpaces[chosenSpace][random.Next(0, validRotationsForValidSpaces[chosenSpace].Count)]
            };

            dungeonGraph.AddNode(newDungeonNode);

            // Update the space lists
            availableSpaces.Remove(chosenSpace);
            occupiedSpaces.Add(chosenSpace);

            // Add the newly available positions (that aren't already occupied) to the search area
            List<Vector3Int> potentialAvailablePositions = newDungeonNode.GetOutputSpaces();
            potentialAvailablePositions.RemoveAll(potentialPosition => occupiedSpaces.Contains(potentialPosition));
            potentialAvailablePositions.RemoveAll(potentialPosition => availableSpaces.Contains(potentialPosition));

            availableSpaces.AddRange(potentialAvailablePositions);
        }

        adjacencyDict = GetAdjacencyDict(dungeonGraph);

        // Cap all the open corridors

        foreach (Vector3Int availableSpace in availableSpaces)
        {
            byte tileInputsCode = (byte)GetTileInputsCodeForSpace(availableSpace, adjacencyDict);

            DungeonTileData peekedTileData = GetBestTileForInputs(tileInputsCode, out int rotation);

            DungeonNode newDungeonNode = new DungeonNode
            {
                TileData = peekedTileData,
                Position = availableSpace,
                Rotation = rotation
            };

            dungeonGraph.AddNode(newDungeonNode);

            occupiedSpaces.Add(availableSpace);
        }

        availableSpaces.Clear();
        adjacencyDict = GetAdjacencyDict(dungeonGraph);

        // Resolve all the edge connections in the graph

        List<DungeonNode> dungeonNodes = dungeonGraph.Nodes.ToList();

        foreach ((DungeonNode dungeonNode, List<Vector3Int> connectedSpaces) in adjacencyDict)
        {
            foreach (Vector3Int connectedSpace in connectedSpaces)
            {
                DungeonNode connectedNode = dungeonNodes.Find(node => node.Position == connectedSpace);

                if (connectedNode == null)
                    continue;

                dungeonGraph.AddEdge(dungeonNode, connectedNode);
            }
        }

        //dungeonGraph.DebugGraph(Vector3.up);

        // Create all the tile game objects from the dungeon model

        GameObject dungeonInstanceObject = new GameObject("DungeonInstance");

        foreach (DungeonNode cell in dungeonGraph.Nodes)
        {
            GameObject tilePrefab = cell.TileData.TilePrefab;
            DungeonTile dungeonTile = Instantiate(tilePrefab, cell.Position, Quaternion.Euler(0, 90 * cell.Rotation, 0), dungeonInstanceObject.transform).GetComponent<DungeonTile>();
            dungeonTile.Initialise(cell.Position);
        }

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
        
        return dungeonGraph;
    }

    public Dictionary<DungeonNode, List<Vector3Int>> GetAdjacencyDict(DungeonGraph graph)
    {
        Dictionary<DungeonNode, List<Vector3Int>> connectionsList = new Dictionary<DungeonNode, List<Vector3Int>>();

        foreach (DungeonNode cell in graph.Nodes)
        {
            connectionsList.Add(cell, cell.GetOutputSpaces());
        }

        return connectionsList;
    }

    private DungeonTileData GetDataForTileType(DungeonTileType tileType)
    {
        return _tiles.Find(tile => tile.TileType == tileType);
    }

    private GameObject GetPrefabForTileType(DungeonTileType tileType)
    {
        return _tiles.Find(tile => tile.TileType == tileType)?.TilePrefab;
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
    private int GetTileInputsCodeForSpace(Vector3Int availableSpace, Dictionary<DungeonNode, List<Vector3Int>> adjacencyDict)
    {
        List<Vector3Int> positionsLeadingToSpace = new List<Vector3Int>();

        foreach ((DungeonNode cell, List<Vector3Int> adjacentSpaces) in adjacencyDict)
        {
            if (adjacentSpaces.Contains(availableSpace))
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
    private int GetWalledInputsCodeForSpace(Vector3Int availableSpace, List<Vector3Int> occupiedSpaces, Dictionary<DungeonNode, List<Vector3Int>> adjacencyDict)
    {
        List<Vector3Int> positionsLeadingToSpace = new List<Vector3Int>();

        foreach ((DungeonNode cell, List<Vector3Int> adjacentSpaces) in adjacencyDict)
        {
            if (adjacentSpaces.Contains(availableSpace))
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

    private DungeonTileData GetBestTileForInputs(byte connectionInputs, out int rotation)
    {
        List<DungeonTileType> validTypes = new List<DungeonTileType>
        {
            DungeonTileType.Cap,
            DungeonTileType.Straight,
            DungeonTileType.Corner,
            DungeonTileType.Junction,
            DungeonTileType.Cross
        };

        foreach (DungeonTileType tileType in validTypes)
        {
            DungeonTileData tileData = GetDataForTileType(tileType);

            for (int rotIndex = 0; rotIndex < 4; rotIndex++)
            {
                if (tileData.GetRotatedOutputsCode(rotIndex) == connectionInputs)
                {
                    rotation = rotIndex;
                    return tileData;
                }
            }
        }

        rotation = 0;
        return null;
    }

}