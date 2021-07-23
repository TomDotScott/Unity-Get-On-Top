using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManagerGOT : NetworkManager
{
    [SerializeField] private Transform playerOneSpawn;
    [SerializeField] private Transform playerTwoSpawn;
    [SerializeField] private CameraMovement gameCamera;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        // Work out the player start position
        Transform start = numPlayers == 0 ? playerOneSpawn : playerTwoSpawn;

        // Instantiate the gameobject to show on the client end
        GameObject player = Instantiate(playerPrefab, start.position, start.rotation);

        gameCamera.AddPlayerToTrack(player);

        // Tell the server that it should add a player to the other clients
        NetworkServer.AddPlayerForConnection(conn, player);
    }


    public override void OnServerDisconnect(NetworkConnection conn)
    {
        
        // Call the base functionality
        base.OnServerDisconnect(conn);
    }
}
