using System;
using System.Collections;
using System.Collections.Generic;
using GoodHub.Core.Runtime;
using UnityEngine;

public class StackCharactersOverlay : MonoBehaviour
{

    [SerializeField] private RectTransform _iconsContainer;
    [SerializeField] private GameObject _iconPrefab;

    private Vector3Int _trackedTile;

    public void Initialise(List<CharacterEntity> stackedCharacters, Vector3Int position)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        foreach (CharacterEntity stackedCharacter in stackedCharacters)
        {
            StackCharacterIcon stackCharacterIcon = Instantiate(_iconPrefab, _iconsContainer).GetComponent<StackCharacterIcon>();
            stackCharacterIcon.SetAvatar(stackedCharacter.Data.ProfilePicture);
        }

        _trackedTile = position;
    }

    private void Update()
    {
        ((RectTransform)transform).anchoredPosition = CameraHelper.Camera.WorldToScreenPoint(_trackedTile);
    }

}