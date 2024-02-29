using System;
using System.Collections;
using System.Collections.Generic;
using GoodHub.Core.Runtime;
using UnityEngine;

public class PartyController : SceneSingleton<PartyController>
{

    [SerializeField] private Transform _charactersContainer;
    [SerializeField] private List<CharacterData> _partyCharacters;

    private List<CharacterInstance> _characterInstances = new List<CharacterInstance>();

    private void Awake()
    {
        foreach (CharacterData partyCharacter in _partyCharacters)
        {
            _characterInstances.Add(new CharacterInstance(partyCharacter));
        }

        foreach (CharacterInstance characterInstance in _characterInstances)
        {
            GameObject characterObject = Instantiate(characterInstance.CharacterData.CharacterPrefab, _charactersContainer);
            characterInstance.SetCharacterObject(characterObject);
        }
    }

    private void Start()
    {
        CharacterCanvas.Singleton.SetupForCharacter(_characterInstances[0]);
    }

    public void Initialise()
    {
        foreach (CharacterInstance characterInstance in _characterInstances)
        {
            characterInstance.CharacterObject.transform.position = Vector3.zero;
            characterInstance.CharacterObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    public void MoveSelectedCharacter(MovementDirection movementDirection) { }

}