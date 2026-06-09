using System;
using System.Collections.Generic;
using R3;

namespace Gameplay.BattleEncounter.Status
{
    // สถานะทั้งหมดของตัวละคร 1 ตัว (type → จำนวน stack)
    public class StatusEffects : IDisposable
    {
        private readonly Dictionary<StatusType, int> _stacks = new();
        private readonly Subject<Unit> _changed = new();

        public Observable<Unit> Changed => _changed;          // ให้ View รีเฟรชไอคอน
        public IReadOnlyDictionary<StatusType, int> All => _stacks;

        public int Get(StatusType type) => _stacks.TryGetValue(type, out var v) ? v : 0;
        public bool Has(StatusType type) => Get(type) > 0;

        // เพิ่ม/ลด stack (amount ติดลบได้); ถึง 0 = เอาออก
        public void Add(StatusType type, int amount)
        {
            if (amount == 0) return;
            int next = Get(type) + amount;
            if (next <= 0) _stacks.Remove(type);
            else _stacks[type] = next;
            _changed.OnNext(Unit.Default);
        }

        public void Clear()
        {
            if (_stacks.Count == 0) return;
            _stacks.Clear();
            _changed.OnNext(Unit.Default);
        }

        public void Dispose() => _changed.Dispose();
    }
}
