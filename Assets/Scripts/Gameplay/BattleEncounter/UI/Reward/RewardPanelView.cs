using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.BattleEncounter.UI.Card;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.BattleEncounter.UI.Reward
{
    public class RewardPanelView : MonoBehaviour
    {
        [Serializable]
        private class Slot
        {
            public CardView Card;     
            public Button Button;     
        }

        [SerializeField]
        private GameObject _root;
        [SerializeField]
        private List<Slot> _slots = new();
        [SerializeField]
        private Button _skipButton;

        private UniTaskCompletionSource<CardDefinition> _choice;

        public async UniTask<CardDefinition> ShowAsync(IReadOnlyList<CardDefinition> choices, CancellationToken ct)
        {
            _choice = new UniTaskCompletionSource<CardDefinition>();
            _root.SetActive(true);

            for (int i = 0; i < _slots.Count; i++)
            {
                var slot = _slots[i];
                if (slot == null || slot.Card == null) continue;

                bool has = i < choices.Count && choices[i] != null;
                slot.Card.gameObject.SetActive(has);
                if (slot.Button != null) slot.Button.gameObject.SetActive(has);
                if (!has) continue;

                var def = choices[i];
                slot.Card.Bind(new CardViewModel(new Card.Data.Card(def)), null);
                slot.Button.onClick.RemoveAllListeners();
                slot.Button.onClick.AddListener(() => _choice.TrySetResult(def));
            }

            _skipButton.onClick.RemoveAllListeners();
            _skipButton.onClick.AddListener(() => _choice.TrySetResult(null));

            try
            {
                return await _choice.Task.AttachExternalCancellation(ct);
            }
            finally
            {
                _root.SetActive(false);
            }
        }
    }
}
