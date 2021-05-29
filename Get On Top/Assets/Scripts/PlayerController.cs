using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    enum PlayerState
    {
        walking,
        jumping,
        dashing,
        dead
    }

    private PlayerState playerState;

    enum PlayerType
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
        float movementAxis = Input.GetAxis(hztlMovementAxis);
        transform.position = transform.position + new Vector3(movementAxis * speed * Time.deltaTime, 0, 0);


        switch (playerState)
        {
            case PlayerState.walking:
                if (Input.GetButtonDown(jumpMovementAxis))
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
                    if (doubleJumpTimer >= timeBetweenJumps && Input.GetButtonDown(jumpMovementAxis))
                    {
                        Jump();
                        canDoubleJump = false;
                    }
                }

                rb.gravityScale = Input.GetAxis(jumpMovementAxis) == -1f ? fastFallGravityScale : gravityScale;

                break;
        }
    }

    private void Jump()
    {
        rb.AddForce(new Vector2(0, jumpHeight));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            playerState = PlayerState.walking;
            canDoubleJump = true;
        }
    }
}
