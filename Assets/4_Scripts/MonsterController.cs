using System.Collections;
using System.Collections.Generic;
using GoodHub.Core.Runtime;
using UnityEngine;

public class MonsterController : SceneSingleton<MonsterController>
{

    [SerializeField] private GameObject _monsterDetectedEffect;
    [SerializeField] private Transform _monsterContainer;

    private Dictionary<Vector3Int, GameObject> _detectedMonsterEffects = new Dictionary<Vector3Int, GameObject>();
    
    public void RefreshDetectedMonsters()
    {
        foreach (DungeonTile revealedTile in DungeonController.Singleton.RevealedTiles)
        {
            if (revealedTile.Node.MetaData.IsMonsterTile == false || revealedTile.Node.StateData.MonsterDetected)
                continue;
            
            // Create a monster glow effect

            revealedTile.Node.StateData.MonsterDetected = true;

            GameObject monsterDetectedEffect = Instantiate(_monsterDetectedEffect, _monsterContainer);
            monsterDetectedEffect.transform.localPosition = revealedTile.Node.Position;

            _detectedMonsterEffects.Add(revealedTile.Node.Position, _monsterDetectedEffect);
        }
    }

}