using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    
    Rigidbody2D enemyRB;

    [SerializeField] float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        enemyRB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        enemyRB.velocity = new Vector2(moveSpeed, 0f);
    }

    private void Flip()
    {
        moveSpeed *= -1;
        transform.localScale = new Vector2(Mathf.Sign(enemyRB.velocity.x), transform.localScale.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Colidiu " + collision.gameObject.name);
        if (collision.gameObject.layer.Equals("Foreground"))
        {
            Flip();
        }
    }
}
