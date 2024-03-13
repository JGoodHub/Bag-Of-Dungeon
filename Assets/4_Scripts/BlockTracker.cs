using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockTracker : MonoBehaviour
{

    [SerializeField] private List<BlockCounter> _blockCounters;

    private void Reset()
    {
        _blockCounters = GetComponentsInChildren<BlockCounter>().ToList();
    }

    private void Awake()
    {
        for (int i = 0; i < _blockCounters.Count; i++)
        {
            _blockCounters[i].Initialise(i + 1);
        }
    }

    public void SetActiveCount(int count)
    {
        for (int i = 0; i < _blockCounters.Count; i++)
        {
            _blockCounters[i].gameObject.SetActive(i < count);
        }
    }

    public void SetUncrossedCount(int count)
    {
        for (int i = 0; i < _blockCounters.Count; i++)
        {
            _blockCounters[i].SetCrossStatus(i >= count);
        }
    }

}