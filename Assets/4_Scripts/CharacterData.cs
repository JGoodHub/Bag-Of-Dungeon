using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "BagOfDungeon/Create Character Data")]
public class CharacterData : ScriptableObject
{

    public string Class;
    public int MaxLives;
    public int MaxHeath;
    public int MaxActionPoints;
    public int MaxCombatDice;
    public int MaxCombatModifier;
    [Space]
    public GameObject CharacterPrefab;
    public Sprite ProfilePicture;

}