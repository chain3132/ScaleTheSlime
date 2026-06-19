using Gameplay.BattleEncounter.Characters.Enums;
using Gameplay.BattleEncounter.Characters.Passives;

namespace Gameplay.BattleEncounter.Characters.Data
{
    public interface IPassiveProvider
    {
        Passive PassiveFor(SizeForm form);
    }
}
