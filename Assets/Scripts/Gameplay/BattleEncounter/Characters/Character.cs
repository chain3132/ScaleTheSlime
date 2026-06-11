using System;
using Gameplay.BattleEncounter.Characters.Enums;
using Gameplay.BattleEncounter.Status;
using R3;
using UnityEngine;

namespace Gameplay.BattleEncounter.Characters
{
    public abstract class Character : IDisposable
    {
        public int MaxHealth { get; }

        public ReactiveProperty<int> Health { get; }  
        public ReactiveProperty<int> Size { get; }     
        public ReactiveProperty<int> Shield { get; }    
        public ReadOnlyReactiveProperty<SizeForm> Form { get; } 
        public StatusEffects Statuses { get; } = new();

        public bool IsDead { get; private set; }

        private readonly Subject<Unit> _died = new();
        public Observable<Unit> Died => _died;

        protected Character( int maxHealth, int startSize,int startHealth = -1)
        {
            MaxHealth = maxHealth;
            Health = new ReactiveProperty<int>(startHealth < 0 ? maxHealth : Mathf.Clamp(startHealth, 1, maxHealth));
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
            Size.Value = Mathf.Clamp(Size.Value + delta, 0, 10);
        }

        public void CheckSizeDeath()
        {
            if (IsDead) return;
            if (IsLethalSize(Size.Value)) Die();
        }

        protected virtual bool IsLethalSize(int size) => size <= 0 || size >= 10;

        public void ApplyStatus(StatusType type, int amount, bool persistent = false)
        {
            if (IsDead) return;
            Statuses.Add(type, amount, persistent);
        }

        public void ClearShield()
        {
            if (Shield.Value != 0) Shield.Value = 0;
        }

        public void ClearTemporaryStatuses()
        {
            Statuses.ClearTemporary();
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
