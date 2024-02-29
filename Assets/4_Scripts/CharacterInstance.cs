public class CharacterInstance
{

    private CharacterData _characterData;

    private int _currentLives;
    private int _currentHeath;
    private int _currentActionPoints;

    public CharacterData CharacterData => _characterData;

    public int CurrentLives => _currentLives;

    public int CurrentHeath => _currentHeath;

    public int CurrentActionPoints => _currentActionPoints;

    public CharacterInstance(CharacterData characterData)
    {
        _characterData = characterData;

        _currentLives = characterData.MaxLives;
        _currentHeath = characterData.MaxHeath;
        _currentActionPoints = characterData.MaxActionPoints;
    }

}