using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class TileStack
{

    public int straightTileCount = 30;
    public int cornerTileCount = 15;
    public int junctionTileCount = 15;
    public int crossTileCount = 10;

    private List<DungeonTileType> _stack;
    private Random _random;

    public void Initialise(int seed)
    {
        _random = new Random(seed);

        _stack = new List<DungeonTileType>();

        for (int i = 0; i < straightTileCount; i++)
            _stack.Add(DungeonTileType.Straight);

        for (int i = 0; i < cornerTileCount; i++)
            _stack.Add(DungeonTileType.Corner);

        for (int i = 0; i < junctionTileCount; i++)
            _stack.Add(DungeonTileType.Junction);

        for (int i = 0; i < crossTileCount; i++)
            _stack.Add(DungeonTileType.Cross);
        
        // Shuffle the stack
        
        for (int i = 0; i < _stack.Count * 10; i++)
        {
            int indexA = _random.Next(0, _stack.Count);
            int indexB = _random.Next(0, _stack.Count);

            (_stack[indexA], _stack[indexB]) = (_stack[indexB], _stack[indexA]);
        }

        _stack.Insert(_random.Next(_stack.Count / 2, _stack.Count), DungeonTileType.End);
    }

    public DungeonTileType PeekTile()
    {
        if (_stack.Count == 0)
            throw new IndexOutOfRangeException();

        return _stack[0];
    }

    public DungeonTileType PopTile()
    {
        if (_stack.Count == 0)
            throw new IndexOutOfRangeException();

        DungeonTileType poppedTile = _stack[0];
        _stack.RemoveAt(0);

        return poppedTile;
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

        _stack.Add(PopTile());
    }

}