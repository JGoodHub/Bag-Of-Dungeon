using System;
using System.Collections;
using System.Collections.Generic;
using GoodHub.Core.Runtime;
using UnityEngine;

public class PartyController : SceneSingleton<PartyController>
{

    [SerializeField] private Transform _charactersContainer;
    [SerializeField] private List<CharacterData> _partyCharacters;

    private List<CharacterEntity> _characterEntities = new List<CharacterEntity>();

    private int _activeCharacterIndex;

    public CharacterEntity ActiveCharacterEntity => _characterEntities[_activeCharacterIndex];

    private void Awake()
    {
        foreach (CharacterData partyCharacter in _partyCharacters)
        {
            _characterEntities.Add(new CharacterEntity(partyCharacter));
        }

        foreach (CharacterEntity characterInstance in _characterEntities)
        {
            GameObject characterObject = Instantiate(characterInstance.Data.CharacterPrefab, _charactersContainer);
            characterInstance.SetCharacterObject(characterObject);
        }
    }

    public void Initialise()
    {
        foreach (CharacterEntity characterEntity in _characterEntities)
        {
            characterEntity.CharacterObject.transform.position = Vector3.zero;
            characterEntity.CharacterObject.transform.rotation = Quaternion.Euler(0, 180, 0);

            characterEntity.Position = Vector3Int.zero;
        }

        CharacterCanvas.Singleton.SetSourceCharacter(ActiveCharacterEntity, true);

        PartyPanel.Singleton.Initialise(_characterEntities);
        PartyPanel.Singleton.SetActiveCharacter(_activeCharacterIndex);

        CameraController.Singleton.SetTrackingTarget(ActiveCharacterEntity.CharacterObject.transform);

        RevealAdjacentTilesToCharacters();

        RefreshMovementHandle();

        HandleStackedCharacters();
    }

    private void RevealAdjacentTilesToCharacters()
    {
        foreach (CharacterEntity characterEntity in _characterEntities)
        {
            DungeonController.Singleton.RevealConnectedTiles(characterEntity.Position);
        }
    }

    private bool IsCharacterOnEdge(CharacterEntity character)
    {
        foreach (Vector3Int adjacentPosition in DungeonUtils.GetAdjacentPositions(character.Position))
        {
            if (DungeonController.Singleton.IsTileReveled(adjacentPosition) == false)
            {
                return true;
            }
        }

        return false;
    }

    private void RefreshMovementHandle()
    {
        List<bool> walkableDirections = DungeonController.Singleton.GetConnections(ActiveCharacterEntity.Position);

        MovementControls.Singleton.Setup(ActiveCharacterEntity.Position, walkableDirections,
            ActiveCharacterEntity.CurrentActionPoints > 0);
    }

    public void MoveSelectedCharacter(MovementDirection movementDirection)
    {
        Vector3Int nextPosition = GetNextPosition(ActiveCharacterEntity.Position, movementDirection);

        ActiveCharacterEntity.HopToPosition(nextPosition);

        ActiveCharacterEntity.CurrentActionPoints--;

        CharacterCanvas.Singleton.RefreshFields();

        RevealAdjacentTilesToCharacters();

        RefreshMovementHandle();

        HandleStackedCharacters();
    }

    private void HandleStackedCharacters()
    {
        Dictionary<Vector3Int, List<CharacterEntity>> charactersByPosition = new Dictionary<Vector3Int, List<CharacterEntity>>();

        foreach (CharacterEntity characterEntity in _characterEntities)
        {
            if (charactersByPosition.TryGetValue(characterEntity.Position, out List<CharacterEntity> entitiesList))
            {
                entitiesList.Add(characterEntity);
            }
            else
            {
                charactersByPosition.Add(characterEntity.Position, new List<CharacterEntity> { characterEntity });
            }
        }

        foreach ((Vector3Int position, List<CharacterEntity> characters) in charactersByPosition)
        {
            if (characters.Count == 1)
            {
                characters[0].CharacterObject.SetActive(true);
                continue;
            }

            foreach (CharacterEntity character in characters)
            {
                character.CharacterObject.SetActive(false);
            }

            if (characters.Contains(ActiveCharacterEntity))
            {
                ActiveCharacterEntity.CharacterObject.SetActive(true);
            }
            else
            {
                characters[0].CharacterObject.SetActive(true);
            }
        }

        OverlayGraphicsController.Singleton.RefreshStackedCharacterOverlays(_characterEntities, ActiveCharacterEntity);
    }

    private Vector3Int GetNextPosition(Vector3Int position, MovementDirection direction)
    {
        switch (direction)
        {
            case MovementDirection.UP:
                return position + Vector3Int.forward;
            case MovementDirection.RIGHT:
                return position + Vector3Int.right;
            case MovementDirection.DOWN:
                return position + Vector3Int.back;
            case MovementDirection.LEFT:
                return position + Vector3Int.left;
            default:
                return position;
        }
    }

    public void IncrementActiveCharacter()
    {
        _activeCharacterIndex++;
        _activeCharacterIndex %= _characterEntities.Count;

        ActiveCharacterEntity.ResetActionPoints();

        CharacterCanvas.Singleton.SetSourceCharacter(ActiveCharacterEntity, true);

        PartyPanel.Singleton.SetActiveCharacter(_activeCharacterIndex);

        CameraController.Singleton.SetTrackingTarget(ActiveCharacterEntity.CharacterObject.transform);

        RefreshMovementHandle();
    }

}