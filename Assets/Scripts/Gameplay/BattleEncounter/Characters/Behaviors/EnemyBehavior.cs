using Cysharp.Threading.Tasks;
using Gameplay.BattleEncounter.Battle;
using UnityEngine;

namespace Gameplay.BattleEncounter.Characters.Behaviors
{
    public abstract class EnemyBehavior : ScriptableObject
    {
        public abstract UniTask Act(Enemy self, BattleContext ctx);
    }
}
