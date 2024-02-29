using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovementHandle : MonoBehaviour, IPointerClickHandler
{

    private Action _callback;

    public void SetCallback(Action callback)
    {
        _callback = callback;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _callback?.Invoke();
    }

}