using System;
using Gameplay.BattleEncounter.Characters.Enums;
using Gameplay.BattleEncounter.Status;
using R3;
using UnityEngine;

namespace Gameplay.BattleEncounter.Characters
{
    public abstract class Character : IDisposable
    {
        public string Name { get; }
        public int MaxHealth { get; }

        public ReactiveProperty<int> Health { get; }  
        public ReactiveProperty<int> Size { get; }     
        public ReactiveProperty<int> Shield { get; }    
        public ReadOnlyReactiveProperty<SizeForm> Form { get; } 
        public StatusEffects Statuses { get; } = new();

        public bool IsDead { get; private set; }

        private readonly Subject<Unit> _died = new();
        public Observable<Unit> Died => _died;

        protected Character(string name, int maxHealth, int startSize)
        {
            Name = name;
            MaxHealth = maxHealth;
            Health = new ReactiveProperty<int>(maxHealth);
            Size = new ReactiveProperty<int>(startSize);
            Shield = new ReactiveProperty<int>(0);
            Form = Size.Select(SizeFormUtil.FormOf).ToReadOnlyReactiveProperty();
        }

        public void TakeDamage(int amount)
        {
            if (IsDead || amount <= 0) return;

            int remaining = amount;
            if (Shield.Value > 0)
            {
                int absorbed = Mathf.Min(Shield.Value, remaining);
                Shield.Value -= absorbed;
                remaining -= absorbed;
            }
            if (remaining > 0)
                Health.Value = Mathf.Max(0, Health.Value - remaining);

            if (Health.Value <= 0) Die();
        }

        public void Heal(int amount)
        {
            if (IsDead || amount <= 0) return;
            Health.Value = Mathf.Min(MaxHealth, Health.Value + amount);
        }

        public void GainShield(int amount)
        {
            if (IsDead || amount <= 0) return;
            Shield.Value += amount;
        }

        public void ChangeSize(int delta)
        {
            if (IsDead || delta == 0) return;
            int next = Mathf.Clamp(Size.Value + delta, 0, 10);
            Size.Value = next;
            if (next == 0 || next == 10) Die();
        }

        public void ApplyStatus(StatusType type, int amount)
        {
            if (IsDead) return;
            Statuses.Add(type, amount);
        }

        protected virtual void Die()
        {
            if (IsDead) return;
            IsDead = true;
            _died.OnNext(Unit.Default);
        }

        public virtual void Dispose()
        {
            Health.Dispose();
            Size.Dispose();
            Shield.Dispose();
            Form.Dispose();
            Statuses.Dispose();
            _died.Dispose();
        }
    }
}
