using UnityEngine;

namespace Gameplay.BattleEncounter.UI.Card
{
    
    public class CardViewModel
    {
        public string DisplayName { get; }
        public Sprite Art { get; }

        public CardViewModel(string displayName, Sprite art)
        {
            DisplayName = displayName;
            Art = art;
        }
    }
}
