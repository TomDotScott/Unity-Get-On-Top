using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum PlayerState
    {
        walking,
        jumping,
        dashing,
        squashed,
        dead
    }

    private PlayerState playerState;

    public enum PlayerType
    {
        playerOne,
        playerTwo
    }

    [SerializeField] private PlayerType playerType;
    private string hztlMovementAxis = "Horizontal";
    private string jumpMovementAxis = "Jump";
    private string dashMovementAxis = "Dash";

    [SerializeField] private float movementSpeed;
    [SerializeField] private float dashSpeed;

    private float speed;

    [SerializeField] private float jumpHeight;

    private Rigidbody2D rb;

    private bool canDoubleJump = false;

    private float doubleJumpTimer;
    [SerializeField] private float timeBetweenJumps;

    private float dashTimer;
    [SerializeField] private float dashDuration;

    [SerializeField] private float gravityScale;
    [SerializeField] private float fastFallGravityScale;

    public bool Frozen = false;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
        speed = movementSpeed;

        hztlMovementAxis += playerType == PlayerType.playerOne ? "PlayerOne" : "PlayerTwo";
        jumpMovementAxis += playerType == PlayerType.playerOne ? "PlayerOne" : "PlayerTwo";
        dashMovementAxis += playerType == PlayerType.playerOne ? "PlayerOne" : "PlayerTwo";
    }

    void Update()
    {
        if (!Frozen)
        {
            float movementAxis = Input.GetAxis(hztlMovementAxis);
            transform.position = transform.position + new Vector3(movementAxis * speed * Time.deltaTime, 0, 0);


            switch (playerState)
            {
                case PlayerState.walking:
                    if (Input.GetButtonDown(jumpMovementAxis) && Input.GetAxis(jumpMovementAxis) != -1)
                    {
                        Jump();
                        doubleJumpTimer = 0f;
                        playerState = PlayerState.jumping;
                    }

                    if (Input.GetButtonDown(dashMovementAxis))
                    {
                        dashTimer = 0f;
                        speed = dashSpeed;
                        playerState = PlayerState.dashing;
                    }
                    break;

                case PlayerState.dashing:
                    dashTimer += Time.deltaTime;
                    if (dashTimer >= dashDuration)
                    {
                        speed = movementSpeed;
                        playerState = PlayerState.walking;
                    }
                    break;

                case PlayerState.jumping:
                    if (canDoubleJump)
                    {
                        doubleJumpTimer += Time.deltaTime;
                        if (doubleJumpTimer >= timeBetweenJumps && Input.GetButtonDown(jumpMovementAxis) && Input.GetAxis(jumpMovementAxis) > 0)
                        {
                            Jump();
                            canDoubleJump = false;
                        }
                    }

                    rb.gravityScale = Input.GetAxis(jumpMovementAxis) == -1f ? fastFallGravityScale : gravityScale;

                    break;
            }
        }
    }

    public PlayerType GetPlayerType()
    {
        return playerType;
    }

    private void Jump()
    {
        rb.AddForce(new Vector2(0, jumpHeight));
    }

    public void Respawn(Vector2 respawnPosition)
    {
        gameObject.transform.position = respawnPosition;

        playerState = PlayerState.jumping;

        // Reset speed, in case of dashing
        speed = movementSpeed;
    }

    private void Kill()
    {
        playerState = PlayerState.dead;
    }

    private void Squash()
    {
        playerState = PlayerState.squashed;
    }

    public PlayerState GetState()
    {
        return playerState;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            playerState = PlayerState.walking;
            canDoubleJump = true;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            Player otherPlayer = collision.gameObject.GetComponent<Player>();

            // See if the collision is on the top
            Vector2 direction = collision.GetContact(0).normal;
            if (direction.y == 1)
            {
                Debug.Log("Collision on top!");
                otherPlayer.Squash();
            }
        }

        if (collision.gameObject.CompareTag("KillBox"))
        {
            Kill();
        }
    }
}
