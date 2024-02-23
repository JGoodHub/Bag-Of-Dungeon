using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class TileBag
{
    
    public int cornerTileCount;
    public int straightTileCount;
    public int crossTileCount;

    private List<DungeonTileType> _bag;

    public void Initialise()
    {
        _bag = new List<DungeonTileType>();

        for (int i = 0; i < cornerTileCount; i++)
            _bag.Add(DungeonTileType.Corner);

        for (int i = 0; i < straightTileCount; i++)
            _bag.Add(DungeonTileType.Straight);

        for (int i = 0; i < crossTileCount; i++)
            _bag.Add(DungeonTileType.Cross);


        for (int i = 0; i < _bag.Count * 10; i++)
        {
            int indexA = Random.Range(0, _bag.Count);
            int indexB = Random.Range(0, _bag.Count);

            (_bag[indexA], _bag[indexB]) = (_bag[indexB], _bag[indexA]);
        }

        _bag.Insert(Random.Range(_bag.Count / 2, _bag.Count), DungeonTileType.End);
    }

    public DungeonTileType DrawTile()
    {
        if (_bag.Count == 0)
            throw new IndexOutOfRangeException();

        DungeonTileType drawnTile = _bag[0];
        _bag.RemoveAt(0);
        return drawnTile;
    }

    public DungeonTileType PeekTile()
    {
        if (_bag.Count == 0)
            throw new IndexOutOfRangeException();

        return _bag[0];
    }

    public bool IsEmpty()
    {
        return _bag.Count == 0;
    }
}