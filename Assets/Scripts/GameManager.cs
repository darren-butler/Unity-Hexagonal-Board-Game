using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;

// Each game has a GameManager which has this as its main component
// This component handles board, player & turn system initialization.
public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] public GameObject playerIndicator;

    [SerializeField] private GameObject playerPrefab = null;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject gameOverBanner;
    [SerializeField] private int numberOfPlayers = 2;
    [SerializeField] private int winningScore = 75;
    [SerializeField] private bool isBotGame = false;

    public List<GameObject> players;
    public bool gameIsWon = false;

    private HexGrid hexGrid;
    private TurnSystem turnSystem;
    private bool ready = false;

    // Start is called before the first frame update
    void Start()
    {
        GameObject player;
        startButton.SetActive(false);
        if(PhotonNetwork.IsConnected)
        {
            if(PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                // setup players based on number of players?
            }
            if(NetworkedPlayer.LocalPlayerInstance ==  null)
            {
                // instantiate the networked prefabs
                player = PhotonNetwork.Instantiate(playerPrefab.name, transform.position, Quaternion.identity, 0);
            }
            if(PhotonNetwork.IsMasterClient) 
            {
                startButton.SetActive(true);
                turnSystem = transform.gameObject.GetComponent<TurnSystem>();
                InitHexGrid();
            }
        }
        else
        {
            turnSystem = transform.gameObject.GetComponent<TurnSystem>();
            InitHexGrid();
            InitPlayers();
            ready = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(ready) // make sure to only start this coroutine once, and only when players are initialized
        {
            StartCoroutine(turnSystem.TurnRoutine(players, winningScore)); // once players and board is initialized, start the turnsystem coroutine
            ready = false;
        }

        if(gameIsWon) // when the game is over, stop all coroutines and show winning player banner
        {
            StopAllCoroutines();
            gameOverBanner.SetActive(true);
            gameOverBanner.gameObject.GetComponentInChildren<Text>().text = $"Player {turnSystem.winner.Number + 1} Wins!";
            gameOverBanner.gameObject.GetComponentInChildren<Text>().color = turnSystem.winner.Colour;
            gameIsWon = !gameIsWon;
        }
    }

    // Sets up player gameobjects from prefab, and initialize their Player component
    private void InitPlayers()
    {
        players = new List<GameObject>();

        for (int i = 0; i < numberOfPlayers; i++)
        {
            GameObject player = Instantiate(playerPrefab, transform);
            player.GetComponent<Player>().Number = i;
            player.GetComponent<Player>().Colour = Util.Colours[i];
            player.GetComponent<Player>().MoveTile(hexGrid.hexTiles[Util.BoardVertices[i]]); // assign the player a starter tile from one of the 4 corners of the board
            player.GetComponent<Player>().isBot = false;

            if (isBotGame && i != 0) // if the game has been set as a player vs bots game, then player[0] is not a bot, and all other players are to be tagged as bots
            {
                player.GetComponent<Player>().isBot = true;
            }

            players.Add(player);
        }
    }


    // Call two HexGrid component methods to setup the hex grid and randomly distribute resources across them
    private void InitHexGrid()
    {
        hexGrid = GameObject.FindGameObjectWithTag("Board").GetComponent<HexGrid>();

        if (hexGrid)
        {
            hexGrid.GenerateHexTiles_MK2();
            hexGrid.DistributeResources();
        }
        else
        {
            Debug.LogError("HexGrid Component is null");
        }
    }

    public void StartOnlineGame()
    {
        numberOfPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        InitPlayers();
        ready = true;
        Debug.Log($"players connected: {PhotonNetwork.CurrentRoom.PlayerCount}");
        startButton.SetActive(false);
    }
}
