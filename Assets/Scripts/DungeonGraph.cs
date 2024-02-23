using System;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGraph
{
    private List<DungeonNode> _nodes;
    private Dictionary<DungeonNode, List<DungeonNode>> _adjacencyList;

    public List<DungeonNode> Nodes => _nodes;

    public DungeonGraph()
    {
        _nodes = new List<DungeonNode>();
        _adjacencyList = new Dictionary<DungeonNode, List<DungeonNode>>();
    }

    public void AddNode(DungeonNode node)
    {
        _nodes.Add(node);
        _adjacencyList.Add(node, new List<DungeonNode>());
    }

    public void AddEdge(DungeonNode node, DungeonNode adjacentNode)
    {
        if (_nodes.Contains(node) == false || _nodes.Contains(adjacentNode) == false)
        {
            throw new ArgumentException("One or both nodes are not in the dungeon.");
        }

        if (_adjacencyList.ContainsKey(node) == false)
        {
            throw new ArgumentException("Node is not in the adjacency list.");
        }

        _adjacencyList[node].Add(adjacentNode);
    }
}

public class DungeonNode
{
    private Vector2Int _position;
    private DungeonTileType _tileType;

    public Vector2Int Position => _position;

    public DungeonNode(DungeonTileType tileType, Vector2Int position)
    {
        _position = position;
        _tileType = tileType;
    }
}