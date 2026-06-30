using Gameplay.BattleEncounter.Characters.Behaviors;
using Gameplay.BattleEncounter.Status;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Gameplay.BattleEncounter.UI.Tooltip
{

    public class IntentTooltipTrigger : TooltipTrigger
    {
        [SerializeField] 
        private StatusDatabase _statusDatabase;
        private const string Table  = "Intents";

        private EnemyAction _action;

        public void SetAction(EnemyAction action) => _action = action;

        protected override bool TryBuild(out TooltipData data)
        {
            if (_action == null) { data = default; return false; }
            
            string title = LocalizationSettings.StringDatabase.GetLocalizedString(Table, GetTitle(_action));
            data = TooltipData.Standard(title, Describe(_action));
            return true;
        }

        private string GetTitle(EnemyAction a)
        {
             string key = a.Type switch
            {
                EnemyActionType.Attack       => a.Repeat > 1 ? "intent.attackmulti.name" : "intent.attack.name",
                EnemyActionType.GainArmor    => "intent.gainarmor.name",
                EnemyActionType.GrowSize     => "intent.growsize.name",
                EnemyActionType.ShrinkSize   => "intent.shrinksize.name",
                EnemyActionType.BuffSelf     => "intent.buffself.name",
                EnemyActionType.DebuffPlayer => "intent.debuffplayer.name",
                EnemyActionType.AddCardToDrawPile => "intent.addcardtodrawpile.name",
                _ => null,
            };
             return key;
        }
        private string Describe(EnemyAction a)
        {
            string key = a.Type switch
            {
                EnemyActionType.Attack       => a.Repeat > 1 ? "intent.attackmulti.description" : "intent.attack.description",
                EnemyActionType.GainArmor    => "intent.gainarmor.description",
                EnemyActionType.GrowSize     => "intent.growsize.description",
                EnemyActionType.ShrinkSize   => "intent.shrinksize.description",
                EnemyActionType.BuffSelf     => "intent.buffself.description",
                EnemyActionType.DebuffPlayer => "intent.debuffplayer.description",
                EnemyActionType.AddCardToDrawPile => "intent.addcardtodrawpile.description",
                _ => null,
            };
            string status = _statusDatabase != null ? _statusDatabase.NameFor(a.Status) : a.Status.ToString();

            string result = LocalizationSettings.StringDatabase.GetLocalizedString(Table, key)
                .Replace("{value}",  a.Value.ToString())
                .Replace("{repeat}", a.Repeat.ToString())
                .Replace("{status}", status);
            if (a.cardToAdd != null)
            {
                result = result.Replace("{card}", a.cardToAdd.DisplayName?.GetLocalizedString());
            }                     
            return result;
        }
        
    }
}
