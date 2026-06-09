using Gameplay.BattleEncounter.Battle;
using Gameplay.BattleEncounter.Characters.Data;
using Gameplay.BattleEncounter.Characters.Enums;
using Gameplay.BattleEncounter.Characters.Passives;
using Gameplay.BattleEncounter.UI.Card;
using R3;

namespace Gameplay.BattleEncounter.Characters
{
    public class Player : Character
    {
        private readonly PlayerDefinition _def;
        private readonly Subject<CardDefinition> _uniqueCardGranted = new();
        private readonly CompositeDisposable _scope = new();

        public Observable<CardDefinition> UniqueCardGranted => _uniqueCardGranted;

        private Passive _currentPassive;
        private BattleContext _ctx;

        public Player(PlayerDefinition def)
            : base(def.DisplayName, def.MaxHealth, def.StartSize)
        {
            _def = def;
        }

        public void Activate(BattleContext ctx)
        {
            _ctx = ctx;

            ApplyPassive(Form.CurrentValue);                       
            Form.Skip(1).Subscribe(OnFormChanged).AddTo(_scope);   
            Size.Skip(1).Subscribe(OnSizeChanged).AddTo(_scope);   
        }

        private void OnFormChanged(SizeForm form)
        {
            _currentPassive?.OnExit(this, _ctx);
            ApplyPassive(form);
        }

        private void ApplyPassive(SizeForm form)
        {
            _currentPassive = _def.PassiveFor(form);
            _currentPassive?.OnEnter(this, _ctx);
        }

        private void OnSizeChanged(int size)
        {
            var unique = _def.UniqueForSize(size);
            if (unique != null) _uniqueCardGranted.OnNext(unique);
        }

        public override void Dispose()
        {
            _currentPassive?.OnExit(this, _ctx);
            _scope.Dispose();
            _uniqueCardGranted.Dispose();
            base.Dispose();
        }
    }
}
