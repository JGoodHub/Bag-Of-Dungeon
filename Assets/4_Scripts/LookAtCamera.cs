using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LookAtCamera : MonoBehaviour
{

    private Camera _camera;

    void Update()
    {
        _camera ??= Camera.main;

        if (_camera == null)
            return;

        transform.LookAt(_camera.transform.position, Vector3.up);
    }

}