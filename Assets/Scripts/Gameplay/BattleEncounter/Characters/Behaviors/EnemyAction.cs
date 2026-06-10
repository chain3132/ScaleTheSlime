using System;
using Gameplay.BattleEncounter.Status;
using NaughtyAttributes;
using UnityEngine;

namespace Gameplay.BattleEncounter.Characters.Behaviors
{
    [Serializable]
    public class EnemyAction
    {
        public EnemyActionType Type;
        public int Value = 5;

        [Min(1)]
        public int Repeat = 1;  

        private bool UseStatus => Type == EnemyActionType.BuffSelf || Type == EnemyActionType.DebuffPlayer;
        [ShowIf(nameof(UseStatus))]
        [AllowNesting]
        public StatusType Status;
    }
}
