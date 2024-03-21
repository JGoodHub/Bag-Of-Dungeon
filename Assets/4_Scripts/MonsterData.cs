using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "BagOfDungeon/Create Monster Data")]
public class MonsterData : ScriptableObject
{

    public string Class;
    public int MaxHeath;
    public int MaxCombatDice;
    public int MaxCombatModifier;

    [Space]
    public GameObject CharacterPrefab;
    public Sprite ProfilePicture;

}