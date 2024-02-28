using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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