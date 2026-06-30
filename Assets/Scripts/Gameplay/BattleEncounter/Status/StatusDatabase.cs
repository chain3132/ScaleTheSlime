    using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
    using UnityEngine.Localization;

    namespace Gameplay.BattleEncounter.Status
{
    [CreateAssetMenu(menuName = "Battle/StatusDatabase")]
    public class StatusDatabase : ScriptableObject
    {
        [Serializable]
        public class StatusEntry
        {
            public StatusType Type;
            public Sprite Icon;
            public LocalizedString DisplayName;
            public LocalizedString Description;
        }

        [SerializeField] 
        private List<StatusEntry> _entries = new();
        private Dictionary<StatusType, StatusEntry> _map;

        private Dictionary<StatusType, StatusEntry> Map
        {
            get { return _map ??= _entries.Where(e => e != null).ToDictionary(e => e.Type); }
        }

        public Sprite IconFor(StatusType type)
        {
            return Map.TryGetValue(type, out var e) ? e.Icon : null;
        }

        public string NameFor(StatusType type)
        {
            return Map.TryGetValue(type, out var e) ? e.DisplayName.GetLocalizedString() : type.ToString();
        }

        public string DescriptionFor(StatusType type)
        {
            return Map.TryGetValue(type, out var e) ? e.Description.GetLocalizedString() : string.Empty;
        }
    }
}
