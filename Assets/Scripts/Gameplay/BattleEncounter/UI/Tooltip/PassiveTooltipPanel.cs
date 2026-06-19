using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Gameplay.BattleEncounter.UI.Tooltip
{
    public class PassiveTooltipPanel : TooltipPanel
    {
        [SerializeField] 
        private TMP_Text _title;
        [SerializeField] 
        private Transform _rowRoot;
        [SerializeField] 
        private PassiveRowView _rowPrefab;

        private readonly List<PassiveRowView> _rows = new();

        public override void Bind(TooltipData data)
        {
            if (_title != null && !string.IsNullOrEmpty(data.Title)) _title.text = data.Title;

            ClearRows();
            if (data.Lines == null || _rowPrefab == null) return;

            var parent = _rowRoot != null ? _rowRoot : transform;
            foreach (var line in data.Lines)
            {
                var row = Instantiate(_rowPrefab, parent);
                row.Bind(line);
                _rows.Add(row);
            }
        }

        private void ClearRows()
        {
            foreach (var r in _rows)
                if (r != null) Destroy(r.gameObject);
            _rows.Clear();
        }

        private void OnDisable() => ClearRows();
    }
}
