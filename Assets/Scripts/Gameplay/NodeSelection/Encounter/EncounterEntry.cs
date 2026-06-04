using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Gameplay.NodeSelection.Encounter
{
    public class EncounterEntry : IEncounter
    {
        private readonly bool _forceDefeat;

        public EncounterEntry(bool forceDefeat = false)
        {
            _forceDefeat = forceDefeat;
        }
        
        public async UniTask<EncounterResult> EnterAsync(EncounterRequest request, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            await UniTask.Delay(TimeSpan.FromSeconds(1.5f), cancellationToken: ct);
            return _forceDefeat ? EncounterResult.Defeat : EncounterResult.Victory;        }
    }
}
