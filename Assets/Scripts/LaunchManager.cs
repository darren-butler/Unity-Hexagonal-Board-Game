using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;

// This component was an attempt at setting up networked multiplayer using photon, I could not get this working
// The component was then used as a scene manager of sorts,
// The buttons in the main menu, call methods in this component to laucnh specific game scenes
public class LaunchManager : MonoBehaviourPunCallbacks
{
    byte maxPlayersPerRoom = 4;
    bool isConnecting;
    [SerializeField] Text feedbackText;
    [SerializeField] Input playerNameInput;

    private string gameVersion = "1";

    private void Awake()
    {
        if(feedbackText)
        {
            feedbackText.text = "";
        }
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void ConnectSinglePlayer()
    {
        SceneManager.LoadScene("Game");
    }

    public void StartBotGame()
    {
        // the bot game scene is a clone of the origional game scene but with a boolean isBotGame set to true
        // This was done becuase bot gameplay was a late addition, and I did not have time to properly setup GameMaster prefab init 
        // with things like number of players & bots set from the main menu
        SceneManager.LoadScene("BotGame"); 
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ConnectMultiplayer()
    {
        feedbackText.text = "";
        isConnecting = true;
        PhotonNetwork.NickName = "testNickName";

        if (PhotonNetwork.IsConnected)
        {
            feedbackText.text += "\nJoining  Room...";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            feedbackText.text += "\nConnecting to network...";
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        if(isConnecting)
        {
            feedbackText.text += "\nConnected to master, joining room...";
            PhotonNetwork.JoinRandomRoom();

        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        feedbackText.text += "\nFailed to join random room...";
        feedbackText.text += "\nCreating a new room...";
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        feedbackText.text += "\nDisconnected: " + cause + "...";
        isConnecting = false;
    }

    public override void OnJoinedRoom()
    {
        feedbackText.text += "\nSuccessfully joined room, starting game...";
        feedbackText.text += $"\nThere are {PhotonNetwork.CurrentRoom.PlayerCount} other players...";
        PhotonNetwork.LoadLevel(1);

    }


}
