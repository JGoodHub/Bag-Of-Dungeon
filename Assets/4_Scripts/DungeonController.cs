using System.Collections.Generic;
using GoodHub.Core.Runtime;
using UnityEngine;


public class DungeonController : SceneSingleton<DungeonController>
{

    [SerializeField] private TileStack _tileStack;

    private DungeonGraph _dungeonGraph;

    private DungeonTile _startTile;
    private DungeonTile _endTile;

    private HashSet<DungeonTile> _revealedTiles = new HashSet<DungeonTile>();

    private Dictionary<Vector3Int, DungeonTile> _tilesByPosition = new Dictionary<Vector3Int, DungeonTile>();

    public HashSet<DungeonTile> RevealedTiles => _revealedTiles;

    public void Initialise()
    {
        _tileStack.Initialise(GameMaster.Singleton.Seed);

        _dungeonGraph = DungeonGenerator.Singleton.GenerateDungeonGraph(GameMaster.Singleton.Seed, _tileStack);

        GameObject dungeonInstanceObject = new GameObject("DungeonInstance");

        foreach (DungeonNode node in _dungeonGraph.Nodes)
        {
            GameObject tilePrefab = node.VisualData.TilePrefab;
            DungeonTile dungeonTile = Instantiate(tilePrefab, dungeonInstanceObject.transform).GetComponent<DungeonTile>();

            dungeonTile.Initialise(node);

            _tilesByPosition.Add(node.Position, dungeonTile);

            if (node.VisualData.TileType == DungeonTileType.Start)
            {
                _startTile = dungeonTile;
            }

            if (node.VisualData.TileType == DungeonTileType.End)
            {
                _endTile = dungeonTile;
            }
        }

        HideAllTileExceptStart();
    }

    public DungeonTile GetTileAtPosition(Vector3Int position)
    {
        return _tilesByPosition.GetValueOrDefault(position);
    }

    public void HideAllTileExceptStart()
    {
        foreach (DungeonTile dungeonTile in _tilesByPosition.Values)
        {
            if (dungeonTile.Node.VisualData.TileType == DungeonTileType.Start)
            {
                dungeonTile.Reveal();
                continue;
            }

            dungeonTile.Hide();
        }

        _revealedTiles.Clear();
        _revealedTiles.Add(_startTile);
    }

    public void RevealConnectedTiles(Vector3Int position)
    {
        if (_dungeonGraph.NodesByPosition.TryGetValue(position, out DungeonNode node) == false)
            return;

        List<DungeonNode> adjacentNodes = _dungeonGraph.AdjacencyList[node];

        foreach (DungeonNode adjacentNode in adjacentNodes)
        {
            DungeonTile adjacentTile = _tilesByPosition[adjacentNode.Position];

            if (_revealedTiles.Contains(adjacentTile))
                continue;

            adjacentTile.Reveal();
            _revealedTiles.Add(adjacentTile);
        }
    }

    public List<bool> GetConnections(Vector3Int position)
    {
        if (_dungeonGraph.NodesByPosition.TryGetValue(position, out DungeonNode node) == false)
            return new List<bool>();

        return node.GetConnections();
    }

    public bool IsTileReveled(Vector3Int position)
    {
        if (_tilesByPosition.TryGetValue(position, out DungeonTile tile) == false)
            return false;

        if (_revealedTiles.Contains(tile))
            return true;

        return false;
    }

    public List<bool> GetHiddenDirections(Vector3Int position)
    {
        List<bool> hiddenDirections = new List<bool>();

        foreach (Vector3Int adjacentPosition in DungeonUtils.GetAdjacentPositions(position))
        {
            hiddenDirections.Add(IsTileReveled(adjacentPosition) == false);
        }

        return hiddenDirections;
    }

}