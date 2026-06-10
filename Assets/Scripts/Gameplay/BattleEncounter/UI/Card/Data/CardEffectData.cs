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

    [AllowNesting]
    [ShowIf(nameof(UseCardValue))]
    public CustomValueSource CustomSource;
    
    private bool UseCardValue()
    {
        return ValueSource == ValueSource.Custom;
    }
}
