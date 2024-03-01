using System;
using System.Collections;
using System.Collections.Generic;
using GoodHub.Core.Runtime;
using UnityEngine;

public class CameraController : SceneSingleton<CameraController>
{

    [SerializeField] private float _cameraTrackSpeed;
    [SerializeField] private bool _lockVertical;

    private Transform _trackingTarget;
    private Vector3 _velocity = Vector3.zero;

    public void SetTrackingTarget(Transform target)
    {
        _trackingTarget = target;
    }

    private void Update()
    {
        if (_trackingTarget == null)
            return;

        Vector3 position = Vector3.SmoothDamp(transform.position, _trackingTarget.position, ref _velocity, _cameraTrackSpeed);

        if (_lockVertical)
        {
            position.y = 0;
        }

        transform.position = position;
    }

}