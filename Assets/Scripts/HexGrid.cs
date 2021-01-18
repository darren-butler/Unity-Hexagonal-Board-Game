using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// An instance of this component is held in a gameobject, parented to the GameManager gameobject
// This script is responsible for gernerating the board of hex tiles, and distributing resources to those tiles
// https://www.redblobgames.com/grids/hexagons/ - This site was invaluable in understanding how hex based grids work
// There are two version of the GenerateHexTiles() method, the first resulted in a co-ordinate system which became too difficult 
// to figure out how to calculate the neighbours of a given hex
public class HexGrid : MonoBehaviour
{
    [SerializeField] private GameObject hexTilePrefab = null;
    [SerializeField] public List<HexTile> hexTiles = new List<HexTile>();


    // This version of the method generates a hex grid, whith an overall hexagonal shape
    // This turned out to be quite difficult to figure a hexagons neighbours
    public void GenerateHexTiles()
    {

        int size = 9;
        int radius = 3;
        double angle = Math.PI / 180 * 30;
        float padding = 4.1f;
        int half = size / 2;
        Vector3 origin = new Vector3(0, 0, 0);

        double xOffset = Math.Cos(angle) * (radius - padding);
        double yOffset = Math.Sin(angle) * (radius - padding);

        for (int row = 0; row < size; row++)
        {
            //int cols = size - Math.Abs(row - half);

            for (int col = 0; col < size; col++)
            {
                float x = (float)(origin.x + xOffset * (col * 2 + 1 - size));
                float y = (float)(origin.y + yOffset * (row - half) * 3);
                //Debug.Log(x + " " + y);

                GameObject hexTileGameObject = Instantiate(hexTilePrefab, new Vector3(x, 0, y), Quaternion.identity, transform);
                hexTileGameObject.name = $"({row},{col})";

                hexTileGameObject.GetComponent<HexTile>().x = row;
                hexTileGameObject.GetComponent<HexTile>().y = col;
                hexTileGameObject.GetComponent<HexTile>().isActive= false;

                hexTiles.Add(hexTileGameObject.GetComponent<HexTile>()); // Add the HexTile Component to List<HexTile> maintained by the HexGrid component
            }
        }
    }


    // This version of the method generates a hex grid that is square based, i.e. 10*10 rows & columns of hextiles
    public void GenerateHexTiles_MK2()
    {
        int width = 10;
        int height = 10;
        int radius = 3;
        double angle = Math.PI / 180 * 30;
        float xPadding = 5.1f;
        float yPadding = 6.1f;

        double xOffset = Math.Cos(angle) * (radius - xPadding);
        double yOffset = Math.Sin(angle) * (radius - yPadding);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                double x = i * xOffset;

                if(j % 2 == 1)
                {
                    x += xOffset / 2f;
                }


                // Instantiate a hex prefab, and setup its HexTile component with its positional data
                GameObject hexTileGameObject = Instantiate(hexTilePrefab, new Vector3((float)x, 0, (float)(j * yOffset)), Quaternion.identity, transform);
                hexTileGameObject.name = $"({i},{j})";

                hexTileGameObject.GetComponent<HexTile>().x = i;
                hexTileGameObject.GetComponent<HexTile>().y = j;
                hexTileGameObject.GetComponent<HexTile>().isActive = false;
                hexTiles.Add(hexTileGameObject.GetComponent<HexTile>()); // Add the HexTile Component to List<HexTile> maintained by the HexGrid component
            }
        }
    }


    // 
    public void DistributeResources()
    {
        Array values = Enum.GetValues(typeof(Resource));
        System.Random random = new System.Random();

        for (int i = 0; i < hexTiles.Count; i++)
        {

            HexTile hexTile = hexTiles[i];

            if (Util.BoardVertices.Contains(i))
            {
                 // do nothing, this is a starter tile
            }
            else
            {
                hexTile.resource = (Resource)values.GetValue(random.Next(values.Length));
                //hexTile.quantity = UnityEngine.Random.Range(2, 10);
                hexTile.quantity = 1;

                SpriteRenderer hexResourceSpriteRenderer = hexTile.transform.gameObject.GetComponentInChildren<SpriteRenderer>();
                switch (hexTile.resource)
                {
                    case Resource.CLAY:
                        hexResourceSpriteRenderer.sprite = Util.ResourceSprites[1];
                        break;
                    case Resource.WOOD:
                        hexResourceSpriteRenderer.sprite = Util.ResourceSprites[2];
                        break;
                    case Resource.STONE:
                        hexResourceSpriteRenderer.sprite = Util.ResourceSprites[3];
                        break;
                    default:
                        hexResourceSpriteRenderer.sprite = Util.ResourceSprites[0];
                        break;

                }

            }
        }
    }
}
