using System;
using Gameplay.BattleEncounter.Status;
using Gameplay.BattleEncounter.UI.Card;
using Gameplay.BattleEncounter.UI.Card.Enum;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
public class CardEffectData 
{
    public CardEffectType Type;
    [AllowNesting]
    [ShowIf(nameof(IsGainStatus))]
    public StatusType StatusType;
    [AllowNesting]
    [ShowIf(nameof(IsGainStatus))]
    public int StatusAmount;
    public CardTarget Target;
    [AllowNesting]
    [ShowIf(nameof(IsAddCard))]
    public CardDefinition CardToAdd;
    

    [Header("Value")]
    [AllowNesting]
    [HideIf(nameof(IsGainStatus))]
    public ValueSource ValueSource;
    [AllowNesting]
    [HideIf(nameof(IsGainStatus))]
    public int CardValue;
    [Min(1)]
    public int Repeat = 1;
    [AllowNesting]
    [ShowIf(nameof(UseCardValue))]
    public CustomValueSource CustomSource;
    
    private bool UseCardValue()
    {
        return ValueSource == ValueSource.Custom;
    }
    private bool IsGainStatus() 
    {
        return Type == CardEffectType.GainStatus;
    }
    private bool IsAddCard()
    {
        return Type == CardEffectType.AddCardToDrawPile || Type == CardEffectType.AddCardToDiscardPile;
    }
}
