using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public List<Waypoint> neighbors = new List<Waypoint>();

    private void Awake()
    {
        foreach (var neighbor in neighbors)
            if (neighbor != null && !neighbor.neighbors.Contains(this))
                neighbor.neighbors.Add(this);
    }
}

