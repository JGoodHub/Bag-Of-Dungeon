using System.Collections.Generic;
using UnityEngine;


public class DungeonModel
{

    private DungeonGraph _dungeonGraph;

    private List<DungeonTile> _dungeonTiles = new List<DungeonTile>();

    public DungeonModel(DungeonGraph dungeonGraph)
    {
        _dungeonGraph = dungeonGraph;

        GameObject dungeonInstanceObject = new GameObject("DungeonInstance");

        foreach (DungeonNode node in _dungeonGraph.Nodes)
        {
            GameObject tilePrefab = node.TileData.TilePrefab;
            DungeonTile dungeonTile = Object.Instantiate(tilePrefab, dungeonInstanceObject.transform).GetComponent<DungeonTile>();

            dungeonTile.Initialise(node);

            _dungeonTiles.Add(dungeonTile);
        }
    }

    public void HideAllTileExceptStart()
    {
        foreach (DungeonTile dungeonTile in _dungeonTiles)
        {
            if (dungeonTile.Node.TileData.TileType == DungeonTileType.Start)
                continue;

            dungeonTile.Hide();
        }
    }

    public void RevealTileAtPosition(Vector3Int position)
    {
        DungeonTile dungeonTile = _dungeonTiles.Find(tile => tile.Node.Position == position);

        if (dungeonTile == null)
            return;

        dungeonTile.Reveal();
    }

}