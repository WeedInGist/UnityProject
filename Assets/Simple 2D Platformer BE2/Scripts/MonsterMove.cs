using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMove : MonoBehaviour
{   
    private Animator ani;
    private SpriteRenderer sprite;
    private Rigidbody2D monster_rigidbody2D;
    private float direction;
    private RaycastHit2D ray;
    private Collider2D coll;
    // Start is called before the first frame update
    void Awake()
    {   sprite = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();
        monster_rigidbody2D = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();
        InvokeRepeating("Think", 2,3);
    }

    void Update()
    {  
        if (direction != 0)
        {
            ani.SetBool("IsRunning", true);
        }
        else
            ani.SetBool("IsRunning", false);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 check_Platform = new Vector2(monster_rigidbody2D.position.x + direction * 0.3f, monster_rigidbody2D.position.y);
        Debug.DrawRay(check_Platform, Vector2.down, new Color(0,1,0));
        RaycastHit2D ray = Physics2D.Raycast(check_Platform, Vector2.down, 1.0f, LayerMask.GetMask("Platform"));
        if (ray.collider == null)
        {
            Turn(direction);
        }
        monster_rigidbody2D.velocity = new Vector2(direction, 0);
    }
    void Think()
    {   
        direction = Random.Range(-1,2);
        Debug.Log("direction");
        if (direction != 0 )
        {
            sprite.flipX = direction == 1 ? true : false;
        }
    }
    void Turn(float di)
    {
        direction = -1 * di;
        if (direction != 0 )
        {
            sprite.flipX = direction == 1 ? true : false;
        }
    }
    public void OnDamaged()
    {
        sprite.flipY = true;
        sprite.color = new Color(1,1,1,0.4f);
        monster_rigidbody2D.AddForce((Vector2.up)*5f);
        coll.enabled = false;
        Destroy(gameObject,2f);
    }
}
