using Gameplay.BattleEncounter.Characters;
using Gameplay.BattleEncounter.Status;
using Gameplay.BattleEncounter.UI.Card;
using Gameplay.BattleEncounter.UI.Card.Data;
using Gameplay.BattleEncounter.UI.Card.Enum;
using UnityEngine;

namespace Gameplay.BattleEncounter.Battle
{
    public class CardEffectExecutor
    {
        private readonly BattleContext _ctx;

        public CardEffectExecutor(BattleContext ctx) => _ctx = ctx;

        public void Execute(Card card, Enemy selectedEnemy)
        {
            foreach (var effect in card.Definition.Effects)
            {
                var target = ResolveTarget(effect.Target, selectedEnemy);
                int value = ResolveValue(effect, target);
                Apply(effect.Type, target, value);
            }
        }

        private Character ResolveTarget(CardTarget t, Enemy selectedEnemy)
        {
            switch (t)
            {
                case CardTarget.Player:
                    return _ctx.Player;
                case CardTarget.Enemy:
                    return selectedEnemy;
                default:
                    return null;
            }
        }

        private int ResolveValue(CardEffectData e, Character target) 
        {
            switch (e.ValueSource)
            {
                case ValueSource.Card:
                    return e.CardValue;
                case ValueSource.Custom:
                    return e.CustomSource switch
                    {
                        CustomValueSource.AliveEnemyCount => _ctx.AliveEnemyCount,
                        CustomValueSource.TargetSize => target?.Size.Value ?? 0,
                        CustomValueSource.SelfSize => _ctx.Player.Size.Value,
                        CustomValueSource.NumberOfCardsDiscardedByThisCard => 0, 
                        _ => 0,
                    };
                default:
                    return 0;
            }
        }

        private void Apply(CardEffectType type, Character target, int value)
        {
            switch (type)
            {
                case CardEffectType.Attack:     
                    target?.TakeDamage(value); 
                    break;
                case CardEffectType.LoseHealth: 
                    target?.TakeDamage(value); 
                    break; 
                case CardEffectType.GainShield: 
                    target?.GainShield(value); 
                    break;
                case CardEffectType.Heal:       
                    target?.Heal(value); 
                    break;
                case CardEffectType.ChangeSize: 
                    target?.ChangeSize(value); 
                    break;
                case CardEffectType.HalveSize:
                    if (target != null) target.ChangeSize(-(target.Size.Value / 2));
                    break;
                case CardEffectType.Stun:       
                    target?.ApplyStatus(StatusType.Stun, Mathf.Max(1, value)); 
                    break;


                default: break;
            }
        }
    }
}
