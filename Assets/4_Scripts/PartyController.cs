using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyController : MonoBehaviour
{

    [SerializeField] private List<CharacterData> _partyCharacters;

    private List<CharacterInstance> _characterInstances = new List<CharacterInstance>();


    private void Start()
    {
        foreach (CharacterData partyCharacter in _partyCharacters)
        {
            _characterInstances.Add(new CharacterInstance(partyCharacter));
        }

        CharacterCanvas.Singleton.SetupForCharacter(_characterInstances[0]);
    }

}