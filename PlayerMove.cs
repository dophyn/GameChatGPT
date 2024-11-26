using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager;
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;
    
    public float maxSpeed;  // 최대 이동속도
    public float jumpPower;  // 점프할 때 가할 힘
    
    private Rigidbody2D rigid;
    private SpriteRenderer spriter;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private AudioSource asource;
    private static readonly int IsWalking = Animator.StringToHash("isWalking");

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        asource = GetComponent<AudioSource>();
    }

    void PlaySound(string action)
    {
        switch (action)
        {
            case "JUMP":
                asource.clip = audioJump;
                break;
            case "ATTACK":
                asource.clip = audioAttack;
                break;
            case "DAMAGED":
                asource.clip = audioDamaged;
                break;
            case "ITEM":
                asource.clip = audioItem;
                break;
            case "DIE":
                asource.clip = audioDie;
                break;
            case "FINISH":
                asource.clip = audioFinish;
                break;
        }
        asource.Play();
    }

    void Update()
    {
        // 플레이어 점프
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
            PlaySound("JUMP");
        }

        // 저항 설정
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }
        
        // 스프라이트 애니메이션
        if (Input.GetButton("Horizontal"))
            spriter.flipX = Input.GetAxisRaw("Horizontal") == -1;
        
        // 애니메이션 전환
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);
    }

    void FixedUpdate()
    {
        // 키 입력으로 플레이어 이동
        float hr = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * hr, ForceMode2D.Impulse);
        
        // 최대 이동속도 설정
        if (rigid.velocity.x > maxSpeed)
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed * (-1))
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
        
        // 바닥에 플랫폼이 있는지 확인
        if (rigid.velocity.y < 0.2)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null)
            {
                // 거리 계산
                if(rayHit.distance < 0.7f)
                    anim.SetBool("isJumping", false);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
            }
            else
                OnDamaged(collision.transform.position);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            gameManager.stagePoint += 100;
            collision.gameObject.SetActive(false);
            PlaySound("ITEM");
        }
        else if (collision.gameObject.tag == "Finish")
        {
            gameManager.NextStage();
            PlaySound("FINISH");
        }
    }

    void OnAttack(Transform enemy)
    {
        gameManager.stagePoint += 100;
        rigid.AddForce(Vector2.up * 2, ForceMode2D.Impulse);
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
        PlaySound("ATTACK");
    }

    void OnDamaged(Vector2 targetPos)
    {
        gameManager.HealthDown();
        gameObject.layer = 11;
        spriter.color = new Color(1, 1, 1, 0.4f);

        int dir = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dir, 1) * 7, ForceMode2D.Impulse);
        
        Invoke("OffDamaged", 1);
        PlaySound("DAMAGED");
    }

    void OffDamaged()
    {
        gameObject.layer = 10;
        spriter.color = new Color(1, 1, 1, 1);
    }

    public void OnDead()
    {
        spriter.color = new Color(1, 1, 1, 0.4f);
        spriter.flipY = true;
        boxCollider.enabled = false;
        rigid.AddForce(Vector2.up * 3, ForceMode2D.Impulse);
        PlaySound("DIE");
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
}
