using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PathGenerator
{
    // TODO: Figure out how to use the heat map to generate a more interesting path.
    // TODO: Figure out how to generate several different paths. One way would be to incures the weight to maximum on most of the hexes
    // that are already in a path, this way there is a chance of usign that hex but its way too small, so that can create some interesting paths.
    public List<GameObject> GeneratePath(GameObject[,] grid, GameObject start, GameObject end, int numberOfPaths = 1)
    {
        Dictionary<GameObject, GameObject> cameFrom = new();
        Queue<GameObject> frontier = new();
        List<GameObject> path = new();
        List<List<GameObject>> allPaths = new();

        frontier.Enqueue(start);
        cameFrom[start] = null;

        while (frontier.Count > 0)
        {
            GameObject current = frontier.Dequeue();
            HexData currentHex = current.GetComponent<HexData>();

            if (current == end)
                break;

            foreach (GameObject neighbor in currentHex.getNeighbours())
            {
                if (!cameFrom.ContainsKey(neighbor))
                {
                    frontier.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }

        // Reconstruct path
        GameObject step = end;
        while (step != null)
        {
            path.Insert(0, step);
            var renderer = step.GetComponent<Renderer>();
            if (renderer != null)
                renderer.material.color = Color.yellow;

            step = cameFrom.ContainsKey(step) ? cameFrom[step] : null;
        }


        return path;
    }
}