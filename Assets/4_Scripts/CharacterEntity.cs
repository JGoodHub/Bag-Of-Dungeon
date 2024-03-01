using DG.Tweening;
using UnityEngine;

public class CharacterEntity
{

    private GameObject _characterObject;

    public CharacterData Data { get; }

    public int CurrentLives { get; }

    public int CurrentHeath { get; }

    public int CurrentActionPoints { get; }

    public GameObject CharacterObject => _characterObject;

    public Vector3Int Position { get; set; }

    public CharacterEntity(CharacterData data)
    {
        Data = data;

        CurrentLives = data.MaxLives;
        CurrentHeath = data.MaxHeath;
        CurrentActionPoints = data.MaxActionPoints;
    }

    public void SetCharacterObject(GameObject characterObject)
    {
        _characterObject = characterObject;
    }

    public void HopToPosition(Vector3Int position)
    {
        Position = position;
        _characterObject.transform.DOJump(position, 0.75f, 1, 0.4f);
    }

}