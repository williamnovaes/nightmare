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

    public GameObject objWolf;
    public GameObject objHuman;

    public bool isWolf;
    public bool isHuman;
    public bool isBat;

    void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        //objHuman = GameObject.FindWithTag("Human");
       // objWolf = GameObject.FindWithTag("Wolf");
        facingRigth = true;
        isHuman = true;
        AlterarLayersWeight(1, 0, 0, 0);
        AlterarObjects();
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
        anim.SetBool("Walking", isWalking());
        anim.SetFloat("VelocityX", playerRB.velocity.x);
        anim.SetFloat("VelocityY", playerRB.velocity.y);
        anim.SetBool("Running", running > 0f);

        if (Input.GetButtonDown("Fire3"))
        {
            AlterarLayersWeight(0, 1, 0, 0);
            anim.SetTrigger("Transform");
            isHuman = false;
            isWolf = true;
            AlterarObjects();
            anim.Play("knight_wolf_transform");
        }
        if (Input.GetButtonUp("Fire3"))
        {
            AlterarLayersWeight(1, 0, 0, 0);
            AlterarFlagsForma(true, false, false, false);
            anim.SetTrigger("Transform");
            isHuman = true;
            isWolf = false;
            AlterarObjects();
            anim.Play("knight_wolf_transform");
        }
    }

    void AlterarObjects() {
        objHuman.SetActive(isHuman);
        objWolf.SetActive(isWolf);
    }

    void AlterarFlagsForma(bool human, bool wolf, bool bat, bool golen)
    {
        isHuman = human;
        isWolf = wolf;
        isBat = bat;
    }

    void AlterarLayersWeight(int val, int val2, int val3, int val4)
    {
        anim.SetLayerWeight(0, val);
        anim.SetLayerWeight(1, val2);
        anim.SetLayerWeight(2, val3);
        anim.SetLayerWeight(3, val4);
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

    bool isWalking()
    {
        return horizontal > 0f || horizontal < 0f;
    }
}
