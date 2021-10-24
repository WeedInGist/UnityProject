using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{   
    private bool onhitrecovery = false;
    private float hitrecovery = 1f;
    private float curTime;
    private Rigidbody2D rigidbody2D;
    public float maxSpeed = 3;
    public float jumpPower = 9.8f;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private int jumpCount = 0;
    private float time1;
    private float time2;
    // Start is called before the first frame update
    void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        curTime=hitrecovery;
    }

    void Update()
    {   
        if (jumpCount == 1)
        {
            if (rigidbody2D.velocity.y < 0.1 || rigidbody2D.velocity.y > -0.1)
            {
                time2 = Time.time;
                Debug.Log("지금이야");
                Debug.Log(time2-time1);
            }
        }
        if(onhitrecovery == true)
        {
            curTime -=Time.deltaTime;
            if (curTime <= 0)
            {
                curTime = hitrecovery;
                onhitrecovery = false;
            }
        }
        if(onhitrecovery == false)
        {
            if ( Input.GetButtonDown("Jump") && jumpCount <=1 )
            {   
                time1 = Time.time;
                rigidbody2D.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
                animator.SetBool("Jump_down", false);
                animator.SetBool("Jump_up", true);
                jumpCount += 1;
            }
            if (rigidbody2D.velocity.y < 0)
            {
                animator.SetBool("Jump_up", false);
                animator.SetBool("Jump_down", true);
            }
            if ( Input.GetButtonUp("Horizontal"))
            {
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.normalized.x *0.5f, rigidbody2D.velocity.y);
            }
            if (Mathf.Abs(rigidbody2D.velocity.x) < 0.3)
            {
                animator.SetBool("Run", false);
            }
            else 
            {
                animator.SetBool("Run", true);
            }
        }
    }
    void OnCollisionEnter2D(Collision2D collider)
    {
        if ( collider.gameObject.tag == "Enemy")
            if (collider.gameObject.layer == 9)
                if ( transform.position.y > collider.gameObject.transform.position.y && rigidbody2D.velocity.y < 0)
                {
                    rigidbody2D.AddForce(Vector2.up * 7f);
                    OnAttack(collider.gameObject.transform);
                }
                else
                {
                    StartCoroutine(OnDamaged( collider.gameObject.transform.position));
                }
            else
            {
                StartCoroutine(OnDamaged( collider.gameObject.transform.position));
            }
    }
    void OnAttack(Transform target)
    {
        MonsterMove monster = target.GetComponent<MonsterMove>();
        monster.OnDamaged();
    }
    IEnumerator OnDamaged(Vector2 targetpos)
    {
        onhitrecovery = true;
        gameObject.layer = 11;
        spriteRenderer.color = new Color(1,1,1,0.4f);
        int reflection_direction = transform.position.x - targetpos.x > 0 ? 1 : -1;
        Debug.Log(transform.position.x); 
        Debug.Log(targetpos.x);
        Debug.Log(reflection_direction);
        rigidbody2D.velocity = new Vector2(0,0);
        rigidbody2D.AddForce(new Vector2(reflection_direction, 1) * 3, ForceMode2D.Impulse);
        yield return new WaitForSeconds(1.0f);
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1,1,1,1);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(onhitrecovery == false)
        {
            float h = Input.GetAxisRaw("Horizontal");
            
            rigidbody2D.AddForce(Vector2.right * h, ForceMode2D.Impulse);
            if ( h < 0)
            {
                spriteRenderer.flipX = true;
            }
            else if ( h > 0)
            {
                spriteRenderer.flipX = false;
            }
            if (rigidbody2D.velocity.x > maxSpeed)
            {
                rigidbody2D.velocity = new Vector2(maxSpeed, rigidbody2D.velocity.y);
            }
            else if (rigidbody2D.velocity.x < -maxSpeed)
            {
                rigidbody2D.velocity = new Vector2(-maxSpeed, rigidbody2D.velocity.y);
            }
            if (rigidbody2D.velocity.y < 0)
            {   Debug.DrawRay(rigidbody2D.position, Vector2.down, new Color(0,1,0));
                RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, Vector2.down, 1f, LayerMask.GetMask("Platform"));
                if( raycastHit2D.collider != null)
                {
                    animator.SetBool("Jump_down", false);
                    jumpCount = 0 ;
                }
            }
        }
    }
}
