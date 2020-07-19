using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameObject hitEffect;

    [System.NonSerialized] public Rigidbody2D rb2D;
    [System.NonSerialized] public Animator animator;

    [System.NonSerialized] public bool nockBackEnabled = false;
    [System.NonSerialized] public float expGain = 0;

    [System.NonSerialized] public float nockBackStartTime = 0;
    private Collider2D bodyCollider;
    private bool ignoreLayerEnabled = false;
    private float ignoreLayerStartTime = 0;
    private float nockBackTimer = 0;

    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        bodyCollider = GetComponentInChildren<Collider2D>();
    }

    public void FixedUpdate()
    {
        if(nockBackEnabled)
        {
            if(Time.fixedTime - nockBackStartTime > nockBackTimer)
            {
                nockBackEnabled = false;
                nockBackTimer = 0;
                rb2D.gravityScale = 10;
                bodyCollider.sharedMaterial.bounciness = 0.3f;
                bodyCollider.sharedMaterial.friction = 0.4f;
                bodyCollider.enabled = false;
                bodyCollider.enabled = true;
            }
        }

        if(ignoreLayerEnabled)
        {
            if(Time.fixedTime - ignoreLayerStartTime > nockBackTimer)
            {
                ignoreLayerEnabled = false;
                Physics2D.IgnoreLayerCollision(9, 10, false);
            }
        }
    }

    public void NockBack(Vector2 nockBackVector, bool isSmash, bool isLastAttack, float timer)
    {
        rb2D.velocity = nockBackVector;

        if(isLastAttack)
        {
            //当たり判定のマスク（壁ハメにならないようにするため）
            ignoreLayerEnabled = true;
            ignoreLayerStartTime = Time.fixedTime;
            Physics2D.IgnoreLayerCollision(9, 10, true); //PlayerBodyとEnemyBody

            if(isSmash)
            {
                //吹っ飛びの制御
                nockBackEnabled = true;
                nockBackStartTime = Time.fixedTime;
                nockBackTimer = timer;

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
