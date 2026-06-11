using System.Collections.Generic;
using Gameplay.BattleEncounter.UI.Card;
using UnityEngine;

namespace Gameplay.NodeSelection.UI
{
    public class RunProgress 
    { 
        public int MaxHealth { get;}
        public int CurrentHealth { get; private set; }
        public List<CardDefinition> Deck { get; }

        public RunProgress(int maxHealth, List<CardDefinition> deck)
        {
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            Deck = deck;
        }   
        public void SetHealth(int hp)
        {
            CurrentHealth = Mathf.Clamp(hp, 0, MaxHealth);
        }

        public void HealFull() => CurrentHealth = MaxHealth;
    }
}
