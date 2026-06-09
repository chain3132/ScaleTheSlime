using Gameplay.BattleEncounter.Characters;
using R3;
using TMPro;
using UnityEngine;

namespace Gameplay.BattleEncounter.UI.Characters
{
    public class CharacterView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _hpText;
        [SerializeField] private TMP_Text _sizeText;
        [SerializeField] private TMP_Text _shieldText;

        private readonly CompositeDisposable _bindings = new();

        public void Bind(Character c)
        {
            _bindings.Clear();

            if (_hpText != null)
                c.Health.Subscribe(h => _hpText.text = $"HP {h}/{c.MaxHealth}").AddTo(_bindings);
            if (_sizeText != null)
                c.Size.Subscribe(s => _sizeText.text = $"Size {s}").AddTo(_bindings);
            if (_shieldText != null)
                c.Shield.Subscribe(s => _shieldText.text = s > 0 ? $"Shield {s}" : "").AddTo(_bindings);
        }

        private void OnDestroy() => _bindings.Dispose();
    }
}
