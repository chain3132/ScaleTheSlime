using Gameplay.NodeSelection.UI.Nodes.Enums;
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
        private Image _nodeTypeIcon;
        [SerializeField] 
        private GameObject _glow;

        #endregion
        
        private readonly CompositeDisposable _bindings = new();

        
        public void Bind(NodeViewModel vm,
            Sprite icon)
        {
            _nodeTypeIcon.sprite = icon;
        
            vm.IsReachable.Subscribe(on =>
            {
                _glow.SetActive(on);
                _button.interactable = on;
            }).AddTo(_bindings);
            _button.OnClickAsObservable()
                .Subscribe(_ => vm.Select())
                .AddTo(_bindings);
        }
        
        
        private void OnDestroy()
        {
            _bindings.Dispose();
        }
    }
}
