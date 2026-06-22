using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MainMenu.UI
{
    public class UIParallax : MonoBehaviour
    {
        [Serializable]
        public class Layer
        {
            public RectTransform Target;
            public Vector2 MaxOffset = new Vector2(40f, 25f);
            public Vector2 Direction = new Vector2(1f, 1f);
        }

        [SerializeField]
        private Layer[] _layers = Array.Empty<Layer>();

        [SerializeField]
        private float _smooth = 6f;

        private Vector2[] _basePos;

        private void Awake()
        {
            _basePos = new Vector2[_layers.Length];
            for (int i = 0; i < _layers.Length; i++)
                if (_layers[i].Target != null)
                    _basePos[i] = _layers[i].Target.anchoredPosition;
        }

        private void Update()
        {
            var mouse = Mouse.current;
            if (mouse == null) return;

            Vector2 mouseVector2 = mouse.position.ReadValue();
            Vector2 normalized = new Vector2(
                Mathf.Clamp((mouseVector2.x / Screen.width) * 2f - 1f, -1f, 1f),
                Mathf.Clamp((mouseVector2.y / Screen.height) * 2f - 1f, -1f, 1f));

            float t = 1f - Mathf.Exp(-_smooth * Time.deltaTime); 

            for (int i = 0; i < _layers.Length; i++)
            {
                var layer = _layers[i];
                if (layer.Target == null) continue;     

                Vector2 target = _basePos[i] + new Vector2(
                    normalized.x * layer.MaxOffset.x * layer.Direction.x,
                    normalized.y * layer.MaxOffset.y * layer.Direction.y);

                layer.Target.anchoredPosition =
                    Vector2.Lerp(layer.Target.anchoredPosition, target, t);
            }
        }
    }
}
