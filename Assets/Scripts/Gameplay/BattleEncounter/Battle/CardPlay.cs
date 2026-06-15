using Gameplay.BattleEncounter.Characters;
using Gameplay.BattleEncounter.UI.Card.Data;

namespace Gameplay.BattleEncounter.Battle
{
    public readonly struct CardPlay
    {
        public readonly Card Card;
        public readonly Enemy Target;

        public CardPlay(Card card, Enemy target)
        {
            Card = card;
            Target = target;
        }
    }
}
