using Gameplay.BattleEncounter.Characters;
using Gameplay.BattleEncounter.Characters.Behaviors;
using TMPro;
using UnityEngine;

namespace Gameplay.BattleEncounter.UI.Characters
{
    
    public class EnemyIntentView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _valueText;

        [SerializeField] 
        private GameObject _stackGameObject;
        [SerializeField]
        private TMP_Text _stackText;   

        public void Bind(EnemyAction action, Enemy self = null)
        {
            SetValue(action, self);
            SetStack(action);
        }

        private void SetValue(EnemyAction a, Enemy self)
        {
            if (_valueText == null) return;

            string text = ValueText(a, self);
            bool show = !string.IsNullOrEmpty(text);
            _valueText.gameObject.SetActive(show);
            if (show) _valueText.text = text;
        }

        private static string ValueText(EnemyAction a, Enemy self)
        {
            switch (a.Type)
            {
                case EnemyActionType.Attack:
                    int dmg = self != null ? self.PreviewOutgoingDamage(a.Value) : a.Value;
                    return dmg.ToString();

                case EnemyActionType.GainArmor:
                case EnemyActionType.GrowSize:
                case EnemyActionType.ShrinkSize:
                case EnemyActionType.BuffSelf:
                case EnemyActionType.DebuffPlayer:
                case EnemyActionType.Heal:
                    return a.Value.ToString();

                default:
                    return string.Empty;
            }
        }

        private void SetStack(EnemyAction a)
        {
            if (_stackText == null) return;

            bool multi =  a.Repeat > 1;
            _stackGameObject.gameObject.SetActive(multi);
            if (multi) _stackText.text = $"x{a.Repeat}";
        }
    }
}
