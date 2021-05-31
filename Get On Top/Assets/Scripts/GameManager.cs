using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerOnePointsUI;
    private int playerOnePoints = 0;

    public int PlayerOnePoints
    {
        get => playerOnePoints;
        set
        {
            playerOnePoints = value;
            playerOnePointsUI.text = playerOnePoints.ToString();
        }
    }

    [SerializeField] private TextMeshProUGUI playerTwoPointsUI;
    private int playerTwoPoints = 0;

    public int PlayerTwoPoints
    {
        get => playerTwoPoints;
        set
        {
            playerTwoPoints = value;
            playerTwoPointsUI.text = playerTwoPoints.ToString();
        }
    }

    [SerializeField] private Player[] players;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Update the scores
        foreach (var player in players)
        {
            Player.PlayerState currentPlayerState = player.GetState();
            switch (currentPlayerState)
            {
                case Player.PlayerState.squashed:
                    // Check if it's player 1 or player 2
                    switch (player.GetPlayerType())
                    {
                        case Player.PlayerType.playerOne:
                            PlayerTwoPoints++;
                            break;
                        case Player.PlayerType.playerTwo:
                            PlayerOnePoints++;
                            break;
                    }

                    player.Respawn();
                    break;

                case Player.PlayerState.dead:
                    // Check if it's player 1 or player 2
                    switch (player.GetPlayerType())
                    {
                        case Player.PlayerType.playerOne:
                            PlayerOnePoints--;
                            break;
                        case Player.PlayerType.playerTwo:
                            PlayerTwoPoints--;
                            break;
                    }

                    player.Respawn();
                    break;
            }
        }
    }
}
