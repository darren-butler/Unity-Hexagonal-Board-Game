using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// This component is attatched the scoreboard UI object (recttransform)
public class HighScoreTable : MonoBehaviour
{
    [SerializeField] private GameObject entryPrefab;

    private GameObject scoreEntryContainer;
    private List<GameObject> players;
    private List<GameObject> entries;

    // Start is called before the first frame update
    void Start()
    {
        scoreEntryContainer = GameObject.Find("EntryContainer");
        entries = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if(players == null) //get a list of players
        {
            players = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().players;
            InitScoreBoard(); // setup the scoreboard for those players
        }
        else
        {
            UpdateScoreBoard(); 
        }
    }

    // Instantiate a score row prefab for each player in the game
    private void InitScoreBoard()
    {
        float yOffset = 20f;

        for (int i = 0; i < players.Count; i++)
        {
            Player player = players[i].GetComponent<Player>();
            GameObject entry = Instantiate(entryPrefab, scoreEntryContainer.transform);
            entries.Add(entry);
            RectTransform entryRectTransform = entry.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector2(0, i * -yOffset);
            entry.gameObject.SetActive(true);

            // init the text in the entries
            entry.transform.Find("position").GetComponent<Text>().text = $"{i+1}";
            entry.transform.Find("player").GetComponent<Text>().text = $"{player.Number + 1}";
            entry.transform.Find("resources").GetComponent<Text>().text = "0";
        }
    }

    // update the test in the score board entries
    private void UpdateScoreBoard()
    {
        List<Player> playerComponents = new List<Player>();
        foreach(GameObject go in players)
        {
            playerComponents.Add(go.GetComponent<Player>());
        }

        List<Player> sortedList = playerComponents.OrderByDescending(o=>o.score).ToList();

        for (int i = 0; i < sortedList.Count; i++)
        {
            entries[i].transform.Find("player").GetComponent<Text>().text = $"Player {sortedList[i].Number + 1}";
            entries[i].transform.Find("resources").GetComponent<Text>().text = $"{sortedList[i].score}";
        }
    }
}
