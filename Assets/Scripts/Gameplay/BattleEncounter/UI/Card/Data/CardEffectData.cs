using System;
using Gameplay.BattleEncounter.UI.Card;
using Gameplay.BattleEncounter.UI.Card.Enum;
using UnityEngine;

[Serializable]
public class CardEffectData 
{
    public CardEffectType Type;
    public CardTarget Target;

    [Header("Value")]
    public ValueSource ValueSource;
    public int CardValue;                  
    public CustomValueSource CustomSource;
}
