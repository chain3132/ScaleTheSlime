using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.BattleEncounter.Characters.Behaviors
{
    public abstract class EnemyBehavior : ScriptableObject
    {
        public abstract IReadOnlyList<EnemyAction> PlanTurn();
    }
}
