using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpHeight;

    private Rigidbody2D rb;

    private bool onGround = false;
    private bool canDoubleJump = false;

    private float doubleJumpTimer;
    [SerializeField] private float timeBetweenJumps;

    [SerializeField] private float gravityScale;
    [SerializeField] private float fastFallGravityScale;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
    }

    void Update()
    {
        float movementAxis = Input.GetAxis("Horizontal");
        transform.position = transform.position + new Vector3(movementAxis * speed * Time.deltaTime, 0, 0);

        if (Input.GetAxis("Jump") == 1f && onGround)
        {
            Jump();
            doubleJumpTimer = 0f;
        }

        if (!onGround)
        {
            if (canDoubleJump)
            {
                doubleJumpTimer += Time.deltaTime;
                if (doubleJumpTimer >= timeBetweenJumps)
                {
                    if (Input.GetAxis("Jump") == 1f)
                    {
                        Jump();
                        canDoubleJump = false;
                    }
                }
            }

            if (Input.GetAxis("Jump") == -1f)
            {
                rb.gravityScale = fastFallGravityScale;
            }
            else
            {
                rb.gravityScale = gravityScale;
            }
        }

        
    }

    private void Jump()
    {
        rb.AddForce(new Vector2(0, jumpHeight));
        onGround = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            onGround = true;
            canDoubleJump = true;
        }
    }
}
