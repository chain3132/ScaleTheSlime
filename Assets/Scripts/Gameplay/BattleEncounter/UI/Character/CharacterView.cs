using System.Threading;
using Coffee.UIExtensions;
using Cysharp.Threading.Tasks;
using Gameplay.BattleEncounter.Characters;
using Gameplay.BattleEncounter.Characters.Data;
using Gameplay.BattleEncounter.Characters.Enums;
using Gameplay.BattleEncounter.Status;
using R3;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LitMotion;
using LitMotion.Extensions;
using Unity.VisualScripting;
using UnityEngine.Serialization;

namespace Gameplay.BattleEncounter.UI.Characters
{
    public class CharacterView : MonoBehaviour
    {
        #region References

        [SerializeField] 
        private TMP_Text _hpText;
        [SerializeField] 
        private TMP_Text _sizeText;
        [SerializeField] 
        private TMP_Text _shieldText;
        [SerializeField] 
        private Image _sizeFill;
        [SerializeField]
        private Image _hpFill;
        [SerializeField] 
        private Image _hpFillBackground;
        [SerializeField] 
        private RectTransform _hpIndicator;
        [SerializeField]
        private UIParticle shieldGainEffect;
        [SerializeField]
        private Image shieldIcon;
        [SerializeField]
        private Image healthIcon;
        [SerializeField]
        private CharacterFxDatabase _fxDatabase;
        [SerializeField]
        private Transform _fxAnchor;
        [FormerlySerializedAs("_statusStrip")]
        [SerializeField]
        private StatusView status;

        [Header("Form visual")]
        [SerializeField]
        private SkeletonAnimation _skeleton;    
        [SerializeField]
        private RectTransform _sizeRoot;         
        [SerializeField]
        private string _idleAnimationName = "idle";


        #endregion

        #region Setup

        [SerializeField]
        private Color _sizeColorTinyForm = Color.blue;
        [SerializeField]
        private Color _sizeColorNormal = Color.gray;
        [SerializeField]
        private Color _sizeColorGiantForm = Color.darkOrange;

        #endregion
        
        

        private readonly CompositeDisposable _bindings = new();
        private ParticleSystem[] _particles;
        private int _currentHealth;
        private int _maxHealth;

        public void Bind(Character c, CharacterDefinition def = null)
        {
            _bindings.Clear();

            c.Fx.SubscribeAwait((fx, ct) => PlayFxAsync(fx, ct), AwaitOperation.Sequential)
                .AddTo(_bindings);
            _maxHealth = c.MaxHealth;

            status?.Bind(c.Statuses);

            if (def != null && (_skeleton != null || _sizeRoot != null))
                c.Form.Subscribe(form => ApplyForm(form, def)).AddTo(_bindings);

            if (_hpText != null)
                c.Health.Subscribe(h 
                    => UpdateHealth(h,c.MaxHealth))
                    .AddTo(_bindings);
            if (_sizeText != null){
                c.Size.Subscribe(UpdateSizeText)
                    .AddTo(_bindings);}

            if (_sizeFill != null)
                c.Form.Subscribe(UpdateFillColor)
                    .AddTo(_bindings);
            
            if (_shieldText != null)
                c.Shield.Skip(1).
                    SubscribeAwait((shield, ct) 
                        => UpdateShield(shield,ct))
                    .AddTo(_bindings);
        }
        private void ApplyForm(SizeForm form, CharacterDefinition def)
        {
            if (form == SizeForm.Dead) return;   

            if (_skeleton != null)
            {
                var data = def.SkeletonFor(form);
                if (data != null && _skeleton.skeletonDataAsset != data)
                {
                    _skeleton.skeletonDataAsset = data;
                    _skeleton.Initialize(true);
                    if (!string.IsNullOrEmpty(_idleAnimationName))
                        _skeleton.AnimationState.SetAnimation(0, _idleAnimationName, true);
                }
            }

            if (_sizeRoot != null)
                _sizeRoot.anchoredPosition =
                    new Vector2(_sizeRoot.anchoredPosition.x, def.OffsetYFor(form));
        }

        private async UniTask PlayFxAsync(CharacterFx type, CancellationToken ct)
        {
            if (_fxDatabase == null) return;
            var prefab = _fxDatabase.PrefabFor(type);
            if (prefab == null) return;

            var pos = _fxAnchor != null ? _fxAnchor.position : transform.position;
            var fx = Instantiate(prefab, pos, prefab.transform.rotation);

            await UniTask.Delay(System.TimeSpan.FromSeconds(fx.main.duration), cancellationToken: ct);
        }

        private async UniTask UpdateShield(int shield, CancellationToken ct)
        {
            healthIcon.gameObject.SetActive(false);
            shieldIcon.gameObject.SetActive(true);
            shieldGainEffect.gameObject.SetActive(true);
            _particles = shieldGainEffect.GetComponentsInChildren<ParticleSystem>();
            shieldGainEffect.Play();

            await UniTask.WaitUntil(() => {
                foreach (var ps in _particles)
                {
                    if (ps.IsAlive(true))
                    {
                        return false;
                    }
                }
                return true;}, cancellationToken: ct);     
            if (_shieldText != null)
                _shieldText.text = shield > 0 ? $"{shield}" : "";
        }
        private void UpdateHealth(int health, int maxHealth)
        {
            _currentHealth = health;
            _hpText.text = $"{health}/{maxHealth}";
            if (_hpFillBackground != null)
            {
                LMotion.Create(_hpFillBackground.fillAmount, (float)health / maxHealth, 0.25f)
                    .BindToFillAmount(_hpFillBackground)
                    .AddTo(_bindings);
            }
            if (_hpFill != null)
            {
                LMotion.Create(_hpFill.fillAmount, (float)health / maxHealth, 0.2f)
                    .BindToFillAmount(_hpFill)
                    .AddTo(_bindings);
            }
            if (_hpIndicator != null) _hpIndicator.gameObject.SetActive(false);
        }

        public void ShowDamagePreview(int predictedDamage)
        {
            int previewHp = Mathf.Max(0, _currentHealth - predictedDamage);
            var fillAmount = (float)previewHp / _maxHealth;
            _hpText.text = $"{previewHp}/{_maxHealth}";
            LMotion.Create(_hpFill.fillAmount, fillAmount, 0.2f)
                .BindToFillAmount(_hpFill)
                .AddTo(_bindings);
            UpdatePositionIndicator(fillAmount);
            if (_hpIndicator != null)
            {
                _hpIndicator.gameObject.SetActive(predictedDamage > 0);
            }
            
        }

        public void ClearDamagePreview()
        {
            float f = (float)_currentHealth / _maxHealth;
            _hpText.text = $"{_currentHealth}/{_maxHealth}";

            LMotion.Create(_hpFill.fillAmount, f, 0.12f).BindToFillAmount(_hpFill).AddTo(_bindings);
            if (_hpIndicator != null) _hpIndicator.gameObject.SetActive(false);
        }

        public void UpdatePositionIndicator(float fillAmount)
        {
            var min = _hpIndicator.anchorMin;
            min.x = fillAmount; 
            _hpIndicator.anchorMin = min;
            var max = _hpIndicator.anchorMax; 
            max.x = fillAmount; 
            _hpIndicator.anchorMax = max;
            _hpIndicator.anchoredPosition = new Vector2(0f, _hpIndicator.anchoredPosition.y);
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
