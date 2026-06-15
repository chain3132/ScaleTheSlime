using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

namespace Gameplay.BattleEncounter.Status
{
    public class StatusView : MonoBehaviour
    {
        [SerializeField] 
        private Transform _root;
        [SerializeField] 
        private StatusIconView _iconPrefab;
        [SerializeField] 
        private StatusDatabase _database;

        private readonly List<StatusIconView> _spawned = new();
        private IDisposable _sub;

        public void Bind(StatusEffects statuses)
        {
            _sub?.Dispose();
            if (statuses == null) return;

            _sub = statuses.Changed.Subscribe(_ => Rebuild(statuses));
            Rebuild(statuses);  
        }

        private void Rebuild(StatusEffects statuses)
        {
            Debug.Log("Rebuilding status view");
            foreach (var v in _spawned)
                if (v != null) Destroy(v.gameObject);
            _spawned.Clear();

            var parent = _root != null ? _root : transform;
            foreach (var kv in statuses.Active)
            {
                var icon = Instantiate(_iconPrefab, parent);
                icon.SetData(_database != null ? _database.IconFor(kv.Key) : null, kv.Value);
                _spawned.Add(icon);
            }
        }

        private void OnDestroy() => _sub?.Dispose();
    }
}
