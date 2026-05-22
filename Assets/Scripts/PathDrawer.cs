using System.Collections.Generic;
using UnityEngine;

public class PathDrawer : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;

    [SerializeField]
    private Waypoint currentWaypoint;

    public void CalculatePath(Waypoint destination)
    {
        List<Waypoint> path = FindPath(currentWaypoint, destination);

        if (path == null)
        {
            Debug.Log("No se encontró camino");
            return;
        }

        DrawPath(path);

        currentWaypoint = destination;
    }

    List<Waypoint> FindPath(Waypoint start, Waypoint goal)
    {
        Queue<Waypoint> frontier = new Queue<Waypoint>();

        Dictionary<Waypoint, Waypoint> cameFrom =
            new Dictionary<Waypoint, Waypoint>();

        frontier.Enqueue(start);

        cameFrom[start] = null;

        while (frontier.Count > 0)
        {
            Waypoint current = frontier.Dequeue();

            if (current == goal)
                break;

            foreach (Waypoint neighbor in current.neighbors)
            {
                if (!cameFrom.ContainsKey(neighbor))
                {
                    frontier.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }

        if (!cameFrom.ContainsKey(goal))
            return null;

        List<Waypoint> path = new List<Waypoint>();

        Waypoint temp = goal;

        while (temp != null)
        {
            path.Add(temp);
            temp = cameFrom[temp];
        }

        path.Reverse();

        return path;
    }

    void DrawPath(List<Waypoint> path)
    {
        lineRenderer.positionCount = path.Count;

        float totalDistance = 0f;

        for (int i = 0; i < path.Count; i++)
        {
            lineRenderer.SetPosition(
                i,
                path[i].transform.position
            );

            if (i > 0)
            {
                totalDistance += Vector3.Distance(
                    path[i - 1].transform.position,
                    path[i].transform.position
                );
            }
        }

        Debug.Log("Distancia total: " + totalDistance);
    }
}