using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerControler : Character  
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float speed;
    
    [SerializeField] private float jumforce = 300f;

    [SerializeField] private Kunai kunaiPrefab ;
    [SerializeField] private Transform throwPoint ;
    [SerializeField] private GameObject Attackarea ;

    private bool isGrounded = true;
    private bool isAttack= false;
    private bool isJumping;
    private bool isDeath;
    //private bool isIdle = true;



    private float horizontal;
    
    private int coin = 0;

    private Vector3 savepoint;

    private void Awake()
    {
        coin = PlayerPrefs.GetInt("coin", 0);
    }
    public override void OnInit()
    {
        base.OnInit();
        
        isAttack = false;

        transform.position = savepoint;
        ChangeAnim("idle");
        DeActiveAttack();

        savePoint();
        UIManager.instance.SetCoint(coin);
    }
    protected override void OnDeath()
    {
        base.OnDeath();
    }
    public override void OnDespawn()
    {
        base.OnDespawn();
        OnInit();
    }
    // Update is called once per frame
    void Update()
    {
        if (IsDead)
        { 
            return; 
        }
        isGrounded = Checkgrounded();
        //Debug.Log(isGrounded);
        //-1 0 1
        //horizontal = Input.GetAxisRaw("Horizontal");
        //Debug.Log(horizontal);
        if (isAttack)
        {
            
            rb.velocity = Vector2.zero;
            return;
        }

        if (isGrounded)
        {
            if (isJumping)
            {
                return;
            }
            //jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
            //run 
            if (Mathf.Abs(horizontal) > 0.1f && isGrounded)
            {
                ChangeAnim("run");
                
            }
            //attack
            if (Input.GetKeyDown(KeyCode.E) && isGrounded)
            {
                Attack();
            }
            //throw
            if (Input.GetKeyDown(KeyCode.Q) && isGrounded)
            {
                Throw();
            }
        }
        //check falling
        if (!isGrounded && rb.velocity.y < 0)
        {
            ChangeAnim("fall");
            isJumping = false;
        }
        //moving
        if (Mathf.Abs(horizontal) > 0.1f)
        {        
            rb.velocity = new Vector2(horizontal  * speed, rb.velocity.y);
            transform.rotation = Quaternion.Euler(new Vector3(0, horizontal > 0 ? 0 : 180, 0));
        }
        // idle
        else if (isGrounded)
        {  
            ChangeAnim("idle");
            rb.velocity = Vector2.zero;
        }
    }
    private bool Checkgrounded()
    {
        
        Debug.DrawLine(transform.position, transform.position + Vector3.down * 1.1f,Color.red);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);
        if (hit.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
        // return hit.collider != null
    }
    public void Attack()
    {
        ChangeAnim("attack");
        isAttack = true;
        Invoke(nameof(Resetattack), 0.5f);     
        ActiveAttack();
        Invoke(nameof(DeActiveAttack), 0.5f);
    }
    public void Throw()
    {
        ChangeAnim("throw");
        isAttack = true;
        
        Invoke(nameof(Resetattack), 0.5f);
        Instantiate(kunaiPrefab, throwPoint.position, throwPoint.rotation);
    }
    public void Jump()
    {
        if (isJumping || !isGrounded)
        {
            return;
        }
        if(isJumping && isGrounded)
        {
            ChangeAnim("idle");
        }

        isJumping = true;
        ChangeAnim("jump");
        rb.AddForce(jumforce * Vector2.up);
    }
    private void Resetattack()
    {
        isAttack = false;
        ChangeAnim("idle");
        
    }
    
    
    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.tag == "platform")
    //    {
    //        OnInit();
    //    }

    //}

    internal void savePoint()
    {
        savepoint = transform.position;
    }
    private void ActiveAttack()
    {
        Attackarea.SetActive(true);
    }
    private void DeActiveAttack()
    {
        Attackarea.SetActive(false);
    }
    public void SetMoving(float horizontal)
    {
        this.horizontal = horizontal;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "coin")
        {
            coin++;
            PlayerPrefs.SetInt("coin", coin);
            UIManager.instance.SetCoint(coin);
            Destroy(collision.gameObject);
            //Debug.Log(coin);
        }
        if (collision.tag == "Deadzone")
        {
            isDeath = true;
            ChangeAnim("die");
            Invoke(nameof(OnInit), 1f);
        }

    }
}

