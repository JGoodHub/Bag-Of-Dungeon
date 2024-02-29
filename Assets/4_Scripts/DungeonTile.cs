using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class DungeonTile : MonoBehaviour
{
    [SerializeField] private DungeonTileType _tileType;
    [SerializeField] private Transform _animRoot;

    private Vector3Int _position;
    private List<DungeonTile> _connectedTiles = new List<DungeonTile>();

    public Vector3Int Position => _position;

    public List<DungeonTile> ConnectedTiles => _connectedTiles;

    public DungeonTileType DungeonTileType => _tileType;

    public void Initialise(Vector3Int position)
    {
        _position = position;
        gameObject.name = $"DungeonTile_({_tileType} / {_position.x}, {_position.z})";
    }

    public void Reveal()
    {
        _animRoot.gameObject.SetActive(true);
        _animRoot.localScale = Vector3.zero;
        _animRoot.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);
    }

    public void Hide()
    {
        _animRoot.gameObject.SetActive(false);
    }
}