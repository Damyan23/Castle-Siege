using System.Collections;
using System.Collections.Generic;
using System.IO;
using GDX;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[System.Serializable]
public class HexTile
{
    public GameObject insideTile;
    public GameObject outsideTile;
}

[System.Serializable]
public class MapHexTiles
{
    public HexTile startPath;
    public HexTile straightPath;
    public HexTile diagonalPath;
    public HexTile perpendiculatPath;
    public HexTile waterTile;
}

public class MapGenerator : MonoBehaviour
{
    //.....
    // Hex Map Values
    //.....
    [Header("Hex Map Values")]
    [SerializeField] private GameObject hexContainer;
    [SerializeField] private GameObject hexPrefab;
    [SerializeField] private int mapWidth = 10;
    [SerializeField] private int mapHeight = 10;
    private GameObject[,] hexGrid;


    //.....
    // Map Visualization Values
    //.....
    public MapHexTiles hexTilesPrefabs;

    //.....
    // Path Finding Values
    //.....
    [Space(5), Header("Path Finidng Values")]
    [SerializeField] private Vector2Int startHex = new(0, 0);
    [SerializeField] private Vector2Int endHex = new(9, 9);
    [Range(0, 5)] public int NumberOfPaths;
    public List<GameObject> Path = new();
    private PathGenerator pathGenerator;


    //.....
    // Water & Weight Map Values
    //.....
    [Header("Water Values")]
    public int NumberOfWaterTiles;
    public int MaxLengthOfWaterBody;
    private List<GameObject> waterBodies = new();

    private WeightMap weightMap;

    void Start()
    {
        pathGenerator = new PathGenerator();
        weightMap = new WeightMap();

        generateGrid();
    }

    public void RegenGrid()
    {
        clearGrid();
        generateGrid();
    }

    private void generateGrid()
    {
        clearGrid();
        hexGrid = new GameObject[mapWidth, mapHeight];

        float hexWidth = hexPrefab.GetComponentInChildren<Renderer>().bounds.size.x;
        float hexHeight = hexPrefab.GetComponentInChildren<Renderer>().bounds.size.z;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float xPos = x * hexWidth;
                float zPos = y * hexHeight * 0.75f;
                if (y % 2 == 1)
                {
                    xPos += hexWidth / 2;
                }

                Vector3 position = new Vector3(xPos, 0, zPos);

                GameObject hex = Instantiate(hexPrefab, position, Quaternion.identity, hexContainer.transform);
                HexData hexData = hex.GetComponent<HexData>();
                hexData.Grid = hexGrid;
                hexData.GridX = x;
                hexData.GridY = y;
                hex.name = $"Hex_{x}_{y}";
                hexGrid[x, y] = hex;
            }
        }

        weightMap.GenerateWeightMap(hexGrid, NumberOfWaterTiles, MaxLengthOfWaterBody);
        Path = pathGenerator.GeneratePath(hexGrid, hexGrid[(int)startHex.x, (int)startHex.y], hexGrid[(int)endHex.x, (int)endHex.y], NumberOfPaths);

        drawMap();
    }

    private void drawMap()
    {
        drawWaterTiles();
        drawPathTiles();
    }

    private void drawPathTiles()
    {
        for (int i = 0; i < Path.Count - 1; i++)
        {
            var currentTile = Path[i].GetComponent<HexData>();
            var nextTile = Path[i + 1].GetComponent<HexData>(); // next tile in the path
            HexData previousTile = null;
            
            if (i > 0) previousTile = Path[i - 1].GetComponent<HexData>();

            var directionToNextTile = currentTile.GetNeighbourDirection(nextTile);
            HexData.Direction? directionToPreviousTile = null;
            if (previousTile != null) directionToPreviousTile = currentTile.GetNeighbourDirection(previousTile);

            var straightPairs = new HashSet<(HexData.Direction?, HexData.Direction?)>
            {
                (HexData.Direction.Right,   HexData.Direction.Left),
                (HexData.Direction.UpRight, HexData.Direction.DownLeft),
                (HexData.Direction.UpLeft, HexData.Direction.DownRight),
            };

            
            if (straightPairs.Contains ((directionToNextTile, directionToPreviousTile)) || straightPairs.Contains ((directionToPreviousTile, directionToNextTile)))
            {
                // If not use a curve path
                spawnTilePrefab(currentTile.gameObject, hexTilesPrefabs.straightPath.insideTile);
            }

            var perpendicularPairs = new HashSet<(HexData.Direction?, HexData.Direction?)>
            {
                (HexData.Direction.Right,   HexData.Direction.UpLeft),
                (HexData.Direction.Right,   HexData.Direction.DownLeft),
                (HexData.Direction.Left,    HexData.Direction.UpRight),
                (HexData.Direction.Left,    HexData.Direction.DownRight),
                (HexData.Direction.UpRight, HexData.Direction.DownRight),
                (HexData.Direction.UpLeft,   HexData.Direction.DownLeft),
            };  

            if (perpendicularPairs.Contains ((directionToNextTile, directionToPreviousTile)) || perpendicularPairs.Contains ((directionToPreviousTile, directionToNextTile)))
            {
                // If not use a curve path
                spawnTilePrefab(currentTile.gameObject, hexTilesPrefabs.perpendiculatPath.insideTile);
            }
            
            var diagonalPairs = new HashSet<(HexData.Direction?, HexData.Direction?)>
            {
                (HexData.Direction.Right,    HexData.Direction.UpRight),
                (HexData.Direction.Right,    HexData.Direction.DownRight),
                (HexData.Direction.Left,     HexData.Direction.DownLeft),
                (HexData.Direction.Left,     HexData.Direction.DownRight),
                (HexData.Direction.UpRight,  HexData.Direction.UpLeft),
                (HexData.Direction.DownRight,HexData.Direction.DownRight)
            };

            if  (diagonalPairs.Contains((directionToNextTile, directionToPreviousTile)) || diagonalPairs.Contains ((directionToPreviousTile, directionToNextTile)))
            {
                spawnTilePrefab(currentTile.gameObject, hexTilesPrefabs.diagonalPath.insideTile);
            }
        }
    }

    private void drawWaterTiles()
    {
        foreach (var waterTile in weightMap.WaterTiles)
        {
            spawnTilePrefab(waterTile, hexTilesPrefabs.waterTile.insideTile);
        }
    }
    
    private void spawnTilePrefab (GameObject tileParent, GameObject tilePrefab)
    {
        tileParent.transform.GetChild(0).gameObject.SafeDestroy();
        Instantiate(tilePrefab, tileParent.transform);
    }

    private void clearGrid()
    {
        if (transform.childCount > 0)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}
