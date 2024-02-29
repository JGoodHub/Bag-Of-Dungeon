using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockCounter : MonoBehaviour
{

    [SerializeField] private Text _valueText;
    [SerializeField] private GameObject _cross;

    public void Initialise(int value)
    {
        _valueText.text = value.ToString();
        _cross.SetActive(false);
    }

    public void SetCrossStatus(bool isCrossed)
    {
        _cross.SetActive(isCrossed);
    }

}