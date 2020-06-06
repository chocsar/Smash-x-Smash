﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //＝＝＝外部パラメータ（Inspector表示）＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    public float speed = 10.0f;

    //＝＝＝外部パラメータ＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    [System.NonSerialized] public float dir = 1.0f;
    [System.NonSerialized] public bool jumped = false;
    [System.NonSerialized] public bool grounded = false;
    [System.NonSerialized] public bool groundedPrev = false;

    //アニメーションのハッシュ名
    public static int ANISTS_Idle = Animator.StringToHash("Base Layer.Player_Idle");
    public static int ANISTS_Walk = Animator.StringToHash("Base Layer.Player_Walk");
    public static int ANISTS_Run = Animator.StringToHash("Base Layer.Player_Run");
    public static int ANISTS_Jump = Animator.StringToHash("Base Layer.Player_Jump");
    public static int ANISTS_Attack_A = Animator.StringToHash("Base Layer.Player_Attack_A");
    public static int ANISTS_Attack_B = Animator.StringToHash("Base Layer.Player_Attack_B");
    public static int ANISTS_Attack_C = Animator.StringToHash("Base Layer.Player_Attack_C");

    
    //＝＝＝キャッシュ＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    [System.NonSerialized] public Animator animator;
    [System.NonSerialized] public Rigidbody2D rb2D;
    private Transform groundCheck_L;
    private Transform groundCheck_C;
    private Transform groundCheck_R;

    private Transform spriteTransform;
    private Transform bodyColliderTransform;

    //＝＝＝内部パラメータ＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    private float speedVx = 0.0f;
    private int jumpCount = 0;
    private Vector3 initSpritePos;
    private Vector3 initBodyColliderPos;

    private bool atkInputEnabled = false; //trueの間、コンボ入力受付
    private bool atkInputNow = false; //コンボ入力が行われたかどうか


    //＝＝＝コード（Monobehavior基本機能の実装）＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();

        groundCheck_L = transform.Find("GroundCheck_L");
        groundCheck_C = transform.Find("GroundCheck_C");
        groundCheck_R = transform.Find("GroundCheck_R");

        Debug.Log(groundCheck_L);

        dir = (transform.localScale.x > 0.0f) ? 1.0f : -1.0f;

        spriteTransform = transform.Find("PlayerSprite");
        bodyColliderTransform = transform.Find("Collider_Body");
        initSpritePos = spriteTransform.localPosition;
        initBodyColliderPos = bodyColliderTransform.localPosition;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //地面チェック
        groundedPrev = grounded;
        grounded = false;

        Collider2D[][] groundCheckCollider = new Collider2D[3][]; //ジャグ配列
        groundCheckCollider[0] = Physics2D.OverlapPointAll(groundCheck_C.position);
        groundCheckCollider[1] = Physics2D.OverlapPointAll(groundCheck_L.position);
        groundCheckCollider[2] = Physics2D.OverlapPointAll(groundCheck_R.position);

        foreach(Collider2D[] groundCheckList in groundCheckCollider)
        {
            foreach(Collider2D groundCheck in groundCheckList)
            {
                if(groundCheck != null)
                {
                    if(!groundCheck.isTrigger)
                    {
                        grounded = true;
                    }
                }
            }
        }

        //キャラクター個別の処理
        FixedUpdateCharacter();

        //移動計算
        rb2D.velocity = new Vector2(speedVx, rb2D.velocity.y);
    
    }

    private void FixedUpdateCharacter()
    {
        //現在のステートを取得
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //ジャンプ中のとき
        if(jumped)
        {
            //着地チェック
            if(grounded && !groundedPrev)
            {
                jumped = false;
                jumpCount = 0;
            }
        }
        //地面にいるとき
        else
        {
            jumpCount = 0;
        }

        //攻撃中の移動停止
        if(stateInfo.fullPathHash == ANISTS_Attack_A ||
            stateInfo.fullPathHash == ANISTS_Attack_B ||
            stateInfo.fullPathHash == ANISTS_Attack_C)
        {
            speedVx = 0;
        }
    
        //キャラの方向
        transform.localScale = new Vector3(dir, transform.localScale.y, transform.localScale.z);

        //ジャンプ中の横移動減速→とりあえず保留

        //カメラ
        //Camera.main.transform.position = transform.position + new Vector3(0, 4, -1);
    }

    //＝＝＝コード（アニメーションイベント用コード）＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    public void ResetPosition() //Sprite, Colliderの位置変化を親にも反映する
    {
        Vector3 currentSpritePos = spriteTransform.localPosition;
        Vector3 diff = currentSpritePos - initSpritePos;

        spriteTransform.localPosition = initSpritePos;
        bodyColliderTransform.localPosition = initBodyColliderPos;

        transform.position += diff * dir;
    }

    //＝＝＝コード（基本アクション）＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    public void ActionMove(float n)
    {
        //アニメーションの指定
        float moveSpeed = Mathf.Abs(n);
        animator.SetFloat("MoveSpeed", moveSpeed);

        //移動
        if(n != 0.0f)
        {
            dir = Mathf.Sign(n);
            moveSpeed = (moveSpeed < 0.5f) ? (moveSpeed * (1.0f/0.5f)) : 1.0f;
            speedVx = speed * moveSpeed * dir;
        }
        else
        {
            speedVx = 0;
        }
    }

    public void ActionJump()
    {
        switch(jumpCount)
        {
            case 0:
            if(grounded)
            {
                animator.SetTrigger("Jump");
                rb2D.velocity = Vector2.up * 30.0f;
                jumped = true;
                jumpCount++;
            }
            break;

            case 1:
            if(!grounded)
            {
                animator.Play("Player_Jump", 0, 0.0f);
                rb2D.velocity = Vector2.up * 20.0f;
                jumped = true;
                jumpCount++;
            }
            break;
        }
    }

    public void ActionAttack()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if()
        animator.SetTrigger("Attack_A");
    }
}
