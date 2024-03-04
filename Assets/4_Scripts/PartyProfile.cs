using UnityEngine;
using UnityEngine.UI;


public class PartyProfile : MonoBehaviour
{

    [SerializeField] private Image _profileImage;
    [SerializeField] private GameObject _selectedGlow;
    [SerializeField] private GameObject _selectedArrow;

    public void Initialise(CharacterEntity characterEntity)
    {
        _profileImage.sprite = characterEntity.Data.ProfilePicture;

        SetSelectedState(false);
    }

    public void SetSelectedState(bool isSelected)
    {
        _selectedGlow.gameObject.SetActive(isSelected);
        _selectedArrow.gameObject.SetActive(isSelected);
    }

}