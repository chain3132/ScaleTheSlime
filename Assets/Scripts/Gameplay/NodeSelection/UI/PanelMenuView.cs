using R3;
using TMPro;
using UnityEngine;

namespace Gameplay.NodeSelection.UI
{
    public class PanelMenuView : MonoBehaviour
    {
        #region References

        [SerializeField] 
        private TMP_Text _nameText;
        [SerializeField] 
        private TMP_Text _runText;
        [SerializeField] 
        private TMP_Text _turnText;
        [SerializeField] 
        private TMP_Text _clockText;

        #endregion
        
        private readonly CompositeDisposable _bindings = new();

        public void Bind(PanelMenuViewModel vm)
        {
            _nameText.text = vm.PlayerName;
 
            vm.RunNumber.Subscribe(r => _runText.text = $"Run {r}").AddTo(_bindings);
            vm.TurnText.Subscribe(t => _turnText.text = string.IsNullOrEmpty(t) ? "" : $"Turn {t}").AddTo(_bindings);
            vm.ClockText.Subscribe(c => _clockText.text = c).AddTo(_bindings);
        }
 
        private void OnDestroy()
        {
            _bindings.Dispose();
        }
    }
}
