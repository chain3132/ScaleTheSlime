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

        private readonly Subject<CharacterFx> _fx = new();
        public Observable<CharacterFx> Fx => _fx;

        protected Character( int maxHealth, int startSize,int startHealth = -1)
        {
            MaxHealth = maxHealth;
            Health = new ReactiveProperty<int>(startHealth < 0 ? maxHealth : Mathf.Clamp(startHealth, 1, maxHealth));
            Size = new ReactiveProperty<int>(startSize);
            Shield = new ReactiveProperty<int>(0);
            Form = Size.Select(SizeFormUtil.FormOf).ToReadOnlyReactiveProperty();
        }

        public void TakeDamage(int amount, Character attacker = null)
        {
            if (IsDead || amount <= 0) return;

            if (attacker != null)
                amount = ModifiedDamage(amount, attacker);
            int remaining = amount;
            if (amount <= 0) return;
            if (Shield.Value > 0)
            {
                int absorbed = Mathf.Min(Shield.Value, remaining);
                Shield.Value -= absorbed;
                remaining -= absorbed;
            }
            if (remaining > 0)
                Health.Value = Mathf.Max(0, Health.Value - remaining);

            _fx.OnNext(remaining > 0 ? CharacterFx.Hit : CharacterFx.Block);

            if (Health.Value <= 0) Die(CharacterFx.Death);
        }
        private int ModifiedDamage(int baseValue, Character attacker)
        {
            float v = baseValue + attacker.Statuses.Get(StatusType.Strength);
            v *= 1f + attacker.Statuses.Get(StatusType.DamagePercent) / 100f;
            if (attacker.Statuses.Has(StatusType.Weak)) v *= 0.75f;
            if (Statuses.Has(StatusType.Vulnerable)) v *= 1.5f;   
            return Mathf.FloorToInt(v);
        }

        public void Heal(int amount)
        {
            if (IsDead || amount <= 0) return;
            Health.Value = Mathf.Min(MaxHealth, Health.Value + amount);
        }

        public void GainShield(int amount)
        {
            if (IsDead || amount <= 0) return;
            Shield.Value += amount + Statuses.Get(StatusType.ShieldBonus);
        }

        public void ChangeSize(int delta)
        {
            if (IsDead || delta == 0) return;
            ApplySize(Size.Value + delta);
        }
        public void SetSize(int newSize)
        {
            if (IsDead) return;
            ApplySize(newSize);
        }

        private void ApplySize(int newSize)
        {
            int old = Size.Value;
            newSize = Mathf.Clamp(newSize, 0, 10);
            if (newSize == old) return;

            Size.Value = newSize;

            var oldForm = SizeFormUtil.FormOf(old);
            var newForm = SizeFormUtil.FormOf(newSize);
            if (oldForm != newForm && newForm != SizeForm.Dead)
                _fx.OnNext(newSize > old ? CharacterFx.FormChangeL : CharacterFx.FormChangeS);
            else
                _fx.OnNext(newSize > old ? CharacterFx.SizeUp : CharacterFx.SizeDown);
        }

        public void CheckSizeDeath()
        {
            if (IsDead) return;
            if (IsLethalSize(Size.Value))
                Die(Size.Value >= 10 ? CharacterFx.DeathExcessSizeL : CharacterFx.DeathExcessSizeS);
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

        protected virtual void Die() => Die(CharacterFx.Death);

        protected void Die(CharacterFx cause)
        {
            if (IsDead) return;
            IsDead = true;
            _fx.OnNext(cause);
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
            _fx.Dispose();
        }
    }
}
