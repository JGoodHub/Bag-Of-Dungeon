using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class AlwaysDownShadow : MonoBehaviour
{
    [SerializeField] private float _distance;

    private Shadow _shadow;

    private void Update()
    {
        _shadow ??= GetComponent<Shadow>();

        if (_shadow == null)
            return;

        Vector2 shadowDirection = new Vector2(Vector2.Dot(transform.right, Vector2.up), Vector2.Dot(transform.up, Vector2.up));

        _shadow.effectDistance = shadowDirection * _distance;
    }


}
