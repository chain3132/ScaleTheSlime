using System.Threading;
using Cysharp.Threading.Tasks;

namespace Gameplay.NodeSelection.Encounter
{
    public interface IEncounter
    {
        UniTask<EncounterResult> EnterAsync(EncounterRequest request, CancellationToken ct);
    }
}
