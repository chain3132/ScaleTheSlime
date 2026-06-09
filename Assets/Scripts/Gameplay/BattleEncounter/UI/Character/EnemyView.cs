using Gameplay.BattleEncounter.Characters;
using Gameplay.BattleEncounter.Characters.Data;
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

        public Enemy Enemy { get; private set; }

        public Enemy Create()
        {
            Enemy = new Enemy(_definition);
            if (_view != null) _view.Bind(Enemy);
            return Enemy;
        }
    }
}
