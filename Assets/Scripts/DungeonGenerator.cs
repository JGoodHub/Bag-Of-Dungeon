using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;
using Random = UnityEngine.Random;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] private int _seed;
    [SerializeField] private AnimationCurve _compactnessCurve;
    [SerializeField] private TileBag _tileBag;
    [Space]
    [SerializeField] private Transform _tilesContainer;
    [SerializeField] private List<DungeonTileData> _tiles;

    private Dungeon _dungeon;

    private void Start()
    {
        _seed = _seed < 0 ? DateTime.Now.GetHashCode() : _seed;

        Random.InitState(_seed);

        _tileBag.Initialise();

        foreach (DungeonTileData tile in _tiles)
        {
            tile.Initialise();
        }

        _dungeon = GenerateDungeonInstanceFromBag(_tileBag);

        // List<DungeonTile> shortestPath = _dungeon.Pathfinding.GetShortestPath(_dungeon.GetStartTile(), _dungeon.GetRandomTile());
        //
        // for (int i = 0; i < shortestPath.Count - 1; i++)
        // {
        //     Debug.DrawLine(shortestPath[i].transform.position + Vector3.up, shortestPath[i + 1].transform.position + Vector3.up, Color.red, 50f);
        // }
    }

    private Dungeon GenerateDungeonInstanceFromBag(TileBag bag)
    {
        // Create the instance object

        GameObject dungeonInstanceObject = new GameObject("DungeonInstance");
        Dungeon dungeon = dungeonInstanceObject.AddComponent<Dungeon>();

        // Create the start tile

        GameObject startTilePrefab = GetPrefabForTileType(DungeonTileType.Start);
        DungeonTile startDungeonTile = Instantiate(startTilePrefab, Vector3.zero, Quaternion.identity, _tilesContainer).GetComponent<DungeonTile>();
        startDungeonTile.transform.rotation = Quaternion.Euler(0, -90, 0);

        List<Vector3Int> occupiedSpaces = new List<Vector3Int> {Vector3Int.zero};
        List<Vector3Int> availableSpaces = new List<Vector3Int>();

        availableSpaces.AddRange(startDungeonTile.GetConnectedSpaces(true));

        List<DungeonTile> tiles = new List<DungeonTile> {startDungeonTile};

        int loopCounter = 0;

        while (bag.IsEmpty() == false && loopCounter++ < 1000)
        {
            Dictionary<DungeonTile, List<Vector3Int>> connectionsList = new Dictionary<DungeonTile, List<Vector3Int>>();

            foreach (DungeonTile tile in tiles)
            {
                connectionsList.Add(tile, tile.GetConnectedSpaces(true));
            }

            DungeonTileType nextTileType = bag.PeekTile();
            DungeonTileData nextTileData = _tiles.Find(tile => tile.DungeonTileType == nextTileType);

            // Check if theres a valid space for the tile
            List<Vector3Int> validSpaces = new List<Vector3Int>();

            foreach (Vector3Int availableSpace in availableSpaces)
            {
                int spaceInputsCode = GetSpaceInputsCode(availableSpace, occupiedSpaces, connectionsList);

                for (int r = 0; r < 4; r++)
                {
                    if (spaceInputsCode == nextTileData.GetRotatedConnectionsCode(r))
                    {
                        validSpaces.Add(availableSpace);
                        break;
                    }
                }
            }

            // There are currently no valid spaces for this type of tile
            if (validSpaces.Count == 0)
                continue;

            // Find the most central available space (keeps the dungeon compact/circular)
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
            bag.DrawTile();
            GameObject tilePrefab = GetPrefabForTileType(nextTileType);
            DungeonTile dungeonTile = Instantiate(tilePrefab, Vector3.zero, Quaternion.identity, _tilesContainer).GetComponent<DungeonTile>();
            dungeonTile.Initialise(chosenSpace);

            // Move the tile to that space
            availableSpaces.Remove(chosenSpace);
            occupiedSpaces.Add(chosenSpace);

            dungeonTile.transform.position = chosenSpace;

            // Find the rotation that yields the most connected sides
            List<Vector3> allOtherConnectors = new List<Vector3>();

            foreach (DungeonTile tile in tiles)
            {
                allOtherConnectors.AddRange(tile.GetConnectorPositions(true));
            }

            int bestRotation = 0;
            int maxConnections = -1;

            for (int r = 0; r < 4; r++)
            {
                dungeonTile.transform.rotation = Quaternion.Euler(0, 90 * r, 0);

                List<Vector3> newTileConnectors = dungeonTile.GetConnectorPositions(true);
                int matchingConnectors = 0;

                foreach (Vector3 otherConnector in allOtherConnectors)
                    if (newTileConnectors.Exists(newTileConnector => (newTileConnector - otherConnector).sqrMagnitude < 0.01f))
                        matchingConnectors++;

                if (matchingConnectors <= maxConnections)
                    continue;

                bestRotation = r;
                maxConnections = matchingConnectors;

                if (matchingConnectors == newTileConnectors.Count)
                    break;
            }

            dungeonTile.transform.rotation = Quaternion.Euler(0, 90 * bestRotation, 0);

            tiles.Add(dungeonTile);

            // Add the newly available positions (that aren't already occupied) to the search area
            List<Vector3Int> potentialAvailablePositions = dungeonTile.GetConnectedSpaces(true);
            potentialAvailablePositions.RemoveAll(potentialPosition => occupiedSpaces.Contains(potentialPosition));
            potentialAvailablePositions.RemoveAll(potentialPosition => availableSpaces.Contains(potentialPosition));

            availableSpaces.AddRange(potentialAvailablePositions);
        }

        // Cap all the open corridors
        foreach (Vector3Int availableSpace in availableSpaces)
        {
            // Create the end cap object
            GameObject tilePrefab = GetPrefabForTileType(DungeonTileType.Cap);
            DungeonTile dungeonTile = Instantiate(tilePrefab, Vector3.zero, Quaternion.identity, _tilesContainer).GetComponent<DungeonTile>();

            occupiedSpaces.Add(availableSpace);

            dungeonTile.transform.position = availableSpace;

            dungeonTile.gameObject.name = $"DungeonTile_({DungeonTileType.Cap} / {availableSpace.x}, {availableSpace.z})";

            // Rotate the end cap to face it's matching tile
            List<Vector3> allOtherConnectors = new List<Vector3>();

            foreach (DungeonTile tile in tiles)
                allOtherConnectors.AddRange(tile.GetConnectorPositions(true));

            for (int r = 0; r < 4; r++)
            {
                dungeonTile.transform.rotation = Quaternion.Euler(0, 90 * r, 0);

                List<Vector3> newTileConnectors = dungeonTile.GetConnectorPositions(true);
                int matchingConnectors = 0;

                foreach (Vector3 otherConnector in allOtherConnectors)
                    if (newTileConnectors.Exists(newTileConnector => (newTileConnector - otherConnector).sqrMagnitude < 0.01f))
                        matchingConnectors++;

                if (matchingConnectors == 1)
                    break;
            }

            tiles.Add(dungeonTile);
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

    private GameObject GetPrefabForTileType(DungeonTileType tileType)
    {
        return _tiles.Find(tile => tile.DungeonTileType == tileType)?.TilePrefab;
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

    private int GetSpaceInputsCode(Vector3Int availableSpace, List<Vector3Int> occupiedSpaces, Dictionary<DungeonTile, List<Vector3Int>> connectionsList)
    {
        List<Vector3Int> positionsLeadingToSpace = new List<Vector3Int>();

        foreach ((DungeonTile tile, List<Vector3Int> connectedSpaces) in connectionsList)
        {
            if (connectedSpaces.Contains(availableSpace))
            {
                positionsLeadingToSpace.Add(tile.Position);
            }
        }

        byte inputCode = 0b0000_0000;

        foreach (Vector3Int adjacentPosition in GetAdjacentPositions(availableSpace))
        {
            if (occupiedSpaces.Contains(adjacentPosition) == false)
            {
                inputCode |= 0b000_0001;
                inputCode <<= 1;
                continue;
            }

            if (positionsLeadingToSpace.Contains(adjacentPosition))
            {
                inputCode |= 0b000_0001;
                inputCode <<= 1;
                continue;
            }
        }

        return inputCode;
    }
}