using System;
using System.Collections.Generic;
using R3;

namespace Gameplay.BattleEncounter.Status
{
   
    public class StatusEffects : IDisposable
    {
        private readonly Dictionary<StatusType, int> _retain = new();
        private readonly Dictionary<StatusType, int> _temporary = new();
        private readonly Subject<Unit> _changed = new();

        public Observable<Unit> Changed => _changed;          

        public int Get(StatusType type) => GetValue(_retain, type) + GetValue(_temporary, type);
        public bool Has(StatusType type) => Get(type) > 0;

        public IEnumerable<KeyValuePair<StatusType, int>> Active
        {
            get
            {
                foreach (var kv in _retain)
                    yield return new(kv.Key, Get(kv.Key));   
                foreach (var kv in _temporary)
                    if (!_retain.ContainsKey(kv.Key))
                        yield return kv;                   
            }
        }

        private static int GetValue(Dictionary<StatusType, int> dict, StatusType type)
            => dict.TryGetValue(type, out var v) ? v : 0;

        public void Add(StatusType type, int amount, bool persistent = false)
        {
            if (amount == 0) return;
            var dict = persistent ? _retain : _temporary;
            int next = GetValue(dict, type) + amount;
            if (next <= 0) dict.Remove(type);
            else dict[type] = next;
            _changed.OnNext(Unit.Default);
        }

        public void ClearTemporary()
        {
            if (_temporary.Count == 0) return;
            _temporary.Clear();
            _changed.OnNext(Unit.Default);
        }

        public void Clear()
        {
            if (_retain.Count == 0 && _temporary.Count == 0) return;
            _retain.Clear();
            _temporary.Clear();
            _changed.OnNext(Unit.Default);
        }

        public void Dispose() => _changed.Dispose();
    }
}
