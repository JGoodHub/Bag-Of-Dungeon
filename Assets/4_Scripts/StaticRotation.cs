using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class StaticRotation : MonoBehaviour
{

    private void Update()
    {
        transform.rotation = Quaternion.identity;
    }

}