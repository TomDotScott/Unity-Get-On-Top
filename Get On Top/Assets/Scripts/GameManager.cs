using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> spawnPositions;

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
                if (playerTwoPoints == maxPoints)
                {
                    GameOver();
                }
            }
        }
    }

    [SerializeField] private Player[] players;

    [SerializeField] private TextMeshProUGUI startCountDownText;
    [SerializeField] private float startCountDown;

    private bool gameStarted = false;
    private bool gameOver = false;

    [SerializeField] private TextMeshProUGUI gameTimerText;
    [SerializeField] private float gameTimer;

    [SerializeField] private List<GameObject> pickUps;
    [SerializeField] private float timeBetweenPickUpSpawning;
    private float pickupSpawnTimer;

    // Start is called before the first frame update
    void Start()
    {
        playerOnePointsUI.gameObject.SetActive(false);
        playerTwoPointsUI.gameObject.SetActive(false);
        gameTimerText.gameObject.SetActive(false);

        foreach(var player in players)
        {
            player.Frozen = true;
        }

        gameOverUI.SetActive(false);
        pickupSpawnTimer = timeBetweenPickUpSpawning;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStarted)
        {
            if (!gameOver)
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

                            player.Respawn(GetRandomPosition());
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

                            player.Respawn(GetRandomPosition());
                            break;
                    }
                }

                CountDown();

                AttemptToSpawnPickup();
            }
        }
        else
        {
            startCountDown -= Time.deltaTime;

            if (startCountDown < 0)
            {
                foreach (var player in players)
                {
                    player.Frozen = false;
                }

                playerOnePointsUI.gameObject.SetActive(true);
                playerTwoPointsUI.gameObject.SetActive(true);
                gameTimerText.gameObject.SetActive(true);
                startCountDownText.gameObject.SetActive(false);

                gameStarted = true;
            }
            else
            {
                startCountDownText.text = ((int)startCountDown).ToString();
            }

        }
    }

    private void CountDown()
    {
        gameTimer -= Time.deltaTime;

        // If the timer runs out, end the game
        if (gameTimer < 1)
        {
            GameOver();
        }

        // Convert seconds to minutes and seconds
        TimeSpan t = TimeSpan.FromSeconds(gameTimer);

        string timeRemaining = string.Format("{0:D2}:{1:D2}",
                t.Minutes,
                t.Seconds
        );

        gameTimerText.SetText(timeRemaining);
    }

    private void AttemptToSpawnPickup()
    {
        pickupSpawnTimer -= Time.deltaTime;

        if (pickupSpawnTimer <= 0)
        {
            Debug.Log("Attempting to spawn pickup");

            GameObject pickup = pickUps[UnityEngine.Random.Range(0, pickUps.Count)];

            Vector2 pickupPosition = GetRandomPosition();

            Instantiate(pickup, new Vector3(pickupPosition.x, pickupPosition.y, 0), Quaternion.identity);

            pickupSpawnTimer = timeBetweenPickUpSpawning;
        }
    }

    private void GameOver()
    {
        gameOver = true;

        // Freeze the players
        foreach (var player in players)
        {
            player.Frozen = true;
        }

        // Show the UI
        gameOverUI.SetActive(true);

        // Change the winning text and colour
        if (playerOnePoints > PlayerTwoPoints)
        {
            winnerText.text = "Player One Wins!";
            winnerText.color = new Color(255, 0, 0);
        }

        else if (playerTwoPoints > PlayerOnePoints)
        {
            winnerText.text = "Player Two Wins!";
            winnerText.color = new Color(0, 0, 255);
        }
        else
        {
            winnerText.text = "Draw!";
            winnerText.color = new Color(255, 255, 255);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private Vector2 GetRandomPosition()
    {
        return spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Count)].transform.position;
    }
}
