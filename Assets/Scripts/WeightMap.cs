using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class WeightMap
{
    // Have a random number generator and based on it in very rare cases set a hex as water, and make the wieght maximum

    // In the rest of the cases set a random weight between 0 and 0.8 foe example

    // When there is water to cross it set the weight of couple of hexes that will act as bridges to low weight so that 
    // the pathfinder will use them
    private GameObject[] watterTiles;

    public void GenerateWeightMap(GameObject[,] grid, int numberOfWaterTiles, int maxLengthOfWaterBody = 3)
    {
        watterTiles = new GameObject[numberOfWaterTiles];
        int currectNumberOfWaterTiles = 0;
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                HexData hexData = grid[x, y].GetComponent<HexData>();
                if (hexData != null && hexData.Weight == 0f)
                {
                    float randomValue = Random.value;
                    if (randomValue < 0.05f && (currectNumberOfWaterTiles < numberOfWaterTiles)) // 5% chance to be water
                    {
                        Debug.Log("asd");
                        hexData.Weight = 1f; // Maximum weight for water
                        currectNumberOfWaterTiles++;
                        watterTiles[currectNumberOfWaterTiles - 1] = grid[x, y];
                        grid[x, y].GetComponentInChildren<Renderer>().material.color = Color.blue;
                    }
                    else
                    {
                        hexData.Weight = Random.Range(0f, 0.8f); // Random weight between 0 and 0.8
                        grid[x, y].GetComponentInChildren<Renderer>().material.color = Color.Lerp(Color.green, Color.red, hexData.Weight);
                    }
                }
            }
        }

        generateWaterBodies(maxLengthOfWaterBody);
    }


    // TODO: Figure out how to make the water body like a river, maybe add some variaty? Some bodies are like a lake, others are rivers?
    private void generateWaterBodies(int maxLengthOfWaterBody)
    {
        foreach (GameObject waterTile in watterTiles)
        {
            List<GameObject> waterBody = new();
            for (int i = 0; i < maxLengthOfWaterBody; i++)
            {
                List<GameObject> neighbours = waterTile.GetComponent<HexData>().getNeighbours();
                // Select random neighbour
                if (neighbours.Count == 0) break;

                GameObject randomNeightbour;

                do
                {
                    randomNeightbour = neighbours.ElementAt(Random.Range(0, neighbours.Count));
                } while (!randomNeightbour && !waterBody.Contains(randomNeightbour) && randomNeightbour.GetComponent<HexData>().Weight != 1);

                Debug.Log(waterBody.Count);

                waterBody.Add(randomNeightbour);
                randomNeightbour.GetComponent<HexData>().Weight = 1;
                randomNeightbour.GetComponentInChildren<Renderer>().material.color = Color.blue;

                continue;
            }
        }

    }
}
