using System;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MainMenu
{
    public class MainManuManager : MonoBehaviour
    {
        [SerializeField] 
        private Button _playButton;
        [SerializeField] 
        private CanvasGroup _fade;
        [SerializeField] 
        private float _fadeDuration = 0.4f;
        [SerializeField] 
        private string sceneName;
        
        private bool _loading;

        private void Start()
        {
            _playButton.onClick.AddListener(PlayGame);
        }

        private void PlayGame()
        {
            LoadSceneAsync(sceneName).Forget();
        }
        private async UniTaskVoid LoadSceneAsync(string sceneName)
        {
            if (_loading) return;
            _loading = true;

            await LMotion.Create(0f, 1f, _fadeDuration)
                .BindToAlpha(_fade)
                .ToUniTask(cancellationToken: destroyCancellationToken);

            await SceneManager.LoadSceneAsync(sceneName)
                .ToUniTask(cancellationToken: destroyCancellationToken);
        }

        private void OnDestroy()
        {
            _playButton.onClick.RemoveListener(PlayGame);
        }
    }
}
