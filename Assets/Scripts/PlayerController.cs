using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [Header("Movement settings")]
    public float acceleration;
    public float maxMovementSpeed;
    public float airMovementCoefficient;

    [Space(10)]
    public float jumpSpeed;
    public float jumpButtonGravityCoefficient;

    [Space(10)]
    public float frictionGround;
    public float frictionAir;

    [Header("Ground settings")]
    public LayerMask groundLayer;
    public float groundCheckPadding;

    private bool onGround = true;
    private bool jump = false;
    private bool jumpCancel = false;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

	void Awake() {
        rb = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
   	}

    void Update() {
        if (Input.GetButtonDown("Jump") && onGround) {
            jump = true;
        }
        else if (Input.GetButtonUp("Jump") && !onGround) {
            jumpCancel = true;
        }
    }
	
	void FixedUpdate() {
        onGround = CheckIfTouchingGround();

        float horizontalInput = Input.GetAxis("Horizontal");

        const float floatTolerance = 0.0001f;

        // Apply friction.
        if (onGround) {
            if (Mathf.Abs(rb.velocity.x) > frictionGround) {
                rb.velocity = new Vector2(
                    rb.velocity.x - frictionGround * Mathf.Sign(rb.velocity.x), 0
                );
            }
            else {
                rb.velocity = new Vector2(0, 0);
            }
        }
        else {
            if (Mathf.Abs(rb.velocity.x) > frictionAir) {
                rb.velocity = new Vector2(
                    rb.velocity.x - frictionAir * Mathf.Sign(rb.velocity.x),
                    rb.velocity.y
                );
            }
            else {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }

        if (Mathf.Abs(horizontalInput) > floatTolerance) {
            float deltaVelocityX = Input.GetAxis("Horizontal") * acceleration;

            if (!onGround) {
                deltaVelocityX *= airMovementCoefficient;
            }

            float newVelocityX = rb.velocity.x + deltaVelocityX;

            rb.velocity = new Vector2(
                Mathf.Sign(newVelocityX) * Mathf.Min(Mathf.Abs(newVelocityX), maxMovementSpeed),
                rb.velocity.y
            );
        }

        if (jump) {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            rb.gravityScale = jumpButtonGravityCoefficient;
            jump = false;
        }
        if (jumpCancel) {
            rb.gravityScale = 1.0f;
            jumpCancel = false;
        }
	}

    bool CheckIfTouchingGround() {
        float spriteHeight = spriteRenderer.sprite.bounds.extents.y;

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position, Vector2.down,
            spriteHeight + groundCheckPadding, groundLayer
        );

        return hit.collider != null;
    }
}
