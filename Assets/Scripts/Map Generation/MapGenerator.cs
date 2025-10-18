using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
    GameObject[,] hexGrid;


    //.....
    // Path Finding Values
    //.....
    [Space(5), Header("Path Finidng Values")]
    [SerializeField] private Vector2 startHex = new Vector2(0, 0);
    [SerializeField] private Vector2 endHex = new Vector2(9, 9);
    [Range (0, 5)]public int numberOfPaths;
    public List<GameObject> path = new();
    private PathGenerator pathGenerator;


    //.....
    // Water & Weight Map Values
    //.....
    [Header ("Water Values")]
    public int numberOfWaterTiles;
    public int maxLengthOfWaterBody;

    private WeightMap weightMap;

    void Start()
    {
        pathGenerator = new PathGenerator();
        weightMap = new WeightMap();

        GenerateGrid();
    }

    void GenerateGrid()
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
                hex.GetComponent<HexData>().grid = hexGrid;
                hex.name = $"Hex_{x}_{y}";
                hexGrid[x, y] = hex;
            }
        }

        weightMap.GenerateWeightMap(hexGrid, numberOfWaterTiles, maxLengthOfWaterBody);
        Debug.Log ($"Generating path from {hexGrid[(int)startHex.x, (int)startHex.y]} to {hexGrid[(int)endHex.x, (int)endHex.y]}");
        //path = pathGenerator.GeneratePath(hexGrid, hexGrid[(int)startHex.x, (int)startHex.y], hexGrid[(int)endHex.x, (int)endHex.y], numberOfPaths);
    }

    void clearGrid()
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
