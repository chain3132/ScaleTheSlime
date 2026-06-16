using System;
using UnityEngine;

namespace Gameplay.BattleEncounter.UI.Card.Enum
{
    [Flags]
    public enum CardKeyword 
    {
        None = 0,
        Exhaust = 1,
        Retain = 2, 
        Unplayable = 4,
        DestroyOnPlay = 8
    }
}
