using System;
using NaughtyAttributes;
using UnityEngine;

namespace Gameplay.BattleEncounter.UI.Card
{
    [Serializable]
    public class CardSprites 
    {
        public Sprite CardBackground;
        public Sprite CardArt;
        public Sprite CardHeader;
        public bool IsSpecialCard = false; 
        [ShowIf(nameof(IsSpecialCard))]
        [AllowNesting]
        public Sprite CardIcon;
        [ShowIf(nameof(IsSpecialCard))]
        [AllowNesting]
        public Sprite CardFrameLeft;
        [ShowIf(nameof(IsSpecialCard))]
        [AllowNesting]
        public Sprite CardFrameRight;
    }
}
