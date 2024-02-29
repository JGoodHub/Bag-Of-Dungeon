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

    public void SetupForCharacter(CharacterInstance characterInstance)
    {
        _classText.text = characterInstance.CharacterData.Class;

        _combatDiceText.text = $"{characterInstance.CharacterData.MaxCombatDice} Dice + {characterInstance.CharacterData.MaxCombatModifier}";

        _livesTracker.SetActiveCount(characterInstance.CharacterData.MaxLives);
        _healthTracker.SetActiveCount(characterInstance.CharacterData.MaxHeath);
        _actionPointsTracker.SetActiveCount(characterInstance.CharacterData.MaxActionPoints);

        _livesTracker.SetUncrossedCount(characterInstance.CurrentLives);
        _healthTracker.SetUncrossedCount(characterInstance.CurrentHeath);
        _actionPointsTracker.SetUncrossedCount(characterInstance.CurrentActionPoints);
    }

}