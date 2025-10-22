using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Il2Cpp;
using UnityEngine;

public class HexData : MonoBehaviour
{
    public enum Direction
    {
        Right,
        UpRight,
        UpLeft,
        Left,
        DownLeft,
        DownRight
    }
    [HideInInspector] public int GridX;
    [HideInInspector] public int GridY;
    [HideInInspector] public GameObject[,] Grid;

    [HideInInspector] public HexData Parent;

    [SerializeField, Range(0f, 1f)]
    private float weight = 0f;

    public float Weight
    {
        get => weight;
        set => weight = Mathf.Clamp(value, 0f, 1f);
    }

    public float GCost = 0;
    public float HCost = 0;

    public float FCost
    {
        get { return (GCost + HCost) * weight; }
    }


    [HideInInspector] public bool IsPath;
    [HideInInspector] public bool IsWater
    {
        get
        {
            return weight == 1;
        }
    }


    public List<HexData> GetNeighbours()
    {
        List<HexData> neighbours = new();

        if (GridX == -1)
        {
            Debug.LogError($"GameObject {gameObject.name} not found in grid!");
            return neighbours;
        }

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



        bool isEvenRow = GridY % 2 == 0;
        int[,] offsets = isEvenRow ? evenRowOffsets : oddRowOffsets;


        for (int i = 0; i < offsets.GetLength(0); i++)
        {
            int nx = GridX + offsets[i, 0];
            int ny = GridY + offsets[i, 1];

            if (nx >= 0 && ny >= 0 && nx < Grid.GetLength(0) && ny < Grid.GetLength(1))
            {
                if (Grid[nx, ny] != null)
                {
                    neighbours.Add(Grid[nx, ny].GetComponent<HexData>());
                }
                else continue;
            }
        }

        return neighbours;
    }

    public Direction GetNeighbourDirection(HexData neighbour)
    {
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

        bool isEvenRow = GridY % 2 == 0;
        int[,] offsets = isEvenRow ? evenRowOffsets : oddRowOffsets;

        int distX = neighbour.GridX -GridX;
        int distY = neighbour.GridY - GridY;

        for (int i = 0; i < offsets.GetLength(0); i++)
        {
            int nx = offsets[i, 0];
            int ny = offsets[i, 1];

            if (distX == nx && distY == ny)
            {
                return (Direction)i;
            }
        }

        return Direction.Right;
    }
}
