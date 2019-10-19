using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    
    Rigidbody2D enemyRB;

    [SerializeField] private float moveSpeed;
    [SerializeField] private bool facingRigth;
    private float time;

    // Start is called before the first frame update
    void Start()
    {
        enemyRB = GetComponent<Rigidbody2D>();
        time = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        enemyRB.velocity = new Vector2(moveSpeed, 0f);
        if (time >= 2f)
        {
            Flip();
            time = 0f;
        }
        else
        {
            time += Time.deltaTime;
        }

    }

    private void Flip()
    {
        moveSpeed *= -1;
        facingRigth = !facingRigth;
        transform.localScale = new Vector2(moveSpeed, transform.localScale.y);
    }
}
