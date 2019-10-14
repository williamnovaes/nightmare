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
    public bool isHuman = true;
    public bool isBat;

    void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ChangeLayersWeight(GetLayerValue(isHuman), GetLayerValue(isWolf), GetLayerValue(isBat), 0);
        ChangeModeSpriteVisible();
    }

    // Update is called once per frame
    void Update()
    {
        Walk();
        Jump();
        Flip();

        running = Input.GetAxisRaw("Fire3");

        anim.SetBool("Grounded", grounded);
        anim.SetBool("Walking", IsWalking());
        anim.SetFloat("VelocityX", playerRB.velocity.x);
        anim.SetFloat("VelocityY", playerRB.velocity.y);
        anim.SetBool("Running", running > Mathf.Epsilon);

        if (Input.GetButtonDown("Fire3"))
        {
            isHuman = false;
            isWolf = true;
            ChangeLayersWeight(GetLayerValue(isHuman), GetLayerValue(isWolf), GetLayerValue(isBat), 0);
            anim.SetTrigger("Transform");
            ChangeModeSpriteVisible();
            anim.Play("knight_wolf_transform");
        }
        if (Input.GetButtonUp("Fire3"))
        {
            isHuman = true;
            isWolf = false;
            ChangeLayersWeight(GetLayerValue(isHuman), GetLayerValue(isWolf), GetLayerValue(isBat), 0);
            ChangeTransformationFlag(true, false, false, false);
            anim.SetTrigger("Transform");
            ChangeModeSpriteVisible();
            anim.Play("knight_wolf_transform");
        }
    }

    void FixedUpdate()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, .02f, whatIsGround);
    }

    int GetLayerValue(bool check)
    {
        return check ? 1 : 0;
    }

    void ChangeModeSpriteVisible() {
        objHuman.SetActive(isHuman);
        objWolf.SetActive(isWolf);
    }

    void ChangeTransformationFlag(bool human, bool wolf, bool bat, bool golen)
    {
        isHuman = human;
        isWolf = wolf;
        isBat = bat;
    }

    void ChangeLayersWeight(int val, int val2, int val3, int val4)
    {
        anim.SetLayerWeight(0, val);
        anim.SetLayerWeight(1, val2);
        anim.SetLayerWeight(2, val3);
        anim.SetLayerWeight(3, val4);
    }

    void Flip()
    {
        if (Mathf.Abs(playerRB.velocity.x) > Mathf.Epsilon) //velocity + turn right / velocity - turn left
        {
            transform.localScale = new Vector2 (Mathf.Sign(playerRB.velocity.x), transform.localScale.y);
        }
        //facingRigth = !facingRigth;
        //Vector2 scale = transform.localScale;
        //scale.x *= -1;
        //transform.localScale = scale;
    }

    void Walk()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        if (!touchingWall) { 
            Vector2 playerVelocity = new Vector2(horizontal * velocity, playerRB.velocity.y);
            playerRB.velocity = playerVelocity;
        }
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && grounded)
        {
            playerRB.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }
    }

    bool IsWalking()
    {
        return horizontal > 0f || horizontal < 0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        touchingWall = CheckWallCollision(collision) ? true : touchingWall;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        touchingWall = CheckWallCollision(collision) ? false : touchingWall;
    }

    bool CheckWallCollision(Collider2D collision)
    {
        return collision.gameObject.layer.Equals("Wall");
    }
}
