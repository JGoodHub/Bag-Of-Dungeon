using UnityEngine;

public class CharacterInstance
{

    private CharacterData _characterData;

    private int _currentLives;
    private int _currentHeath;
    private int _currentActionPoints;

    private GameObject _characterObject;

    public CharacterData CharacterData => _characterData;

    public int CurrentLives => _currentLives;

    public int CurrentHeath => _currentHeath;

    public int CurrentActionPoints => _currentActionPoints;

    public GameObject CharacterObject => _characterObject;

    public CharacterInstance(CharacterData characterData)
    {
        _characterData = characterData;

        _currentLives = characterData.MaxLives;
        _currentHeath = characterData.MaxHeath;
        _currentActionPoints = characterData.MaxActionPoints;
    }

    public void SetCharacterObject(GameObject characterObject)
    {
        _characterObject = characterObject;
    }

}