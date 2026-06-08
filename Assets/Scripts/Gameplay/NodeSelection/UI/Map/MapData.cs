using System.Collections.Generic;
using Gameplay.NodeSelection.UI.Nodes;

namespace Gameplay.NodeSelection.UI.Map
{
    public class MapData 
    {
        public IReadOnlyList<Node> Nodes { get; }
        public int LayerCount { get; }
        public int StartId { get; }
        public int BossId { get; }
 
        private readonly Dictionary<int, Node> _nodeById;
 
        public MapData(IReadOnlyList<Node> nodes,
            int layerCount,
            int startId,
            int bossId)
        {
            Nodes = nodes;
            LayerCount = layerCount;
            StartId = startId;
            BossId = bossId;
 
            _nodeById = new Dictionary<int, Node>(nodes.Count);
            foreach (var n in nodes) _nodeById[n.Id] = n;
        }
 
        public Node Get(int id) => _nodeById[id];
    }
}
