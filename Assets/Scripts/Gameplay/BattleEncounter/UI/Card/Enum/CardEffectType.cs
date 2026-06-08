using UnityEngine;

namespace Gameplay.BattleEncounter.UI.Card
{
    public enum CardEffectType 
    {
        Attack, 
        GainShield,
        Heal, 
        LoseHealth,        
        ChangeSize, 
        HalveSize,                      
        GainStatus,                                  
        DrawCard, 
        DiscardAllCard,
        Redraw,            
        AddCardToDiscardPile,
        AddCardToDrawPile,     
        PlayNextCardMultipleTimes,                    
        Stun,
        Flee, 
        Suicide,
        SummonEnemy,  
    }
}
