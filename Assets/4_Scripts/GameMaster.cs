using System;
using GoodHub.Core.Runtime;
using UnityEngine;
using Random = System.Random;


public class GameMaster : SceneSingleton<GameMaster>
{

    [SerializeField] private int _seed;

    private Random _random;

    public int Seed => _seed;

    public Random Random => _random;

    private void Awake()
    {
        _seed = _seed < 0 ? DateTime.Now.GetHashCode() : _seed;

        _random = new Random(_seed);
    }

    private void Start()
    {
        DungeonController.Singleton.Initialise();

        PartyController.Singleton.Initialise();
    }

}