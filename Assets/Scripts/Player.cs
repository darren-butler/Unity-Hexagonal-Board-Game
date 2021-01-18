using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// GameManager hold a list of instances of this type
// GameManager component instantiates prefabs with Player components and initializes their Number, Color etc
public class Player : MonoBehaviour
{
    public int Number { get; set; }
    public Color Colour { get; set; }


    public bool isTurn = false;
    public bool didRoll = false;
    public bool didMove = false;
    public bool isBot = false;

    public int score;
    public HexTile activeTile;
    public List<HexTile> validMoves;
    public GameObject target;

    private GameObject UI;
    private int roll = 0;
    private SoundController soundController;

    // Start is called before the first frame update
    void Start()
    {
        transform.gameObject.GetComponentInChildren<MeshRenderer>().material.color = Colour; // set player token visual colour, to the colour they were assigned
        soundController = GameObject.FindGameObjectWithTag("Music").GetComponent<SoundController>();

        gameObject.name = $"Player{Number}"; // give the gameobject a unique name for clarity in the hierarchy
    }

    // Update is called once per frame
    void Update()
    {
        if(UI == null) // this was done in update because player components were being intialized before the UI
        {
            UI = GameObject.FindGameObjectWithTag("UI");
        }
    }

    private void InitTurn()
    {
        // setup the player turn indicator in the bottom left corner of the UI i.e. "P1"
        transform.gameObject.GetComponentInParent<GameManager>().playerIndicator.GetComponentInChildren<Text>().text = $"P{Number + 1}";
        transform.gameObject.GetComponentInParent<GameManager>().playerIndicator.GetComponentInChildren<Text>().color = Colour;

        // init bool flags for this turn
        isTurn = true;
        didRoll = false;
        didMove = false;
        roll = 0;

        // Get a handle on the UI and allow the player to click the roll button
        UI = GameObject.FindGameObjectWithTag("UI");
        UI.SetActive(true);
        UI.GetComponentInChildren<Button>().interactable = true;
        UI.GetComponentInChildren<Dice>().roll = 0;
        UI.GetComponentInChildren<Dice>().gameObject.GetComponentInChildren<Text>().text = "-";

        // clear our current mouse target
        target = null;
    }


    // This coroutine is started by the TurnSystem component of the GameManager
    // I.e. for each player, do a turn
    // This method contains everything involved in a single turn for a player
    public IEnumerator Turn()
    {
        InitTurn(); // Turn initialization

        if(isBot) // if the player is a bot, roll and move for them
        {
            yield return new WaitForSeconds(0.5f); // small delay so bot movements aren't instant

            roll = Random.Range(1, 4); // instead of using the roll button and Dice component, bots just get a random number from here
            UI.GetComponentInChildren<Dice>().gameObject.GetComponentInChildren<Text>().text = $"{roll}";

            // calulate valid moves 
            List<HexTile> validMoves = HighlightMoves();
            DeHighlightMoves(); // remove the highlight of these moves (bots don't need to be visually shown where they can move)

            // optimal move returns an integer index for the best move from the valid moves list, 
            MoveTile(validMoves[OptimalMove(validMoves)]); // so pass the tile at this index to the MoveTile method

            CLaimResources(activeTile);

            didMove = true;
        }
        else // if not a bot player - start these two coroutines one after the other, wait for player to roll, wait for player to move
        {

            yield return WaitForRoll(); 

            yield return WaitForMove();
        }
    }


    // This couroutine will run until the player clicks the roll dice button
    private IEnumerator WaitForRoll()
    {
        bool done = false;
        while (!done)
        {
            if (UI.GetComponentInChildren<Dice>().roll > 0)
            {
                roll = UI.GetComponentInChildren<Dice>().roll;
                UI.GetComponentInChildren<Button>().interactable = false;
                //Debug.Log($"player {Number} rolled a {roll}");
                validMoves = HighlightMoves(); // once they roll, highlight their valid moves
                done = true;
            }
            yield return null;
        }
    }


