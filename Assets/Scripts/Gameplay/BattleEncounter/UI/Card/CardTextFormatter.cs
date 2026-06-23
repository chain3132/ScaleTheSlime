using System.Collections.Generic;
using Gameplay.BattleEncounter.Battle;
using Gameplay.BattleEncounter.Characters;

namespace Gameplay.BattleEncounter.UI.Card
{
    
    public static class CardTextFormatter
    {
        private const string BuffedColor = "#7CFC7C";   
        private const string DebuffedColor = "#FF6B6B"; 

        public static string Format(CardDefinition def, BattleContext ctx, Character target = null)
        {
            if (def == null) return string.Empty;
            string formattedDescription = def.Description;
            if (ctx == null || def.Effects == null || string.IsNullOrEmpty(formattedDescription))
                return formattedDescription;

            var tokenCounts = new Dictionary<string, int>();
            foreach (var effect in def.Effects)
            {
                string tokenName = TokenFor(effect);
                if (tokenName == null) continue;

                int tokenIndex = tokenCounts.TryGetValue(tokenName, out var count) ? count : 0;
                tokenCounts[tokenName] = tokenIndex + 1;

                int baseValue = CardEffectExecutor.ResolveValue(effect, ctx, target);
                int displayValue = CardEffectExecutor.PreviewDisplay(effect, ctx, target);
                string formattedValue = Colorize(displayValue, baseValue);

                if (tokenIndex == 0)
                {
                    formattedDescription = formattedDescription.Replace("{" + tokenName + "}", formattedValue);
                    formattedDescription = formattedDescription.Replace("{" + tokenName + "1}", formattedValue);
                }
                else
                {
                    formattedDescription = formattedDescription.Replace("{" + tokenName + (tokenIndex + 1) + "}", formattedValue);
                }
            }
            return formattedDescription;
        }
        public static string Format(CardDefinition def)
        {
            if (def == null) return string.Empty;
            string formattedDescription = def.Description;

            var tokenCounts = new Dictionary<string, int>();
            foreach (var effect in def.Effects)
            {
                string tokenName = TokenFor(effect);
                if (tokenName == null) continue;

                int tokenIndex = tokenCounts.TryGetValue(tokenName, out var count) ? count : 0;
                tokenCounts[tokenName] = tokenIndex + 1;
                string formattedValue = effect.CardValue.ToString();
                
                if (tokenIndex == 0)
                {
                    formattedDescription = formattedDescription.Replace("{" + tokenName + "}",formattedValue );
                    formattedDescription = formattedDescription.Replace("{" + tokenName + "1}", formattedValue);
                }
                else
                {
                    formattedDescription = formattedDescription.Replace("{" + tokenName + (tokenIndex + 1) + "}", formattedValue);
                }
            }
            return formattedDescription;
        }

        private static string Colorize(int displayValue, int baseValue)
        {
            if (displayValue > baseValue) return $"<color={BuffedColor}>{displayValue}</color>";
            if (displayValue < baseValue) return $"<color={DebuffedColor}>{displayValue}</color>";
            return displayValue.ToString();
        }

        private static string TokenFor(CardEffectData e)
        {
            switch (e.Type)
            {
                case CardEffectType.Attack:
                case CardEffectType.LoseHealth:
                    return "damage";
                case CardEffectType.GainShield:
                    return "block";
                case CardEffectType.Heal:
                    return "heal";
                case CardEffectType.ChangeSize:
                case CardEffectType.SetSize:
                    return "size";
                case CardEffectType.GainStatus:
                    return e.StatusType.ToString().ToLowerInvariant();
                case CardEffectType.DrawCard:
                case CardEffectType.ChooseDiscardThenDraw:
                    return "draw";
                case CardEffectType.PlayNextCardMultipleTimes:
                    return "plays";
                case CardEffectType.AddCardToDrawPile:
                    return "add";
                default:
                    return null;
            }
        }
    }
}
