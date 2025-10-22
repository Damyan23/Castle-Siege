using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class PathGenerator
{
    // TODO: Figure out how to use the heat map to generate a more interesting path.
    // TODO: Figure out how to generate several different paths. One way would be to incures the weight to maximum on most of the hexes
    // that are already in a path, this way there is a chance of usign that hex but its way too small, so that can create some interesting paths.
    // public List<GameObject> GeneratePath(GameObject[,] grid, GameObject start, GameObject end, int numberOfPaths = 1)
    // {
    //     Dictionary<GameObject, GameObject> cameFrom = new();
    //     Queue<GameObject> frontier = new();
    //     List<GameObject> path = new();
    //     List<List<GameObject>> allPaths = new();
    //
    //     frontier.Enqueue(start);
    //     cameFrom[start] = null;
    //
    //     while (frontier.Count > 0)
    //     {
    //         GameObject current = frontier.Dequeue();
    //         HexData currentHex = current.GetComponent<HexData>();
    //
    //         if (current == end)
    //             break;
    //
    //         foreach (GameObject neighbor in currentHex.getNeighbours())
    //         {
    //             if (!cameFrom.ContainsKey(neighbor))
    //             {
    //                 frontier.Enqueue(neighbor);
    //                 cameFrom[neighbor] = current;
    //             }
    //         }
    //     }
    //
    //     // Reconstruct path
    //     GameObject step = end;
    //     while (step != null)
    //     {
    //         path.Insert(0, step);
    //         var renderer = step.GetComponent<Renderer>();
    //         if (renderer != null)
    //             renderer.material.color = Color.yellow;
    //
    //         step = cameFrom.ContainsKey(step) ? cameFrom[step] : null;
    //     }
    //
    //
    //     return path;
    // }

    private List<HexData> openSet = new();
    private List<HexData> closedSet = new();
    public List<GameObject> GeneratePath(GameObject[,] grid, GameObject start, GameObject end, int numberOfPaths = 1)
    {
        if (start == null || end == null) return new List<GameObject>();

        HexData startHex = start.GetComponent<HexData>();
        HexData targetHex = end.GetComponent<HexData>();

        // Reset sets
        openSet.Clear();
        closedSet.Clear();

        // Initialize GCost for all nodes to "infinity" and clear parents
        int w = grid.GetLength(0);
        int h = grid.GetLength(1);
        for (int ix = 0; ix < w; ix++)
        {
            for (int iy = 0; iy < h; iy++)
            {
                if (grid[ix, iy] == null) continue;
                HexData hd = grid[ix, iy].GetComponent<HexData>();
                if (hd == null) continue;
                hd.GCost = Mathf.Infinity;
                hd.HCost = 0f;
                hd.Parent = null;
            }
        }

        startHex.GCost = 0f;
        startHex.HCost = getDistance(startHex, targetHex);
        openSet.Add(startHex);

        while (openSet.Count > 0)
        {
            HexData currentHex = openSet[0];

            // find node in openSet with lowest FCost (tie-breaker: lower HCost)
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentHex.FCost || (openSet[i].FCost == currentHex.FCost && openSet[i].HCost < currentHex.HCost))
                {
                    currentHex = openSet[i];
                }
            }

            openSet.Remove(currentHex);
            closedSet.Add(currentHex);

            if (currentHex == targetHex) return retracePath(startHex, targetHex);

            foreach (HexData neighbour in currentHex.GetNeighbours())
            {
                if (neighbour == null) continue;
                if (neighbour.Weight == 1f || closedSet.Contains(neighbour)) continue;

                float newMovementCostToNeighbour = currentHex.GCost + getDistance(currentHex, neighbour);

                if (newMovementCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
                {
                    neighbour.GCost = newMovementCostToNeighbour;
                    neighbour.HCost = getDistance(neighbour, targetHex);
                    neighbour.Parent = currentHex;

                    if (!openSet.Contains(neighbour)) openSet.Add(neighbour);
                }
            }
        }

        // No path found
        return new List<GameObject>();
    }

    private List<GameObject> retracePath(HexData startHex, HexData endHex)
    {
        List<GameObject> path = new();
        HexData currentHex = endHex;

        // Walk back through parents; stop if parent chain ends unexpectedly
        while (currentHex != null)
        {
            path.Add(currentHex.gameObject);
            if (currentHex == startHex) break;
            currentHex = currentHex.Parent;

            currentHex.gameObject.GetComponentInChildren<Renderer>().material.color = Color.white;
            currentHex.IsPath = true;
        }

        // If startHex was not reached, there is no valid path
        if (path.Count == 0 || path[path.Count - 1].GetComponent<HexData>() != startHex)
            return new List<GameObject>();

        path.Reverse();
        return path;
    }

    private float getDistance(HexData nodeA, HexData nodeB)
    {
        float distX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
        float distY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

        if (distX > distY) return 14 * distY + 10 * (distX - distY);
        else return 14 * distX + 10 * (distX - distY);
    }
}