using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private Rigidbody2D playerRB;
    private Animator anim;

    private bool facingRigth;
    private float horizontal;
    public float velocity;
    private bool grounded;
    private bool touchingWall;

    private float running;

    public float jumpForce;

    public Transform groundCheck;

    public LayerMask whatIsGround;

    void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        facingRigth = true;
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        running = Input.GetAxisRaw("Fire3");

        if (!touchingWall)
        {
            playerRB.velocity = new Vector2(horizontal * velocity, playerRB.velocity.y);
        }

        if ((facingRigth && horizontal < 0) || !facingRigth && horizontal > 0)
        {
            Flip();
        }

        anim.SetBool("Grounded", grounded);
        anim.SetBool("Walking", horizontal != 0f);
        anim.SetFloat("VelocityX", playerRB.velocity.x);
        anim.SetFloat("VelocityY", playerRB.velocity.y);
        anim.SetBool("Running", running > 0f);
        if (Input.GetButtonDown("Fire3"))
        {
            anim.SetLayerWeight(0, 1);
            anim.SetTrigger("Transform");
        }
        if (Input.GetButtonUp("Fire3"))
        {
            anim.SetLayerWeight(1, 1);
            anim.SetTrigger("Transform");
        }
    }

    void FixedUpdate()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, .02f, whatIsGround);

        if (Input.GetButtonDown("Jump") && grounded)
        {
            playerRB.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }
    }

    void Flip()
    {
        facingRigth = !facingRigth;
        Vector2 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.isTrigger && collision.gameObject.layer.Equals("Wall"))
        {
            touchingWall = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        touchingWall = false;
    }
}
