using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    Rigidbody2D playerRB;
    Animator playerAnim;
    Collider2D playerColl;

    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] GameObject objWolf;
    [SerializeField] GameObject objHuman;

    [SerializeField] private float velocity;
    [SerializeField] private float jumpForce;
    [SerializeField] private float climbSpeed;

    private float horizontal;
    private float running;

    private bool touchingWall;
    private bool isWolf;
    private bool isHuman = true;
    private bool isBat;

    void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();
        playerColl = GetComponent<Collider2D>();
        ChangeLayersWeight(GetLayerValue(isHuman), GetLayerValue(isWolf), GetLayerValue(isBat), 0);
        ChangeModeSpriteVisible();
    }

    // Update is called once per frame
    void Update()
    {
        Walk();
        Jump();
        Flip();
        ClimbLadder();

        running = CrossPlatformInputManager.GetAxis("Fire3");

        playerAnim.SetBool("Grounded", Grounded());
        playerAnim.SetBool("Walking", Walking());
        playerAnim.SetBool("Running", running > Mathf.Epsilon);
        playerAnim.SetFloat("VelocityX", playerRB.velocity.x);
        playerAnim.SetFloat("VelocityY", playerRB.velocity.y);

        if (CrossPlatformInputManager.GetButtonDown("Fire3"))
        {
            isHuman = false;
            isWolf = true;
            ChangeLayersWeight(GetLayerValue(isHuman), GetLayerValue(isWolf), GetLayerValue(isBat), 0);
            playerAnim.SetTrigger("Transform");
            ChangeModeSpriteVisible();
            playerAnim.Play("knight_wolf_transform");
        }
        if (CrossPlatformInputManager.GetButtonUp("Fire3"))
        {
            isHuman = true;
            isWolf = false;
            ChangeLayersWeight(GetLayerValue(isHuman), GetLayerValue(isWolf), GetLayerValue(isBat), 0);
            ChangeTransformationFlag(true, false, false, false);
            playerAnim.SetTrigger("Transform");
            ChangeModeSpriteVisible();
            playerAnim.Play("knight_wolf_transform");
        }
    }

    void FixedUpdate()
    {

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
        playerAnim.SetLayerWeight(0, val);
        playerAnim.SetLayerWeight(1, val2);
        playerAnim.SetLayerWeight(2, val3);
        playerAnim.SetLayerWeight(3, val4);
    }

    void Flip()
    {
        //Mathf.Abs return absolute value (Mathf.Abs(-10) eq 10)
        if (Mathf.Abs(playerRB.velocity.x) > Mathf.Epsilon) //velocity + turn right / velocity - turn left
        {
            //Mathf.Sign return -1 for negative values and 1 for positive values (Mathf.Sign(-10) eq -1)
            transform.localScale = new Vector2(Mathf.Sign(playerRB.velocity.x), transform.localScale.y);
        }
        //facingRigth = !facingRigth;
        //Vector2 scale = transform.localScale;
        //scale.x *= -1;
        //transform.localScale = scale;
    }

    void Walk()
    {
        horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        if (!touchingWall) {
            Vector2 playerVelocity = new Vector2(horizontal * velocity, playerRB.velocity.y);
            playerRB.velocity = playerVelocity;
        }
    }

    void Jump()
    {
        if (!Grounded()) { return; }

        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            playerRB.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            //other option to have more control of jump
            //set gravity on Physycs2D to 100 or other value bigger than 9.8 (more effective if pixels per unit of sprite is less than normal)
            //Vector2 jump = new Vector2(0f, jumpSpeed);
            //playerRB.velocity += jump;
        }
    }

    bool Walking()
    {
        return Mathf.Abs(playerRB.velocity.x) > Mathf.Epsilon;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        touchingWall = CheckCollisionInWall(collision) ? true : touchingWall;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        touchingWall = CheckCollisionInWall(collision) ? false : touchingWall;
    }

    bool CheckCollisionInWall(Collider2D collision)
    {
        return collision.gameObject.layer.Equals("Wall");
    }

    private bool Grounded()
    {
        //return playerColl.IsTouchingLayers(LayerMask.GetMask("Ground"));
        return Physics2D.OverlapCircle(groundCheck.position, .02f, whatIsGround);
    }

    private void ClimbLadder()
    {
        //reduce the collider size of ladder to center of sprite
        //set isTrigger on composite collider of tile
        if (!playerColl.IsTouchingLayers(LayerMask.GetMask("Ladder"))) { return; }

        float controlThrow = CrossPlatformInputManager.GetAxis("Vertical");
        Vector2 climbVelocity = new Vector2(playerRB.velocity.x, controlThrow * climbSpeed);
        playerRB.velocity = climbVelocity;
    }
}
