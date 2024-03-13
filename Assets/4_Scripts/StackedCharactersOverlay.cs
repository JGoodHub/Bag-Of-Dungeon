using System;
using System.Collections;
using System.Collections.Generic;
using GoodHub.Core.Runtime;
using UnityEngine;

public class StackedCharactersOverlay : MonoBehaviour
{

    [SerializeField] private RadialLayoutGroup _iconsContainer;
    [SerializeField] private GameObject _iconPrefab;

    private Vector3Int _trackedTile;

    public Vector3Int TrackedTile => _trackedTile;

    public void Initialise(List<CharacterEntity> stackedCharacters, Vector3Int position)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        foreach (CharacterEntity stackedCharacter in stackedCharacters)
        {
            StackCharacterIcon stackCharacterIcon = Instantiate(_iconPrefab, _iconsContainer.transform).GetComponent<StackCharacterIcon>();
            stackCharacterIcon.SetAvatar(stackedCharacter.Data.ProfilePicture);
        }
        
        _iconsContainer.RebuildLayout();

        _trackedTile = position;
    }

    private void Update()
    {
        ((RectTransform)transform).anchoredPosition = CameraHelper.Camera.WorldToScreenPoint(_trackedTile);
    }

}