using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class DungeonTile : MonoBehaviour
{
    [SerializeField] private DungeonTileType _tileType;
    [SerializeField] private List<Vector3> _connectors = new();
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

    public List<Vector3> GetConnectorPositions(bool worldPosition)
    {
        if (worldPosition)
            return _connectors.Select(connector => transform.TransformPoint(connector)).ToList();

        return _connectors;
    }

    public List<Vector3Int> GetConnectedSpaces(bool worldPosition)
    {
        List<Vector3> connectors = GetConnectorPositions(false);

        if (worldPosition)
            return connectors.Select(connector => Vector3Int.RoundToInt(transform.TransformPoint(connector * 2f))).ToList();

        return connectors.Select(connector => Vector3Int.RoundToInt(connector * 2f)).ToList();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        foreach (Vector3 connector in GetConnectorPositions(true))
        {
            Gizmos.DrawSphere(connector, 0.05f);
        }

        Gizmos.color = Color.green;

        foreach (Vector3 connector in GetConnectedSpaces(true))
        {
            Gizmos.DrawSphere(connector, 0.1f);
        }
    }
}