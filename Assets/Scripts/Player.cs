using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    Rigidbody2D playerRB;
    Animator playerAnim;
    CapsuleCollider2D playerColl;

    [SerializeField] GameObject objWolf;
    [SerializeField] GameObject objHuman;

    [SerializeField] private float velocity;
    [SerializeField] private float jumpForce;
    [SerializeField] private float climbSpeed;
    [SerializeField] private int health;

    private float gravityScaleStart;
    private float horizontal;
    private float running;

    private bool isHuman = true;
    private bool isWolf;
    private bool isBat;
    private bool jumpOffCoroutineRunning;

    //STATE
    private bool isAlive;

    void Awake()
    {
        isAlive = true;
        playerRB = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();
        playerColl = GetComponent<CapsuleCollider2D>();
        gravityScaleStart = playerRB.gravityScale;
        ChangeLayersWeight(GetLayerValue(isHuman), GetLayerValue(isWolf), GetLayerValue(isBat), 0);
        ChangeModeSpriteVisible();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive) { return; }

        Walk();
        Jump();
        Flip();
        ClimbLadder();
        TakeDamage();

        running = CrossPlatformInputManager.GetAxis("Fire3");

        playerAnim.SetBool("Grounded", Grounded());
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
        Vector2 playerVelocity = new Vector2(horizontal * velocity, playerRB.velocity.y);
        playerRB.velocity = playerVelocity;

        playerAnim.SetBool("Walking", Walking());
    }

    void Jump()
    {
        if (!Grounded()) { return; }

        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                StartCoroutine("JumpOff");
            } else
            {
                playerRB.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
                //other option to have more control of jump
                //set gravity on Physycs2D to 100 or other value bigger than 9.8
                //(more effective if pixels per unit of sprite is less than normal)
                //Vector2 jump = new Vector2(0f, jumpSpeed);
                //playerRB.velocity += jump;
            }
        }
    }

    private void TakeDamage()
    {
        CapsuleCollider2D activeCollider = isHuman ? objHuman.GetComponent<CapsuleCollider2D>() : objWolf.GetComponent<CapsuleCollider2D>();
        if (activeCollider == null) { return; }

        if (activeCollider.IsTouchingLayers(LayerMask.GetMask("Enemy", "Spikes")))
        {
            health--;
        }

        isAlive = !isDead();

        if (!isAlive)
        {
            int indexCurrentScene = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(indexCurrentScene);
        }
    }

    IEnumerator JumpOff()
    {
        jumpOffCoroutineRunning = true;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("PassThrough"), true);
        yield return new WaitForSeconds(0.3f);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("PassThrough"), false);
        jumpOffCoroutineRunning = false;
    }

    private void ClimbLadder()
    {
        //reduce the collider size of ladder to center of sprite
        //set isTrigger on composite collider of tile
        if (!playerColl.IsTouchingLayers(LayerMask.GetMask("Ladder")))
        {
            //playerAnim.SetBool("Climbing", false);
            //set start gravity scale back to normalize gravity
            playerRB.gravityScale = gravityScaleStart;
            return;
        }

        float controlThrow = CrossPlatformInputManager.GetAxis("Vertical");
        Vector2 climbVelocity = new Vector2(playerRB.velocity.x, controlThrow * climbSpeed);
        playerRB.velocity = climbVelocity;
        //0 to gravity scale for player dont drop down in ladder
        playerRB.gravityScale = 0f;

        playerAnim.SetBool("Climbing", Mathf.Abs(playerRB.velocity.y) > Mathf.Epsilon);
    }

    int GetLayerValue(bool check)
    {
        return check ? 1 : 0;
    }

    void ChangeModeSpriteVisible()
    {
        objHuman.SetActive(isHuman);
        objWolf.SetActive(isWolf);
    }

    void ChangeTransformationFlag(bool human, bool wolf, bool bat, bool golen)
    {
        isHuman = human;
        isWolf = wolf;
        isBat = bat;
    }

    void ChangeLayersWeight(int val, int val1, int val2, int val3)
    {
        playerAnim.SetLayerWeight(0, val);
        playerAnim.SetLayerWeight(1, val1);
        playerAnim.SetLayerWeight(2, val2);
        playerAnim.SetLayerWeight(3, val3);
    }

    bool Walking()
    {
        return Mathf.Abs(playerRB.velocity.x) > Mathf.Epsilon;
    }

    private bool Grounded()
    {
        return playerColl.IsTouchingLayers(LayerMask.GetMask("Foreground", "PassThrough"));
        //return Physics2D.OverlapCircle(groundCheck.position, .02f, whatIsGround);
    }

    private bool isDead()
    {
        return health <= 0;
    }
    
    /*
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
    }*/
}