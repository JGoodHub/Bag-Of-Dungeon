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
        CharacterCanvas.Singleton.SetupForCharacter(_characterEntities[0]);

        foreach (CharacterEntity characterEntity in _characterEntities)
        {
            characterEntity.CharacterObject.transform.position = Vector3.zero;
            characterEntity.CharacterObject.transform.rotation = Quaternion.Euler(0, 180, 0);

            characterEntity.Position = Vector3Int.zero;
        }

        RevealAdjacentTilesToCharacters();

        RefreshMovementHandle();

        CameraController.Singleton.SetTrackingTarget(_characterEntities[0].CharacterObject.transform);
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
        foreach (CharacterEntity characterEntity in _characterEntities)
        {
            if (IsCharacterOnEdge(characterEntity) == false)
                continue;

            List<bool> walkableDirections = DungeonController.Singleton.GetConnections(characterEntity.Position);

            MovementControls.Singleton.Setup(characterEntity.Position, walkableDirections);
        }
    }

    public void MoveSelectedCharacter(MovementDirection movementDirection)
    {
        Vector3Int nextPosition = GetNextPosition(_characterEntities[0].Position, movementDirection);

        _characterEntities[0].HopToPosition(nextPosition);

        RevealAdjacentTilesToCharacters();

        RefreshMovementHandle();
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

}