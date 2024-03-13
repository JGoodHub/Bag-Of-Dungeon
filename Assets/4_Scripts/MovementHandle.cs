using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MovementHandle : MonoBehaviour, IPointerClickHandler
{

    [SerializeField] private SpriteRenderer _handleGraphic;
    [SerializeField] private Color _onColour;
    [SerializeField] private Color _offColour;

    private bool _interactable;
    private Action _callback;

    public void SetCallback(Action callback)
    {
        _callback = callback;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_interactable == false)
            return;

        _callback?.Invoke();
    }

    public void SetInteractable(bool isInteractable)
    {
        _interactable = isInteractable;

        _handleGraphic.color = _interactable ? _onColour : _offColour;
    }

}