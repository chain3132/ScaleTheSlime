using System;
using System.Collections.Generic;
using Gameplay.BattleEncounter.Characters.Enums;
using UnityEngine;

namespace Gameplay.BattleEncounter.UI.Tooltip
{

    [CreateAssetMenu(menuName = "Battle/SizeFormInfoDatabase")]
    public class SizeFormInfoDatabase : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public SizeForm Form;
            public string Badge = "S";       // S / M / L
            public Color Color = Color.white;
            public string Title;
            [TextArea] 
            public string Description;
        }

        [SerializeField] 
        private List<Entry> _entries = new();
        private Dictionary<SizeForm, Entry> _map;

        private Dictionary<SizeForm, Entry> Map
        {
            get
            {
                if (_map == null)
                {
                    _map = new Dictionary<SizeForm, Entry>();
                    foreach (var e in _entries)
                        if (e != null) _map[e.Form] = e;
                }
                return _map;
            }
        }

        public Entry InfoFor(SizeForm form)
        {
            return Map.TryGetValue(form, out var e) ? e : null;
        }

        public string BadgeFor(SizeForm form)
        {
            return InfoFor(form)?.Badge ?? string.Empty;    
        }

        public Color ColorFor(SizeForm form)
        {
            return InfoFor(form) is { } e ? e.Color : Color.white;
            
        }
    }
}
