using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Gameplay.BattleEncounter.Characters;
using Gameplay.BattleEncounter.Characters.Behaviors;
using Gameplay.BattleEncounter.Characters.Data;
using Gameplay.BattleEncounter.Characters.Enums;
using R3;
using Spine.Unity;
using UnityEngine;

namespace Gameplay.BattleEncounter.UI.Characters
{
    [RequireComponent(typeof(Collider2D))]
    public class EnemyView : MonoBehaviour
    {
        [SerializeField]
        private EnemyDefinition _definition;   
        [SerializeField]
        private CharacterView _view;
        [SerializeField]
        private ActionIntentDatabase _intentDatabase;
        [SerializeField]
        private SkeletonRendererCustomMaterials _skeletonRenderer;
        [SerializeField]
        private Transform _intentRoot;
        [SerializeField]
        private float _hideDelayOnDeath = 0.6f;

        public Enemy Enemy { get; private set; }
        public CharacterView View => _view;

        private readonly List<GameObject> _intentInstances = new();
        private readonly CompositeDisposable _scope = new();   
        public Enemy Setup(EnemyDefinition def = null)
        {
            Clear();   

            var d = def != null ? def : _definition;
            if (d == null) return null;

            Enemy = new Enemy(d);
            if (_view != null) _view.Bind(Enemy, d);

            Enemy.Form.Skip(1).Subscribe(OnFormChanged).AddTo(_scope);
            Enemy.Died.Subscribe(_ => OnDied()).AddTo(_scope);

            gameObject.SetActive(true);
            Replan();
            return Enemy;
        }

        public void Hide()
        {
            Clear();
            gameObject.SetActive(false);
        }

        public void Clear()
        {
            _scope.Clear();
            ClearIntent();
            HighLight(false);
            Enemy = null;
        }

        public void Replan()
        {
            if (Enemy == null || Enemy.IsDead) return;
            Enemy.PlanTurn();
            ShowIntent(Enemy.CurrentPlan);
        }

        private void OnFormChanged(SizeForm form)
        {
            if (form == SizeForm.Dead) return;
            Replan();
        }

        private void OnDied()
        {
            ClearIntent();
            HighLight(false);
            foreach (var col in GetComponents<Collider2D>()) col.enabled = false;
            HideAfterDeathAsync().Forget();
        }

        private async UniTaskVoid HideAfterDeathAsync()
        {
            if (_hideDelayOnDeath > 0f)
                await UniTask.Delay(TimeSpan.FromSeconds(_hideDelayOnDeath),
                    cancellationToken: destroyCancellationToken);
            gameObject.SetActive(false);
        }

        public void ShowIntent(IReadOnlyList<EnemyAction> plan)
        {
            ClearIntent();
            if (_intentDatabase == null || _intentRoot == null) return;

            foreach (var a in plan)
            {
                var prefab = _intentDatabase.PrefabFor(a.Type);
                if (prefab != null)
                    _intentInstances.Add(Instantiate(prefab, _intentRoot));
            }
        }

        public void HighLight(bool highlight)
        {
            if (_skeletonRenderer != null)
                _skeletonRenderer.enabled = highlight;
        }

        public void ClearIntent()
        {
            foreach (var go in _intentInstances) Destroy(go);
            _intentInstances.Clear();
        }

        private void OnDestroy() => _scope.Dispose();
    }
}
