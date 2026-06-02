using System;
using UnityEngine;

[Serializable]
public class MapGenerationConfig 
{
    public int LayerCount = 7;          
    public int MinNodesPerLayer = 2;    // middle layers: fewest nodes
    public int MaxNodesPerLayer = 3;    // middle layers: most nodes
    public int NoEliteBeforeLayer = 3;  
    public int MaxOutgoingEdges = 2;    // each node links to up to N nodes ahead
}
