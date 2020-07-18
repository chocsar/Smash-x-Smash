using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameObject hitEffect;

    [System.NonSerialized] public Rigidbody2D rb2D;
    [System.NonSerialized] public Animator animator;

    private bool smashAttackEnabled = false;
    private float smashAttackStartTime = 0;
    private Collider2D bodyCollider;
    private bool ignoreLayerEnabled = false;
    private float ignoreLayerStartTime = 0;


    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        bodyCollider = GetComponentInChildren<Collider2D>();
    }

    public void FixedUpdate()
    {
        if(smashAttackEnabled)
        {
            if(Time.fixedTime - smashAttackStartTime > 0.3f)
            {
                smashAttackEnabled = false;
                rb2D.gravityScale = 10;
                bodyCollider.sharedMaterial.bounciness = 0.3f;
                bodyCollider.sharedMaterial.friction = 0.4f;
                bodyCollider.enabled = false;
                bodyCollider.enabled = true;
            }
        }

        if(ignoreLayerEnabled)
        {
            if(Time.fixedTime - ignoreLayerStartTime > 0.1f)
            {
                ignoreLayerEnabled = false;
                Physics2D.IgnoreLayerCollision(9, 10, false);
                Physics2D.IgnoreLayerCollision(10, 10, false);
            }
        }
    }

    public void NockBack(Vector2 nockBackVector, bool isSmash, bool isLastAttack)
    {
        rb2D.velocity = nockBackVector;

        if(isLastAttack)
        {
            //当たり判定のマスク（壁ハメにならないようにするため）
            ignoreLayerEnabled = true;
            ignoreLayerStartTime = Time.fixedTime;
            Physics2D.IgnoreLayerCollision(9, 10, true);
            Physics2D.IgnoreLayerCollision(10, 10, true);

            if(isSmash)
            {
                //吹っ飛びの制御
                smashAttackEnabled = true;
                smashAttackStartTime = Time.fixedTime;
                rb2D.gravityScale = 0;
                bodyCollider.sharedMaterial.bounciness = 1;
                bodyCollider.sharedMaterial.friction = 0;
                bodyCollider.enabled = false;
                bodyCollider.enabled = true;
            }  
        } 
    }

    public void ActionDamage()
    {
        animator.SetTrigger("Damage");

        //ヒットエフェクト
        // GameObject hitObject = Instantiate(hitEffect, transform.position, Quaternion.identity);
        // Destroy(hitObject, 2);
    }
}
