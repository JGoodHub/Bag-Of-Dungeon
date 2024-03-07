using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialLayoutGroup : MonoBehaviour
{

    [SerializeField] private float _cellDegrees;
    [SerializeField] private float _radius;

    private void OnTransformChildrenChanged()
    {
        RebuildLayout();
    }

    private void RebuildLayout()
    {
        if (transform.childCount == 0)
            return;

        List<RectTransform> activeChildren = new List<RectTransform>();

        foreach (RectTransform child in transform)
        {
            if (child.gameObject.activeSelf)
            {
                activeChildren.Add(child);
            }
        }

        float totalArc = ((activeChildren.Count - 1) * _cellDegrees);

        for (int i = 0; i < activeChildren.Count; i++)
        {
            activeChildren[i].rotation = Quaternion.Euler(0f, 0f, (totalArc / 2f) - (i * _cellDegrees));

            activeChildren[i].anchoredPosition = Vector2.zero + ((Vector2)activeChildren[i].up * _radius);
        }
    }

    private void OnDrawGizmos()
    {
        RebuildLayout();
    }

}
