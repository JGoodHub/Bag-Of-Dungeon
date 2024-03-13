using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class DungeonTile : MonoBehaviour
{

    [SerializeField] private DungeonTileType _tileType;
    [SerializeField] private Transform _animRoot;

    private DungeonNode _node;

    private bool _hidden;

    public DungeonNode Node => _node;

    public void Initialise(DungeonNode node)
    {
        _node = node;

        gameObject.name = $"DungeonTile_({_tileType} / {_node.Position.x}, {_node.Position.z})";

        transform.position = _node.Position;
        transform.rotation = Quaternion.Euler(0, 90 * _node.Rotation, 0);
    }

    public void Reveal()
    {
        if (_hidden == false)
            return;

        _animRoot.gameObject.SetActive(true);
        _animRoot.localScale = Vector3.zero;
        _animRoot.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBounce);

        _hidden = false;
    }

    public void Hide()
    {
        _animRoot.gameObject.SetActive(false);

        _hidden = true;
    }

}