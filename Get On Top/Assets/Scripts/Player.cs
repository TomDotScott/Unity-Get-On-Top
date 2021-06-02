using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Movement Settings")]
    [SerializeField] private float movementSpeed;
    private float currentSpeed;

    [SerializeField] private float dashSpeed;
    private float currentDashSpeed;

    [SerializeField] private float jumpHeight;
    private float currentJumpHeight;

    private Rigidbody2D rb;

    private bool canDoubleJump = false;

    private float doubleJumpTimer;
    [SerializeField] private float timeBetweenJumps;

    private float dashTimer;
    [SerializeField] private float dashDuration;

    [SerializeField] private float gravityScale;
    [SerializeField] private float fastFallGravityScale;
    
    public bool Frozen = false;

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

    [Header("Player1/Player2 settings")]
    [SerializeField] private PlayerType playerType;
    private string hztlMovementAxis = "Horizontal";
    private string jumpMovementAxis = "Jump";
    private string dashMovementAxis = "Dash";


    [Header("Powerup settings")]
    [SerializeField] private float bigShapeWidth;
    [SerializeField] private float smallShapeSize;
    [SerializeField] private float powerUpJumpHeight;
    [SerializeField] private float powerUpMovementSpeed;
    [SerializeField] private float powerUpDashSpeed;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite triangleSprite;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Pickup.PickupType powerUpState;

    [SerializeField] private TextMesh powerupText;
    private float powerupDuration = 0f;

    public Pickup.PickupType PowerUpState
    {
        get => powerUpState;
        set
        {
            powerUpState = value;
            powerupText.text = value.ToString();
        }
    }

    void Start()
    {
        powerUpState = Pickup.PickupType.Normal;

        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = defaultSprite;

        currentSpeed = movementSpeed;
        
        currentDashSpeed = dashSpeed;

        currentJumpHeight = jumpHeight;

        hztlMovementAxis += playerType == PlayerType.playerOne ? "PlayerOne" : "PlayerTwo";
        jumpMovementAxis += playerType == PlayerType.playerOne ? "PlayerOne" : "PlayerTwo";
        dashMovementAxis += playerType == PlayerType.playerOne ? "PlayerOne" : "PlayerTwo";
    }

    void Update()
    {
        if (!Frozen)
        {
            float movementAxis = Input.GetAxis(hztlMovementAxis);
            transform.position = transform.position + new Vector3(movementAxis * currentSpeed * Time.deltaTime, 0, 0);

            powerupText.transform.position = new Vector3(
                gameObject.transform.position.x, 
                gameObject.transform.position.y + 0.5f, 
                gameObject.transform.position.z
                );

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
                        currentSpeed = currentDashSpeed;
                        playerState = PlayerState.dashing;
                    }
                    break;

                case PlayerState.dashing:
                    dashTimer += Time.deltaTime;
                    if (dashTimer >= dashDuration)
                    {
                        currentSpeed = movementSpeed;
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

            powerupDuration -= Time.deltaTime;
            if (powerupDuration <= 0)
            {
                ResetPowerup();
            }
        }
    }

    private void ApplyPowerup(Pickup.PickupType powerupType, float duration)
    {
        switch (powerupType)
        {
            case Pickup.PickupType.BiggerShape:
                gameObject.transform.localScale = new Vector3(bigShapeWidth, 1, 1);
                break;
            case Pickup.PickupType.SmallerShape:
                gameObject.transform.localScale = new Vector3(smallShapeSize, smallShapeSize, 1);
                break;
            case Pickup.PickupType.MoreJump:
                currentJumpHeight = powerUpJumpHeight;
                break;
            case Pickup.PickupType.MoreSpeed:
                currentSpeed = powerUpMovementSpeed;
                currentDashSpeed = powerUpDashSpeed;
                break;
            case Pickup.PickupType.Triangle:
                spriteRenderer.sprite = triangleSprite;
                break;
        }
        PowerUpState = powerupType;
        powerupDuration = duration;
    }

    private void ResetPowerup()
    {
        switch (powerUpState)
        {
            case Pickup.PickupType.BiggerShape:
            case Pickup.PickupType.SmallerShape:
                gameObject.transform.localScale = new Vector3(1, 1, 1);
                break;
            case Pickup.PickupType.MoreJump:
                currentJumpHeight = jumpHeight;
                break;
            case Pickup.PickupType.MoreSpeed:
                currentSpeed = movementSpeed;
                currentDashSpeed = dashSpeed;
                break;
            case Pickup.PickupType.Triangle:
                spriteRenderer.sprite = defaultSprite;
                break;
        }

        PowerUpState = Pickup.PickupType.Normal;
    }

    public PlayerType GetPlayerType()
    {
        return playerType;
    }

    private void Jump()
    {
        rb.AddForce(new Vector2(0, currentJumpHeight));
    }

    public void Respawn(Vector2 respawnPosition)
    {
        gameObject.transform.position = respawnPosition;

        ResetPowerup();

        playerState = PlayerState.jumping;

        // Reset speed, in case of dashing
        currentSpeed = movementSpeed;
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

            // Stop the infinite dash glitch
            currentSpeed = PowerUpState == Pickup.PickupType.MoreSpeed ? powerUpMovementSpeed : movementSpeed;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            Player otherPlayer = collision.gameObject.GetComponent<Player>();

            if (otherPlayer.PowerUpState != Pickup.PickupType.Triangle)
            {
                // See if the collision is on the top
                Vector2 direction = collision.GetContact(0).normal;
                if (direction.y == 1)
                {
                    Debug.Log("Collision on top!");
                    otherPlayer.Squash();
                }
            }
            else
            {
                Kill();
            }
        }

        if (collision.gameObject.CompareTag("KillBox"))
        {
            Kill();
        }

        if (collision.gameObject.CompareTag("Pickup"))
        {
            Pickup collidedPickup = collision.gameObject.GetComponent<Pickup>();

            ApplyPowerup(collidedPickup.pickupType, collidedPickup.powerupDuration);

            collidedPickup.Kill();
        }
    }
}
