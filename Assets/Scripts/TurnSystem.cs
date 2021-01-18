using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GameManager gameobject has an instance of this component
public class TurnSystem : MonoBehaviour
{
    [SerializeField] private int round;
    public Player winner;

    // This coroutine is started by the game manager and runs until a player score get;s over the winningScore threshold
    public IEnumerator TurnRoutine(List<GameObject> playerGameObjects, int winningScore)
    {
        round = 1;

        while (true)
        {
            //Debug.Log("round " + round + " started");

            foreach (GameObject playerGameObject in playerGameObjects) // foreach player, start the Turn() coroutine, which starts the WaitForRoll() & WaitForMove() coroutines
            {
                yield return StartCoroutine(playerGameObject.GetComponent<Player>().Turn()); // wait for the players turn to be completed

                if(playerGameObject.GetComponent<Player>().score >= winningScore) // if the players score is above the winning threshold
                {
                    // End the game
                    Debug.Log($"Player {playerGameObject.GetComponent<Player>().Number} wins!");
                    winner = playerGameObject.GetComponent<Player>();
                    transform.gameObject.GetComponent<GameManager>().gameIsWon = true;
                    yield return null;
                }
            }
            round++;
        }
    }
}
