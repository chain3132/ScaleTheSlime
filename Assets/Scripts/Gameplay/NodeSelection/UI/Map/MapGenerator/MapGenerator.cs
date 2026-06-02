using System;
using System.Collections.Generic;
using Gameplay.NodeSelection.UI.Nodes;
using Gameplay.NodeSelection.UI.Nodes.Enums;

namespace Gameplay.NodeSelection.UI.Map.MapGenerator
{
    public class MapGenerator
    {
        private readonly MapGenerationConfig _config;
        private readonly Random _rng;

        public MapGenerator(MapGenerationConfig config, int seed)
        {
            _config = config;
            _rng = new Random(seed);
        }

        public MapData Generate()
        {
            var cfg = _config;
            var layers = new List<List<Node>>();
            var all = new List<Node>();
            int nextId = 0;

            //Create the nodes per One layer.
            for (int layer = 0; layer < cfg.LayerCount; layer++)
            {
                int count = (layer == 0 || layer == cfg.LayerCount - 1)
                    ? 1 // start & boss = 1 node
                    : _rng.Next(cfg.MinNodesPerLayer, cfg.MaxNodesPerLayer + 1);

                var current = new List<Node>(count);
                for (int i = 0; i < count; i++)
                {
                    var node = new Node(nextId++, layer, i);
                    current.Add(node);
                    all.Add(node);
                }

                layers.Add(current);
            }

            // Draw edges from each layer
            for (int layer = 0; layer < layers.Count - 1; layer++)
                ConnectLayers(layers[layer], layers[layer + 1]);

            // set node type
            foreach (var node in all)
                node.Type = PickType(node, cfg);

            return new MapData(all, cfg.LayerCount, layers[0][0].Id, layers[^1][0].Id);
        }

        private void ConnectLayers(List<Node> fromNodes, List<Node> toNodes)
        {
            foreach (var node in fromNodes)
            {
                int center = ProjectIndex(node.IndexInLayer, fromNodes.Count, toNodes.Count);
                // tell 
                int wanted = Math.Min(_rng.Next(1, _config.MaxOutgoingEdges + 1), toNodes.Count);

                var picked = new SortedSet<int> { center };
                for (int step = 1; picked.Count < wanted && step <= toNodes.Count; step++)
                {
                    if (center - step >= 0) picked.Add(center - step);
                    if (picked.Count < wanted && center + step < toNodes.Count) picked.Add(center + step);
                }

                foreach (int idx in picked)
                {
                    node.NextIds.Add(toNodes[idx].Id);
                }
            }

            
            for (int t = 0; t < toNodes.Count; t++)
            {
                if (HasIncoming(fromNodes, toNodes[t].Id)) continue;
                int src = ProjectIndex(t, toNodes.Count, fromNodes.Count);
                fromNodes[src].NextIds.Add(toNodes[t].Id);
            }
        }

        private int ProjectIndex(int index, int fromCount, int toCount)
        {
            if (fromCount <= 1 || toCount <= 1) return toCount / 2;
            float t = index / (float)(fromCount - 1);
            return (int)Math.Round(t * (toCount - 1), MidpointRounding.AwayFromZero);
        }

        private bool HasIncoming(List<Node> from, int targetId)
        {
            foreach (var n in from)
                if (n.NextIds.Contains(targetId))
                    return true;
            return false;
        }

        private NodeType PickType(Node node, MapGenerationConfig cfg)
        {
            if (node.Layer == 0) return NodeType.Start;
            if (node.Layer == cfg.LayerCount - 1) return NodeType.Boss;
            
            var pool = new (NodeType type, int weight)[]
            {
                (NodeType.Minor, 60),
                (NodeType.Elite, node.Layer >= cfg.NoEliteBeforeLayer ? 20 : 0),
                (NodeType.Rest, 10),
                (NodeType.Event, 7),
                (NodeType.Shop, 3),
            };

            int total = 0;
            foreach (var e in pool) total += e.weight;

            int roll = _rng.Next(0, total);
            foreach (var e in pool)
            {
                if (roll < e.weight) return e.type;
                roll -= e.weight;
            }

            return NodeType.Minor;
        }
    }
}
