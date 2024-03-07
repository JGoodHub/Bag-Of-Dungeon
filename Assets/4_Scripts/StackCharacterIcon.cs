using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StackCharacterIcon : MonoBehaviour
{

    [SerializeField] private Image _characterAvatar;

    public void SetAvatar(Sprite avatarSprite)
    {
        _characterAvatar.sprite = avatarSprite;
    }

}