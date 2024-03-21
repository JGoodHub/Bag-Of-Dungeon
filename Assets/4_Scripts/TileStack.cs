using System;
using System.Collections;
using System.Collections.Generic;
using GoodHub.Core.Runtime.Utils;
using UnityEngine;
using Random = System.Random;

[Serializable]
public class TileStack
{

    public class MetaData
    {

        public DungeonTileType Type;
        public bool IsMonsterTile;

    }

    public int straightTileCount = 30;
    public int cornerTileCount = 15;
    public int junctionTileCount = 15;
    public int crossTileCount = 10;
    [Space]
    public int clearTilesCount = 5;
    public int monsterTiles = 12;

    private List<MetaData> _stack;
    private Random _random;

    public static MetaData StartTileMetaData => new MetaData
    {
        Type = DungeonTileType.Start,
        IsMonsterTile = false
    };

    public int TotalTiles => straightTileCount + cornerTileCount + junctionTileCount + crossTileCount;

    public void Initialise(int seed)
    {
        if (monsterTiles + clearTilesCount > TotalTiles)
        {
            Debug.LogError("There can't be more monster tiles than tiles themselves");
            return;
        }

        _random = new Random(seed);

        _stack = new List<MetaData>();

        for (int i = 0; i < straightTileCount; i++)
        {
            _stack.Add(new MetaData
            {
                Type = DungeonTileType.Straight
            });
        }

        for (int i = 0; i < cornerTileCount; i++)
        {
            _stack.Add(new MetaData
            {
                Type = DungeonTileType.Corner
            });
        }

        for (int i = 0; i < junctionTileCount; i++)
        {
            _stack.Add(new MetaData
            {
                Type = DungeonTileType.Junction
            });
        }

        for (int i = 0; i < crossTileCount; i++)
        {
            _stack.Add(new MetaData
            {
                Type = DungeonTileType.Cross
            });
        }

        // Insert the monster tiles

        int monsterTileRemaining = Mathf.Clamp(monsterTiles, 0, TotalTiles);

        while (monsterTileRemaining > 0)
        {
            int monsterTileIndex = _random.Next(_stack.Count);

            if (_stack[monsterTileIndex].IsMonsterTile)
                continue;

            _stack[monsterTileIndex].IsMonsterTile = true;
            monsterTileRemaining--;
        }

        // Shuffle the stack

        for (int i = 0; i < _stack.Count * 10; i++)
        {
            _random.NextSwapIndices(_stack.Count, out int indexA, out int indexB);
            (_stack[indexA], _stack[indexB]) = (_stack[indexB], _stack[indexA]);
        }

        // Clear any monster tiles that are to early

        for (int i = 0; i < clearTilesCount; i++)
        {
            if (_stack[i].IsMonsterTile == false)
                continue;

            int newIndex = _random.Next(clearTilesCount, _stack.Count);

            while (_stack[newIndex].IsMonsterTile)
            {
                newIndex = _random.Next(clearTilesCount, _stack.Count);
            }

            (_stack[i], _stack[newIndex]) = (_stack[newIndex], _stack[i]);
        }

        // Add the end tile
        // George suggestion, convert a cap instead so that it's less chance of being next to the start

        _stack.Insert(_random.Next(_stack.Count / 2, _stack.Count), new MetaData
        {
            Type = DungeonTileType.End
        });
    }

    public MetaData Peek()
    {
        if (_stack.Count == 0)
            throw new IndexOutOfRangeException();

        return _stack[0];
    }

    public MetaData Pop()
    {
        if (_stack.Count == 0)
            throw new IndexOutOfRangeException();

        MetaData poppedMeta = _stack[0];
        _stack.RemoveAt(0);

        return poppedMeta;
    }

    public bool IsEmpty()
    {
        return _stack.Count == 0;
    }

    /// <summary>
    /// Pop the current top tile and re-add it to the bottom of the stack
    /// </summary>
    public void PopToBottom()
    {
        if (_stack.Count <= 1)
            return;

        _stack.Add(Pop());
    }

}