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
        private int _discardedByThisCard;
        private int _nextCardPlays = 1;

        public CardEffectExecutor(BattleContext ctx) => _ctx = ctx;

        public void Execute(Card card, Enemy selectedEnemy)
        {
            _discardedByThisCard = 0;
            int plays = _nextCardPlays;
            _nextCardPlays = 1;
            for (int p = 0; p < plays; p++)
            {
                foreach (var effect in card.Definition.Effects)
                {

                    if (effect.Target == CardTarget.AllEnemies)
                    {
                        foreach (var enemy in _ctx.Enemies)
                        {
                            if (enemy == null || enemy.IsDead) continue;
                            Apply(effect, enemy, ResolveValue(effect, _ctx, enemy, _discardedByThisCard), card);
                        }
                    }
                    else //single target
                    {
                        int times = Mathf.Max(1, effect.Repeat);
                        for (int i = 0; i < times; i++)
                        {
                            var target = ResolveTarget(effect.Target, selectedEnemy);
                            int value = ResolveValue(effect, _ctx, target, _discardedByThisCard);
                            Apply(effect, target, value, card);
                        }
                    }
                }
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

        
        public static int ResolveValue(CardEffectData e, BattleContext ctx, Character target,
            int discardedByThisCard = 0)
        {
            if (e.Type == CardEffectType.GainStatus)
            {
                return e.StatusAmount;
            }
            switch (e.ValueSource)
            {
                case ValueSource.Card:
                    return e.CardValue;
                case ValueSource.Custom:
                    return e.CustomSource switch
                    {
                        CustomValueSource.AliveEnemyCount => ctx.AliveEnemyCount,
                        CustomValueSource.TargetSize => target?.Size.Value ?? 0,
                        CustomValueSource.SelfSize => ctx.Player.Size.Value * e.CardValue,
                        CustomValueSource.NumberOfCardsDiscardedByThisCard => discardedByThisCard,
                        _ => 0,
                    };
                default:
                    return 0;
            }
        }

        
        public static int PreviewDisplay(CardEffectData e, BattleContext ctx, Character target)
        {
            int raw = ResolveValue(e, ctx, target);
            return e.Type switch
            {
                CardEffectType.Attack     => ctx.Player.PreviewOutgoingDamage(raw, target),
                CardEffectType.GainShield => raw + ctx.Player.Statuses.Get(StatusType.ShieldBonus),
                _ => raw,
            };
        }

        private void Apply(CardEffectData e, Character target, int value,Card playedCard)
        {
            switch (e.Type)
            {
                
                case CardEffectType.Attack:     
                    target?.TakeDamage(value, _ctx.Player); 
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
                case CardEffectType.SetSize:    
                    target?.SetSize(value); 
                    break;
                case CardEffectType.GainStatus:  
                    target?.ApplyStatus(e.StatusType, value); 
                    break;
                case CardEffectType.HalveSize:
                    if (target != null) target.ChangeSize(-(target.Size.Value / 2));
                    break;
                case CardEffectType.DrawCard:
                    _ctx.RequestDraw(value);
                    break;
                case CardEffectType.DiscardAllCard:
                    if (_ctx.Deck != null)
                        _discardedByThisCard = _ctx.Deck.DiscardHandExcept(c => c == playedCard);
                    _ctx.RequestDiscard();  
                    break;
                case CardEffectType.AddCardToDrawPile:
                    _ctx.Deck?.AddToDrawPile(e.CardToAdd, value);
                    break;
                case CardEffectType.PlayNextCardMultipleTimes:
                    _nextCardPlays = Mathf.Max(1, value);
                    break;
                case CardEffectType.ChooseDiscardThenDraw:
                    _ctx.RequestChooseDiscard(value);
                    break;
                default: break;
            }
        }
    }
}
