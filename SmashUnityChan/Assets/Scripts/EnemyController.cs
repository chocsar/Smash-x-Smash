using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameObject hitEffect;

    [System.NonSerialized] public Rigidbody2D rb2D;
    [System.NonSerialized] public Animator animator;

    private float speedVx = 0.0f;
    private bool addForceEnabled = false;
    private float addForceStartTime = 0;

    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

    }

    public void FixedUpdate()
    {
        if (addForceEnabled)
        {
            if (Time.fixedTime - addForceStartTime > 0.5f)
            {
                addForceEnabled = false;
            }
        }
        else
        {
            //rb2D.velocity = new Vector2(speedVx, rb2D.velocity.y);
        }


    }

    public void NockBack(Vector2 nockBackVector)
    {
        rb2D.AddForce(nockBackVector, ForceMode2D.Force);
        addForceEnabled = true;
        addForceStartTime = Time.fixedTime;
    }
    public void ActionDamage()
    {
        animator.SetTrigger("Damage");

        //ヒットエフェクト
        // GameObject hitObject = Instantiate(hitEffect, transform.position, Quaternion.identity);
        // Destroy(hitObject, 2);
    }
}
