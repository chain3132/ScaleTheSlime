using System;
using Gameplay.BattleEncounter.UI.Card;
using Gameplay.BattleEncounter.UI.Card.Enum;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
public class CardEffectData 
{
    public CardEffectType Type;
    public CardTarget Target;

    [Header("Value")]
    public ValueSource ValueSource;
    public int CardValue; 
    private bool UseCardValue => ValueSource == ValueSource.Custom;
    [ShowIf(nameof(UseCardValue))]
    [AllowNesting]
    public CustomValueSource CustomSource;
}
