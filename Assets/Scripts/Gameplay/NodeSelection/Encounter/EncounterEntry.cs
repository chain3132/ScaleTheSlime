using System.Threading;
using Cysharp.Threading.Tasks;

namespace Gameplay.NodeSelection.Encounter
{
    public class EncounterEntry : IEncounter
    {
        private readonly bool _forceDefeat;
        public EncounterEntry(bool forceDefeat = false) { _forceDefeat = forceDefeat; }
        
        public UniTask<EncounterResult> EnterAsync(EncounterRequest request, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            return UniTask.FromResult(_forceDefeat ? EncounterResult.Defeat : EncounterResult.Victory);
        }
    }
}
