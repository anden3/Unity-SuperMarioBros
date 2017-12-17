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

    private bool alive = true;
    private bool onGround = true;
    private bool jump = false;
    private bool jumpCancel = false;

    private float defaultScale;

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

	void Awake() {
        anim = gameObject.GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        defaultScale = transform.localScale.x;
   	}

    void Update() {
        if (Input.GetButtonDown("Jump") && onGround) {
            anim.SetBool("IsJumping", true);
            jump = true;
        }
        else if (Input.GetButtonUp("Jump") && !onGround) {
            jumpCancel = true;
        }

        if (rb.velocity.x > 0) {
            transform.localScale = new Vector2(defaultScale, transform.localScale.y);
        }
        else if (rb.velocity.x < 0) {
            transform.localScale = new Vector2(-defaultScale, transform.localScale.y);
        }
    }
	
	void FixedUpdate() {
        if (!alive) {
            return;
        }

        onGround = CheckIfTouchingGround();

        float horizontalInput = Input.GetAxis("Horizontal");
        const float floatTolerance = 0.0001f;

        // Apply friction.
        if (onGround) {
            anim.SetBool("IsJumping", jump);

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
            anim.SetBool("IsRunning", true);

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
        else {
            anim.SetBool("IsRunning", false);
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

    public void Die() {
        alive = false;
        anim.SetBool("IsDead", true);
        Destroy(gameObject.GetComponent<CapsuleCollider2D>());
        StartCoroutine(DeathAnimation());
    }

    private IEnumerator DeathAnimation() {
        float startTime = Time.time;
        Vector2 startPos = transform.position;

        Vector2 goal = Camera.current.ScreenToWorldPoint(new Vector3(
            Camera.current.pixelWidth / 2, Camera.current.pixelHeight / 2, 0
        ));

        while ((goal - (Vector2)transform.position).sqrMagnitude >= 0.5f) {
            transform.position = Vector2.Lerp(startPos, goal, Time.time - startTime);
            yield return null;
        }

        startTime = Time.time;
        startPos = transform.position;

        goal = Camera.current.ScreenToWorldPoint(new Vector3(
            Camera.current.pixelWidth / 2, -16 / 2, 0
        ));

        while ((goal - (Vector2)transform.position).sqrMagnitude >= 0.5f) {
            transform.position = Vector2.Lerp(startPos, goal, (Time.time - startTime) * 2);
            yield return null;
        }

        Application.Quit();
    }
}
