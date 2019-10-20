using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    
    Rigidbody2D enemyRB;

    [SerializeField] private float moveSpeed;
    [SerializeField] private bool facingRigth;
    [SerializeField] private bool mover;
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
        Flip();
        Move();
    }

    private void Move()
    {
        enemyRB.velocity = new Vector2(moveSpeed, 0f);
    }

    private void Flip()
    {
        if (time >= 2f)
        {
            moveSpeed *= -1;
            facingRigth = !facingRigth;
            Vector2 scale = transform.localScale;
            scale = new Vector2(scale.x * -1, scale.y);
            transform.localScale = scale;
            time = 0f;
        }
        else
        {
            time += Time.deltaTime;
        }
    }
}
