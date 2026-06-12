using System;
using System.Collections.Generic;
using Gameplay.BattleEncounter.Characters.Enums;
using UnityEngine;

namespace Gameplay.BattleEncounter.UI.Characters
{
    [CreateAssetMenu(menuName = "Battle/CharacterFxDatabase")]
    public class CharacterFxDatabase : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public CharacterFx Type;
            public ParticleSystem Prefab;
        }

        [SerializeField] private List<Entry> _entries = new();
        private Dictionary<CharacterFx, ParticleSystem> _map;

        public ParticleSystem PrefabFor(CharacterFx type)
        {
            if (_map == null)
            {
                _map = new Dictionary<CharacterFx, ParticleSystem>();
                foreach (var e in _entries)
                    if (e.Prefab != null) _map[e.Type] = e.Prefab;
            }
            return _map.TryGetValue(type, out var p) ? p : null;
        }
    }
}
