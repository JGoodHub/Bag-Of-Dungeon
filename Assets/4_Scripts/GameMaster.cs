using System;
using GoodHub.Core.Runtime;
using UnityEngine;


public class GameMaster : SceneSingleton<GameMaster>
{

    [SerializeField] private int _seed;
    [SerializeField] private TileStack _tileStack;

    private DungeonGraph _dungeonGraph;
    private DungeonModel _dungeonModel;

    private void Start()
    {
        _seed = _seed < 0 ? DateTime.Now.GetHashCode() : _seed;

        _tileStack.Initialise(_seed);

        _dungeonGraph = DungeonGenerator.Singleton.GenerateDungeonGraph(_seed, _tileStack);

        _dungeonModel = new DungeonModel(_dungeonGraph);

        _dungeonModel.HideAllTileExceptStart();

        PartyController.Singleton.Initialise();
    }
    
}