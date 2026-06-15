using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.BattleEncounter.Characters.Behaviors
{
    [CreateAssetMenu(menuName = "Battle/ActionIntentDatabase")]
    public class ActionIntentDatabase : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public EnemyActionType Type;
            public GameObject Prefab;
        }

        [SerializeField] private List<Entry> _entries = new();
        private Dictionary<EnemyActionType, GameObject> _map;

        public GameObject PrefabFor(EnemyActionType type)
        {
            if (_map == null)
            {
                _map = new Dictionary<EnemyActionType, GameObject>();
                foreach (var e in _entries)
                    if (e.Prefab != null) _map[e.Type] = e.Prefab;
            }
            return _map.TryGetValue(type, out var p) ? p : null;
        }
    }
}
