using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class DungeonGraph
{

    private Dictionary<DungeonNode, List<DungeonNode>> _adjacencyList = new Dictionary<DungeonNode, List<DungeonNode>>();

    public Dictionary<DungeonNode, List<DungeonNode>>.KeyCollection Nodes => _adjacencyList.Keys;

    public void AddNode(DungeonNode node)
    {
        if (_adjacencyList.ContainsKey(node))
            return;

        _adjacencyList.Add(node, new List<DungeonNode>());
    }

    public void AddEdge(DungeonNode node, DungeonNode adjacentNode)
    {
        if (_adjacencyList.ContainsKey(node) == false)
        {
            throw new ArgumentException("Node is not in the adjacency list.");
        }

        _adjacencyList[node].Add(adjacentNode);
    }

    public DungeonNode GetStartNode()
    {
        return Nodes.FirstOrDefault(node => node.TileData.TileType == DungeonTileType.Start);
    }
    
    public DungeonNode GetEndNode()
    {
        return Nodes.FirstOrDefault(node => node.TileData.TileType == DungeonTileType.End);
    }

    public DungeonNode GetRandomNode()
    {
        return Nodes.ToList()[new Random().Next(0, Nodes.Count)];
    }

    public List<DungeonNode> GetShortestPath(DungeonNode start, DungeonNode end)
    {
        List<DungeonNode> path = new List<DungeonNode>();
        Dictionary<DungeonNode, int> distanceMap = new Dictionary<DungeonNode, int>();

        Queue<DungeonNode> searchQueue = new Queue<DungeonNode>();
        HashSet<DungeonNode> searchedTilesSet = new HashSet<DungeonNode>();

        searchQueue.Enqueue(start);
        distanceMap.Add(start, 0);

        // Calculate the max distance from the start tile to all other tiles in the dungeon
        while (searchQueue.Count > 0)
        {
            DungeonNode searchTile = searchQueue.Dequeue();
            searchedTilesSet.Add(searchTile);

            int searchTileDistance = distanceMap[searchTile];

            foreach (DungeonNode connectedTile in _adjacencyList[searchTile])
            {
                distanceMap.TryAdd(connectedTile, searchTileDistance + 1);

                if (searchedTilesSet.Contains(connectedTile) == false)
                {
                    searchQueue.Enqueue(connectedTile);
                }
            }
        }

        // From the end tile find the shortest path back to the start using the distance map
        DungeonNode nextTile = end;
        path.Add(end);

        while (nextTile != start)
        {
            List<DungeonNode> adjacentTiles = _adjacencyList[nextTile];

            int bestNextTileDistance = int.MaxValue;

            foreach (DungeonNode adjacentTile in adjacentTiles)
            {
                if (distanceMap[adjacentTile] >= bestNextTileDistance)
                    continue;

                nextTile = adjacentTile;
                bestNextTileDistance = distanceMap[adjacentTile];
            }

            path.Add(nextTile);
        }

        path.Reverse();

        return path;
    }

    public void DebugGraph(Vector3 offset)
    {
        foreach ((DungeonNode node, List<DungeonNode> connectedNodes) in _adjacencyList)
        {
            Debug.DrawRay(node.Position + offset, Vector3.up, Color.green, 5f);

            foreach (DungeonNode connectedNode in connectedNodes)
            {
                Debug.DrawLine(node.Position + offset, connectedNode.Position + offset, Color.red, 5f);
            }
        }
    }

}

public class DungeonNode
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