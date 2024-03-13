using System;
using System.Collections;
using System.Collections.Generic;
using GoodHub.Core.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCanvas : SceneSingleton<CharacterCanvas>
{

    [Header("Character Elements")]
    [SerializeField] private Text _classText;
    [SerializeField] private Text _combatDiceText;
    [SerializeField] private BlockTracker _livesTracker;
    [SerializeField] private BlockTracker _healthTracker;
    [SerializeField] private BlockTracker _actionPointsTracker;

    [Header("Controls Bar")]
    [SerializeField] private Button _endTurnButton;

    private CharacterEntity _characterEntity;

    private void Awake()
    {
        _endTurnButton.onClick.AddListener(EndTurnClicked);
    }

    private void EndTurnClicked()
    {
        PartyController.Singleton.IncrementActiveCharacter();
    }

    public void SetSourceCharacter(CharacterEntity characterEntity, bool displayImmediately)
    {
        _characterEntity = characterEntity;

        if (displayImmediately)
        {
            RefreshFields();
        }
    }

    public void RefreshFields()
    {
        _classText.text = _characterEntity.Data.Class;

        _combatDiceText.text = $"{_characterEntity.Data.MaxCombatDice} Dice + {_characterEntity.Data.MaxCombatModifier}";

        _livesTracker.SetActiveCount(_characterEntity.Data.MaxLives);
        _healthTracker.SetActiveCount(_characterEntity.Data.MaxHeath);
        _actionPointsTracker.SetActiveCount(_characterEntity.Data.MaxActionPoints);

        _livesTracker.SetUncrossedCount(_characterEntity.CurrentLives);
        _healthTracker.SetUncrossedCount(_characterEntity.CurrentHeath);
        _actionPointsTracker.SetUncrossedCount(_characterEntity.CurrentActionPoints);
    }

}