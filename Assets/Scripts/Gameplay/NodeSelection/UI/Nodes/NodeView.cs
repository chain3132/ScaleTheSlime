using Gameplay.NodeSelection.UI.Nodes.Enums;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using R3;
using UnityEngine.UI;

namespace Gameplay.NodeSelection.UI.Nodes
{
    public class NodeView : MonoBehaviour
    {
        #region References

        [SerializeField] 
        private Button _button;
        [SerializeField] 
        private Image  _glow;
        [SerializeField]
        private SerializableMotionSettings<Color,NoOptions> _glowSettings;

        #endregion
        private MotionHandle _glowMotion;
        private readonly CompositeDisposable _bindings = new();

        
        public void Bind(NodeViewModel vm)
        {
            vm.IsReachable.Subscribe(on =>
            {
                _button.interactable = on;
                if (on) StartGlow();
                else    StopGlow();
            }).AddTo(_bindings);
            _button.OnClickAsObservable()
                .Subscribe(_ => vm.Select())
                .AddTo(_bindings);
        }

        private void StartGlow()
        {
            StopGlow();                         
            _glow.gameObject.SetActive(true);
            _glowMotion = LMotion.Create(_glowSettings).BindToColor(_glow);
        }
        private void StopGlow()
        {
            if (_glowMotion.IsActive()) _glowMotion.Cancel();
            _glow.gameObject.SetActive(false);
        }
        
        
        private void OnDestroy()
        {
            if (_glowMotion.IsActive()) _glowMotion.Cancel();   // ★ สำคัญ
            _bindings.Dispose();
        }
    }
}