    // This coroutine will run until the player picks a valid move 
    private IEnumerator WaitForMove()
    {
        bool done = false;
        while (!done)
        {
            if (Input.GetMouseButtonDown(0) && target != null)
            {
                if (IsValidMove(target.GetComponent<HexTile>())) // check if the tile we clicked is a valid move (i.e. it is within our roll range)
                {
                    DeHighlightMoves(); // clear the tile highlight

                    // move our player token and claim the resources on that tile
                    MoveTile(target.GetComponent<HexTile>());
                    CLaimResources(activeTile);
                    didMove = true;
                    done = true;
                }
            }

            yield return null;
        }
    }


    // This method is where the player bot "AI" is implemented
    // all it does is iterates over the possible moves
    // and in order of resource value, if one of our possible moves is of the best resource type, we return that tiles index
    private int OptimalMove(List<HexTile> moves)
    {
        
        for(int i = 0; i < moves.Count; i++) // iterate over possible moves
        {
            if(moves[i].resource == Resource.STONE) // if we find a stone hex tile (the most valuable) ->  then move to that tile
            {
                return i;
            }
        }

        for (int i = 0; i < moves.Count; i++) // we have no stone tiles to move to, check if there are any wood tiles (second best)
        {
            if (moves[i].resource == Resource.WOOD)
            {
                return i;
            }
        }

        for (int i = 0; i < moves.Count; i++) // we have no stone or wood tiles to move to, check if there are any clay tiles (third best)
        {
            if (moves[i].resource == Resource.CLAY)
            {
                return i;
            }
        }

        return Random.Range(0, moves.Count); // finally, if there are only desert tiles (worth nothing), just pick a random one to move to
    }


    private void CLaimResources(HexTile hexTile)
    {
        // Lookup the value of the resource type on hexTile from our Util dictionary
        Util.ResourceTable.TryGetValue(hexTile.resource, out int resourceMultiplier);

        // Sound was implemented late in development, as such there are some bad practices with it
        SoundController soundController = GameObject.FindGameObjectWithTag("Music").GetComponent<SoundController>(); // try to find the soundcontroller
        if(soundController)
        {
            soundController.PlayResourceSound(hexTile.resource); // if we found it, play the sound associated with the resource type 
        }
         
        score += (hexTile.quantity * resourceMultiplier); // update our score
        hexTile.quantity = 0; // remove the resource from the tile
        hexTile.resource = Resource.DESERT; // set it's type to desert (barren)
    }


    // Check if the tile we want to move is valid
    private bool IsValidMove(HexTile attemptedMove)
    {
        if(attemptedMove.isActive) // is there a player currently on the tile?
        {
            return false;
        }
        else if (! validMoves.Contains(attemptedMove)) // is the tile we want to move in our list of neighbour tiles?
        {
            //Debug.Log("invalid move!");
            return false;
        }
        else
        {
            return true;
        }
    }

    
    // Return a list of neighbouring tiles we can move to based on our current position, and our roll
    private List<HexTile> HighlightMoves()
    {
        List<HexTile> neighbours = activeTile.FindNeighbours(roll);

        foreach (HexTile neighbour in neighbours)
        {
            if(!neighbour.isActive)
            {
                neighbour.gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.black;
            }
        }

        return neighbours;
    }


    // Clear the highlight applied by the HighLightMoves() method, after the player has made their choice
    private void DeHighlightMoves()
    {
        List<HexTile> neighbours = activeTile.FindNeighbours(roll);

        foreach (HexTile neighbour in neighbours)
        {
            if (!neighbour.isActive)
            {
                neighbour.gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
            }
        }
    }


    // This method moves our current tile to the new one
    public void MoveTile(HexTile hexTile)
    {
        if (activeTile)
        {
            activeTile.GetComponent<HexTile>().isActive = false; // deactivate our current tile (so other players could move to it later)
            hexTile.transform.gameObject.GetComponentInChildren<SpriteRenderer>().sprite = Util.ResourceSprites[0]; // set the tiles visual sprite to be desert (barren)
        }

        hexTile.isActive = true; // set our new tile to be active
        activeTile = hexTile; // set our current tile to be the new tile
        transform.position = hexTile.transform.position; // move our piece to the new tile

        soundController = GameObject.FindGameObjectWithTag("Music").GetComponent<SoundController>(); // play the sound associated with the resource type of that tile
        if (soundController)
        {
            soundController.PlayMovePieceSound();
        }
    }
}
