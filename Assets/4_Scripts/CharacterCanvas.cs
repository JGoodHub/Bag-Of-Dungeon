using System.Collections;
using System.Collections.Generic;
using GoodHub.Core.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCanvas : SceneSingleton<CharacterCanvas>
{

    [SerializeField] private Text _classText;

    [SerializeField] private Text _combatDiceText;

    [SerializeField] private BlockTracker _livesTracker;
    [SerializeField] private BlockTracker _healthTracker;
    [SerializeField] private BlockTracker _actionPointsTracker;

    public void SetupForCharacter(CharacterEntity characterEntity)
    {
        _classText.text = characterEntity.Data.Class;

        _combatDiceText.text = $"{characterEntity.Data.MaxCombatDice} Dice + {characterEntity.Data.MaxCombatModifier}";

        _livesTracker.SetActiveCount(characterEntity.Data.MaxLives);
        _healthTracker.SetActiveCount(characterEntity.Data.MaxHeath);
        _actionPointsTracker.SetActiveCount(characterEntity.Data.MaxActionPoints);

        _livesTracker.SetUncrossedCount(characterEntity.CurrentLives);
        _healthTracker.SetUncrossedCount(characterEntity.CurrentHeath);
        _actionPointsTracker.SetUncrossedCount(characterEntity.CurrentActionPoints);
    }

}