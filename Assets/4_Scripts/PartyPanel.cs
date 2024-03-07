using System.Collections;
using System.Collections.Generic;
using GoodHub.Core.Runtime;
using UnityEngine;

public class PartyPanel : SceneSingleton<PartyPanel>
{

    [SerializeField] private RectTransform _profilesContainer;
    [SerializeField] private GameObject _profileItem;

    private List<PartyProfile> _partyProfiles = new List<PartyProfile>();

    public void Initialise(List<CharacterEntity> partyCharacterEntities)
    {
        foreach (Transform childTransform in _profilesContainer)
        {
            Destroy(childTransform.gameObject);
        }

        foreach (CharacterEntity partyCharacterEntity in partyCharacterEntities)
        {
            PartyProfile characterProfile = Instantiate(_profileItem, _profilesContainer).GetComponent<PartyProfile>();
            _partyProfiles.Add(characterProfile);

            characterProfile.Initialise(partyCharacterEntity);
        }
    }

    public void SetActiveCharacter(int activeCharacterIndex)
    {
        foreach (PartyProfile partyProfile in _partyProfiles)
        {
            partyProfile.SetSelectedState(false);
        }

        _partyProfiles[activeCharacterIndex].SetSelectedState(true);
    }

}