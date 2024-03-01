using System;
using GoodHub.Core.Runtime;
using UnityEngine;


public class GameMaster : SceneSingleton<GameMaster>
{

    [SerializeField] private int _seed;

    public static int Seed => Singleton._seed;

    private void Awake()
    {
        _seed = _seed < 0 ? DateTime.Now.GetHashCode() : _seed;
    }

    private void Start()
    {
        DungeonController.Singleton.Initialise();

        PartyController.Singleton.Initialise();
    }

}