using System;
using UnityEngine;

namespace Gameplay.BattleEncounter.UI.Card
{
    [Serializable]
    public class CardSprites 
    {
        public Sprite CardBackground;
        public Sprite CardArt;
        public Sprite CardHeader;
        public bool IsSpecialCard; 
        public Sprite CardIcon;
        public Sprite CardFrameLeft;
        public Sprite CardFrameRight;
    }
}
