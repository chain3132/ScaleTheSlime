using System.Collections.Generic;
using Gameplay.BattleEncounter.Characters;
using Gameplay.BattleEncounter.Characters.Behaviors;
using Gameplay.BattleEncounter.Characters.Data;
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

        public Enemy Enemy { get; private set; }

        private readonly List<GameObject> _intentInstances = new();

        public Enemy Create()
        {
            Enemy = new Enemy(_definition);
            if (_view != null) _view.Bind(Enemy);

            Enemy.PlanTurn();               
            ShowIntent(Enemy.CurrentPlan); 
            return Enemy;
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
    }
}
