using Gameplay.BattleEncounter.Characters;
using Gameplay.BattleEncounter.Characters.Enums;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LitMotion;
using LitMotion.Extensions;
using Unity.VisualScripting;

namespace Gameplay.BattleEncounter.UI.Characters
{
    public class CharacterView : MonoBehaviour
    {
        [SerializeField] 
        private TMP_Text _hpText;
        [SerializeField] 
        private TMP_Text _sizeText;
        [SerializeField] 
        private TMP_Text _shieldText;
        [SerializeField] 
        private Image _sizeFill;
        [SerializeField]
        private Color _sizeColorTinyForm = Color.blue;
        [SerializeField]
        private Color _sizeColorNormal = Color.gray;
        [SerializeField]
        private Color _sizeColorGiantForm = Color.darkOrange;

        private readonly CompositeDisposable _bindings = new();

        public void Bind(Character c)
        {
            _bindings.Clear();

            if (_hpText != null)
                c.Health.Subscribe(h => _hpText.text = $"HP {h}/{c.MaxHealth}").AddTo(_bindings);
            if (_sizeText != null){
                c.Size.Subscribe(UpdateSizeText).AddTo(_bindings);}

            if (_sizeFill != null)
                c.Form.Subscribe(UpdateFillColor).AddTo(_bindings);
            
            if (_shieldText != null)
                c.Shield.Subscribe(s => _shieldText.text = s > 0 ? $"Shield {s}" : "").AddTo(_bindings);
        }

        private void UpdateSizeText(int size)
        {
            if (_sizeText != null)
                _sizeText.text = $"{size}";
            if (_sizeFill != null)
            {
                LMotion.Create(_sizeFill.fillAmount, size / 10f, 0.2f)
                    .BindToFillAmount(_sizeFill)
                    .AddTo(_bindings);
            }
        }

        private void UpdateFillColor(SizeForm size)
        {
            switch (size)
            {
                case SizeForm.Tiny:
                    _sizeFill.color = _sizeColorTinyForm;
                    break;
                case SizeForm.Normal:
                    _sizeFill.color = _sizeColorNormal;
                    break;
                default:
                    _sizeFill.color = _sizeColorGiantForm;
                    break;
                    
            }
        }

        private void OnDestroy() => _bindings.Dispose();
    }

}
