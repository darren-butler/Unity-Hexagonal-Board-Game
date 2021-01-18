using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    // List for setting player colours
    public static readonly List<Color> Colours = new List<Color>()
    {
        Color.blue,
        Color.red,
        Color.green,
        Color.magenta,
    };

    // list that holds the list indices of the 4 corner tiles of the board, used for setting player start points
    public static readonly List<int> BoardVertices = new List<int>()
    {
        0,
        9,
        90,
        99
    };

    // Resource type mapped to its score value
    public static readonly Dictionary<Resource, int> ResourceTable = new Dictionary<Resource, int>
    {
        {Resource.DESERT, 0},
        {Resource.CLAY, 2},
        {Resource.WOOD, 4},
        {Resource.STONE, 10},
    };

    // list of sprites, used for setting a tiles sprite based on its resource type
    public static readonly List<Sprite> ResourceSprites = new List<Sprite>()
    {
        Resources.Load("desert", typeof (Sprite)) as Sprite,
        Resources.Load("clay", typeof (Sprite)) as Sprite,
        Resources.Load("wood", typeof (Sprite)) as Sprite,
        Resources.Load("stone", typeof (Sprite)) as Sprite,
    };
}
