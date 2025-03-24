using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [HideInInspector] public GameObject cellIndicator;
    [SerializeField] private InputManager inputManager;
    [SerializeField]private Grid grid;
    [HideInInspector] public Vector3 cellSize;
    [SerializeField] private Material cellIndicatorMaterial;
    [SerializeField] private LayerMask groundLayer;
    private void Start()
    {
        inputManager = GameManager.instance.InputManager;
        cellSize = grid.cellSize;
        cellIndicator.transform.localScale = new Vector3(cellSize.x, 1, cellSize.z);
    }

    private void Update()
    {
        // Get the mouse position in the world
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();

        // Snap the mouse position to the grid
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        Vector3 snappedPosition = grid.CellToWorld(gridPosition);

        // Adjust the snapped position to the terrain height
        snappedPosition.y = GetTerrainHeight(snappedPosition);

        // Set the cell indicator's position to the adjusted snapped position
        cellIndicator.transform.position = snappedPosition;

        // Add a small vertical offset to keep the indicator visible above the terrain
        cellIndicator.transform.position += new Vector3(0, 0.05f, 0);
    }

    private float GetTerrainHeight(Vector3 position)
    {
        if (Terrain.activeTerrain != null)
        {
            return Terrain.activeTerrain.SampleHeight(position);
        }

        Ray ray = new Ray(position + Vector3.up * 100f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 200f, groundLayer))
        {
            return hit.point.y;
        }

        return position.y;
    }



    public void UpdateCellIndicatorColor(bool canPlace)
    {
        if (cellIndicatorMaterial != null)
        {
            cellIndicatorMaterial.color = canPlace ? Color.white : Color.red;
        }
    }
}
