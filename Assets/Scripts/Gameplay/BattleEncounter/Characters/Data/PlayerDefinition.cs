using Gameplay.BattleEncounter.Characters.Enums;
using Gameplay.BattleEncounter.Characters.Passives;
using Gameplay.BattleEncounter.UI.Card;
using Gameplay.BattleEncounter.UI.Card.Data;
using UnityEngine;

namespace Gameplay.BattleEncounter.Characters.Data
{
    [CreateAssetMenu(menuName = "Battle/PlayerDefinition")]
    public class PlayerDefinition : CharacterDefinition
    {
        [Header("Passive")]
        public Passive TinyPassive;
        public Passive NormalPassive;
        public Passive GiantPassive;

        [Header("Unique card ")]
        public CardDefinition Unique2;
        public CardDefinition Unique5;
        public CardDefinition Unique8;

        public DeckDefinition StartingDeck;

        public Passive PassiveFor(SizeForm form) 
        {
            switch (form)
            {
                case SizeForm.Tiny:
                    return TinyPassive;
                case SizeForm.Normal:
                    return NormalPassive;
                case SizeForm.Giant:
                    return GiantPassive;
                default:
                    return null;
            }
        }

        public CardDefinition UniqueForSize(int size) 
        {
            switch (size)
            {
                case 2:
                    return Unique2;
                case 5:
                    return Unique5;
                case 8:
                    return Unique8;
                default:
                    return null;
            }
        }
    }
}
