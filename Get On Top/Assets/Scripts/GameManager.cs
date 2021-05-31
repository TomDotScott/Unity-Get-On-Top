using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float maxPoints;

    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TextMeshProUGUI winnerText;

    [SerializeField] private TextMeshProUGUI playerOnePointsUI;
    private int playerOnePoints = 0;

    public int PlayerOnePoints
    {
        get => playerOnePoints;
        set
        {
            playerOnePoints = value;
            playerOnePointsUI.text = playerOnePoints.ToString();

            if (!gameOver)
            {
                if (playerOnePoints == maxPoints)
                {
                    GameOver();
                }
            }
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

            if (!gameOver)
            {
                if(playerTwoPoints == maxPoints)
                {
                    GameOver();
                }
            }
        }
    }

    [SerializeField] private Player[] players;

    private bool gameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        gameOverUI.SetActive(false);
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

    private void GameOver()
    {
        gameOver = true;

        // Freeze the players
        foreach(var player in players)
        {
            player.Frozen = true;
        }

        // Show the UI
        gameOverUI.SetActive(true);

        // Change the winning text and colour
        if(playerOnePoints == maxPoints)
        {
            winnerText.text = "Player One Wins!";
            winnerText.color = new Color(255, 0, 0);
        }

        if (playerTwoPoints == maxPoints)
        {
            winnerText.text = "Player Two Wins!";
            winnerText.color = new Color(0, 0, 255);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
