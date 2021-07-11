using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class QuickStartRoomController : MonoBehaviourPunCallbacks
{
    [SerializeField] private int multiplayerSceneIndex;

    public override void OnEnable()
    {
        // Register the object for the callback interfaces
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined the room");
        StartGame();
    }

    private void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Starting the game");

            // Sync all players to the level that we want to load
            PhotonNetwork.LoadLevel(multiplayerSceneIndex);
        }
    }
}
