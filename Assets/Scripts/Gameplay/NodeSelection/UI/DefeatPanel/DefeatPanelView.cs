using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Gameplay.NodeSelection.UI.DefeatPanel
{
    public class DefeatPanelView : MonoBehaviour
    {
        [SerializeField] 
        private Button _goBackMenuButton;
        [SerializeField] 
        private string _sceneName;
        private bool _loading;

        private void Start()
        {
            _goBackMenuButton.onClick.AddListener(OnButtonClick);                
        }
        private void OnButtonClick()
        {
            GoBackMenu().Forget();
        }

        private async UniTaskVoid GoBackMenu()
        {
            if (_loading) return;
            _loading = true;

            await SceneManager.LoadSceneAsync(_sceneName)
                .ToUniTask(cancellationToken: destroyCancellationToken);
        }
        private void OnDestroy()
        {
            if (_goBackMenuButton != null)
            {
                _goBackMenuButton.onClick.RemoveListener(OnButtonClick);
            }
        }

        
    }
}
