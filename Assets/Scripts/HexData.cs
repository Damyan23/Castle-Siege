using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexData : MonoBehaviour
{
    [HideInInspector] public GameObject[,] grid;

    [SerializeField, Range(0f, 1f)]
    private float weight = 0f;

    public float Weight
    {
        get => weight;
        set => weight = Mathf.Clamp(value, 0f, 1f);
    }

    public List<GameObject> getNeighbours()
    {
        List<GameObject> neighbours = new();

        Vector2Int gridPos = FindGridPosition(gameObject, grid);
        if (gridPos.x == -1)
        {
            Debug.LogError($"GameObject {gameObject.name} not found in grid!");
            return neighbours;
        }

        bool isEvenColumn = gridPos.x % 2 == 0;

        // Odd-r layout (flat-topped hexes, row offset)
        int[,] evenRowOffsets = new int[,]
        {
            {+1, 0},   // Right
            {0, -1},   // UpLeft
            {-1, -1},  // UpRight
            {-1, 0},   // Left
            {-1, +1},  // DownLeft
            {0, +1}    // DownRight
        };

        int[,] oddRowOffsets = new int[,]
        {
            {+1, 0},   // Right
            {+1, -1},  // UpRight
            {0, -1},   // UpLeft
            {-1, 0},   // Left
            {0, +1},   // DownLeft
            {+1, +1}   // DownRight
        };



        bool isEvenRow = gridPos.y % 2 == 0;
        int[,] offsets = isEvenRow ? evenRowOffsets : oddRowOffsets;


        for (int i = 0; i < offsets.GetLength(0); i++)
        {
            int nx = gridPos.x + offsets[i, 0];
            int ny = gridPos.y + offsets[i, 1];

            if (nx >= 0 && ny >= 0 && nx < grid.GetLength(0) && ny < grid.GetLength(1))
            {
                if (grid[nx, ny] != null)
                {
                    neighbours.Add(grid[nx, ny]);
                }
                else continue;
            }
        }

        return neighbours;
    }

    private Vector2Int FindGridPosition(GameObject hex, GameObject[,] grid)
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y] == hex)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return new Vector2Int(-1, -1); // Not found
    }
}
