using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Simple enumeration for resource types, every hex tile has a resource
public enum Resource { DESERT, CLAY, WOOD, STONE }

// HexGrid component maintains a list of HexTile instances, each hex tile gameobject has a HexTile component
public class HexTile : MonoBehaviour
{
    public int x;
    public int y;
    public bool isActive = false; // is a player currently on this tile?
    public Resource resource;
    public int quantity;

    // Based on the position of (this) hextile instance, and the players roll, calculate a list of hextiles that neighbour this one
    public List<HexTile> FindNeighbours(int roll)
    {
        List<HexTile> neighbours = new List<HexTile>();
        List<GameObject> gameObjects = new List<GameObject>();

        GameObject neighbour;

        neighbour = GameObject.Find($"({x + roll},{y})"); // try and find a tile that is to the right (positive x, y) of this hextile, 
        if(neighbour) // if we found a tile
        {
            gameObjects.Add(neighbour); // add it to the list of neighbouring tiles
        }

        neighbour = GameObject.Find($"({x},{y - roll})"); // try and find a tile that is below (x, negative y)
        if (neighbour)
        {
            gameObjects.Add(neighbour);
        }

        neighbour = GameObject.Find($"({x- roll},{y})"); // try and find a tile that is to the left (negative x, y)
        if (neighbour)
        {
            gameObjects.Add(neighbour);
        }

        neighbour = GameObject.Find($"({x},{y + roll})"); // try and find a tile that is above (x, positive y)
        if (neighbour)
        {
            gameObjects.Add(neighbour);
        }

        foreach (GameObject gameObject in gameObjects) // iterate over the list of neighbour gameobjects we found from above
        {
            neighbours.Add(gameObject.GetComponent<HexTile>()); // add their HexTile components to a list to return from this method
        }

        return neighbours;
    }
}
