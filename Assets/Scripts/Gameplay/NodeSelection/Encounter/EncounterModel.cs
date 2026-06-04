using Gameplay.NodeSelection.UI.Nodes.Enums;

namespace Gameplay.NodeSelection.Encounter
{
    public enum EncounterOutcome
    {
        Victory,
        Defeat
    }

    public readonly struct EncounterRequest
    {
        public int NodeId { get; }
        public NodeType Type { get; }
        public int Layer { get; }
        public int RunNumber { get; }

        public EncounterRequest(int nodeId,
            NodeType type,
            int layer,
            int runNumber)
        {
            NodeId = nodeId; Type = type; Layer = layer; RunNumber = runNumber;
        }
    }

    public readonly struct EncounterResult
    {
        public readonly EncounterOutcome Outcome;
        public EncounterResult(EncounterOutcome outcome) { Outcome = outcome; }

        public static EncounterResult Victory => new(EncounterOutcome.Victory);
        public static EncounterResult Defeat  => new(EncounterOutcome.Defeat);
    }
}
