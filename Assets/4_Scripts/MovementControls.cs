using System;
using System.Collections;
using System.Collections.Generic;
using GoodHub.Core.Runtime;
using UnityEngine;

public class MovementControls : SceneSingleton<MovementControls>
{

    [SerializeField] private MovementHandle _upHandle;
    [SerializeField] private MovementHandle _rightHandle;
    [SerializeField] private MovementHandle _downHandle;
    [SerializeField] private MovementHandle _leftHandle;

    private void Awake()
    {
        _upHandle.SetCallback(HandleUpCallback);
        _rightHandle.SetCallback(HandleRightCallback);
        _downHandle.SetCallback(HandleDownCallback);
        _leftHandle.SetCallback(HandleLeftCallback);

        SetActiveHandles(false, false, false, false);
    }

    public void SetActiveHandles(bool up, bool right, bool down, bool left)
    {
        _upHandle.gameObject.SetActive(up);
        _rightHandle.gameObject.SetActive(right);
        _downHandle.gameObject.SetActive(down);
        _leftHandle.gameObject.SetActive(left);
    }

    private void HandleUpCallback()
    {
        PartyController.Singleton.MoveSelectedCharacter(MovementDirection.UP);
    }

    private void HandleRightCallback()
    {
        PartyController.Singleton.MoveSelectedCharacter(MovementDirection.RIGHT);
    }

    private void HandleDownCallback()
    {
        PartyController.Singleton.MoveSelectedCharacter(MovementDirection.DOWN);
    }

    private void HandleLeftCallback()
    {
        PartyController.Singleton.MoveSelectedCharacter(MovementDirection.LEFT);
    }

}

public enum MovementDirection
{

    UP,
    RIGHT,
    DOWN,
    LEFT

}