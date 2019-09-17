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

    public float jumpForce;

    public Transform groundCheck;

    public LayerMask whatIsGround;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        facingRigth = true;
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, .02f, whatIsGround);
    }

    void FixedUpdate()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        playerRB.velocity = new Vector2(horizontal * velocity, playerRB.velocity.y);

        if ((facingRigth && horizontal < 0) || !facingRigth && horizontal > 0)
        {
            Flip();
        }

        if (Input.GetButtonDown("Jump") && grounded)
        {
            playerRB.AddForce(new Vector2(0, jumpForce));
        }

        anim.SetBool("Grounded", grounded);
        anim.SetBool("Walking", horizontal != 0f);
        anim.SetFloat("VelocityX", playerRB.velocity.x);
        anim.SetFloat("VelocityY", playerRB.velocity.y);
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
        if (!collision.isTrigger && collision.gameObject.tag.Equals("Block"))
        {
            touchingWall = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        touchingWall = false;
    }
}
