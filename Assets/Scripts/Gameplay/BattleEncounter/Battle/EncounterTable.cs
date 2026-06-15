using System;
using System.Collections.Generic;
using Gameplay.BattleEncounter.Characters.Data;
using Gameplay.NodeSelection.UI.Nodes.Enums;
using UnityEngine;

namespace Gameplay.BattleEncounter.Battle
{
    [CreateAssetMenu(menuName = "Battle/EncounterTable")]
    public class EncounterTable : ScriptableObject
    {
        [Serializable]
        public class EncounterSet
        {
            public NodeType NodeType = NodeType.Minor;
            public int MinLayer = 0;
            public int MaxLayer = 99;
            public List<EnemyDefinition> Enemies = new();   
        }

        [SerializeField]
        private List<EncounterSet> _sets = new();

        public IReadOnlyList<EnemyDefinition> Roll(NodeType type, int layer, System.Random rng)
        {
            var encounterRng = new List<EncounterSet>();
            foreach (var s in _sets)
                if (s.NodeType == type && layer >= s.MinLayer && layer <= s.MaxLayer && s.Enemies.Count > 0)
                    encounterRng.Add(s);

            if (encounterRng.Count == 0) return Array.Empty<EnemyDefinition>();
            return encounterRng[rng.Next(encounterRng.Count)].Enemies;
        }
    }
}
