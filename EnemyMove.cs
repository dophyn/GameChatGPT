using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMove : MonoBehaviour
{
    private Rigidbody2D rigid;
    private Animator anim;
    private SpriteRenderer spriter;
    private BoxCollider2D boxCollider;
    
    public int nextMove;  // 몬스터 다음 액션

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        Invoke("Think", 3);
    }

    void FixedUpdate()
    {
        // 몬스터 이동
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);
        
        // 플랫폼 충돌 확인
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove, rigid.position.y);
        
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if (rayHit.collider == null)
            Turn();
    }

    void Think()
    {
        nextMove = Random.Range(-1, 2);
        
        // 몬스터 애니메이션
        anim.SetInteger("WalkSpeed", nextMove);
        if(nextMove != 0)
            spriter.flipX = nextMove == 1;
        
        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);
    }

    void Turn()
    {
        nextMove *= -1;
        spriter.flipX = nextMove == 1;
        
        CancelInvoke();
        Invoke("Think", 3);
    }

    public void OnDamaged()
    {
        // 몬스터 사망 로직
        spriter.color = new Color(1, 1, 1, 0.4f);
        spriter.flipY = true;
        boxCollider.enabled = false;
        rigid.AddForce(Vector2.up * 3, ForceMode2D.Impulse);
        Invoke("DeActive", 3);
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }
}
