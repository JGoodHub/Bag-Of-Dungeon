using System;
using System.Collections;
using System.Collections.Generic;
using GoodHub.Core.Runtime;
using UnityEngine;

public class MovementControls : SceneSingleton<MovementControls>
{

    [SerializeField] private List<MovementHandle> _handles;

    private void Awake()
    {
        for (int handleIndex = 0; handleIndex < _handles.Count; handleIndex++)
        {
            MovementDirection movementDirection = (MovementDirection)handleIndex;
            _handles[handleIndex].SetCallback(() =>
            {
                HandleDirectionCallback(movementDirection);
            });
        }

        Setup(Vector3Int.zero, new List<bool> { false, false, false, false });
    }

    public void Setup(Vector3Int position, List<bool> arrowStates)
    {
        transform.position = position;

        for (int i = 0; i < _handles.Count; i++)
        {
            _handles[i].gameObject.SetActive(arrowStates[i]);
        }
    }

    private void HandleDirectionCallback(MovementDirection direction)
    {
        PartyController.Singleton.MoveSelectedCharacter(direction);
    }

}

public enum MovementDirection
{

    UP,
    RIGHT,
    DOWN,
    LEFT

}