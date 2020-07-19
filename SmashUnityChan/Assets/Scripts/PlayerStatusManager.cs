using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusManager : MonoBehaviour
{
    //＝＝＝＝＝外部パラメータ（Inspector表示）＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

    //＝＝＝＝＝外部パラメータ＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    [System.NonSerialized] public bool isSmash = false; //trueでスマッシュモード
    [System.NonSerialized] public bool isLastAttack = false;
    [System.NonSerialized] public Vector2 attackNockBackVector = Vector2.zero;

    //＝＝＝＝＝キャッシュ＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    public Text levelText; //レベル用のTextUI
    public Slider expSlider; //経験値用のSliderUI
    public Slider smashSlider; //スマッシュゲージ用のSliderUI
    private Animator smashUIAnimator; //スマッシュゲージのアニメータ

    public GameObject expEffect; //ヒット時の経験値エフェクト

    //＝＝＝＝＝内部パラメータ＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    private int playerLevel = 1; //今のレベル
    private int playerExp = 0; //今のレベルにおける経験値量 
    private int totalPlayerExp = 0; //経験値の総量
    private int requiredExp; //次のレベルアップに必要な経験値
    private float smashValue; //今のスマッシュ値（0 ~ 100）
    private float extraSmashValue; //スマッシュモード中も加算されるEXスマッシュ値

    private float basExpGet = 10; //攻撃により得られる経験値
    private float basSmashGet = 10; //攻撃により得られるスマッシュ値

    private Text generatedExpEffectText; //生成したエフェクトのTextへの参照
    private float prevGenerateEffectTime; //一つ前のエフェクト生成時刻
    private float totalExpGet; //攻撃により得られる経験値の累計

    private int conboLevel = 3; //コンボ攻撃の回数（1 ~ 3）
    


    //＝＝＝＝＝コード（Monobehavior基本機能の実装）＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    private void Awake()
    {
        //レベルの表示
        levelText.text = "Lv." + playerLevel.ToString();

        //次のレベルアップに必要な経験値の計算
        requiredExp = CalculateRequiredExp(playerLevel);

        //経験値スライダーの設定
        expSlider.maxValue = requiredExp;
        expSlider.value = playerExp;

        //スマッシュスライダーの設定
        smashSlider.value = 0;

        //キャッシュ
        smashUIAnimator = smashSlider.transform.parent.GetComponent<Animator>();
    }

    void Start()
    {

    }

    void Update()
    {
        //スマッシュモードの時
        if (isSmash)
        {
            //スマッシュゲージの時間減少
            smashValue -= 10 * Time.deltaTime;

            //スマッシュモードの解除
            if (smashValue < 0)
            {
                smashValue = 0;
                extraSmashValue = 0;

                isSmash = false;
                smashUIAnimator.SetBool("SmashMode", isSmash);
            }
        }
        //スマッシュモードじゃない時
        else
        {
            //スマッシュゲージの時間減少
            smashValue -= 5 * Time.deltaTime;
            if (smashValue < 0) smashValue = 0;
        }

        //UI表示
        smashSlider.value = (int)smashValue;

    }



    //＝＝＝＝＝コード（サポート関数）＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    public void OnAttackHit()
    {
        //経験値,スマッシュゲージ計算
        float expGet = 0;
        if (isSmash)
        {
            //経験値計算
            expGet = basExpGet * (5 + extraSmashValue / 20); //スマッシュ値によって5倍~に変動

            //スマッシュ値計算
            smashValue += basSmashGet;
            if (smashValue >= 100) smashValue = 100;
            smashSlider.value = smashValue;

            //EXスマッシュ値計算
            extraSmashValue += basSmashGet;
        }
        else
        {
            //経験値計算
            expGet = basExpGet * (1 + smashValue / 100); //スマッシュ値によって1倍~2倍に変動

            //スマッシュ値計算
            smashValue += basSmashGet;
            if (smashValue >= 100)
            {
                smashValue = 100;

                //スマッシュモード
                isSmash = true;
                smashUIAnimator.SetBool("SmashMode", isSmash);
            }
            smashSlider.value = smashValue;
        }

        //経験値を加算
        playerExp += (int)expGet;
        totalPlayerExp += (int)expGet;

        //レベルアップ処理
        if (playerExp >= requiredExp)
        {
            LevelUp(playerExp - requiredExp);
        }

        //UI表示
        expSlider.value = playerExp;

        //エフェクト表示  
        if (Time.fixedTime > prevGenerateEffectTime + 0.1f)
        {
            prevGenerateEffectTime = Time.fixedTime;
            GameObject generatedEffect = Instantiate(expEffect, transform.position + new Vector3(0, 2, 0), Quaternion.identity) as GameObject;
            generatedExpEffectText = generatedEffect.GetComponentInChildren<Text>();
            totalExpGet = expGet;
        }
        else
        {
            totalExpGet += expGet;
        }
        generatedExpEffectText.text = "<size=2>" + ((int)totalExpGet).ToString() + "</size><size=1>EXP</size>";

    }

    private void LevelUp(int extraExp)
    {
        //レベルアップ処理
        playerLevel++;
        requiredExp = CalculateRequiredExp(playerLevel);

        while (extraExp > requiredExp) //同時に2以上レベルが上がる時の備えて、ループ処理
        {
            playerLevel++;
            extraExp -= requiredExp;
            requiredExp = CalculateRequiredExp(playerLevel);
        }

        //UI設定
        levelText.text = "Lv." + playerLevel.ToString();
        expSlider.maxValue = requiredExp;

        //余り分を今の経験値量に反映
        playerExp = extraExp; //あまり分を今の経験値量に反映
    }

    private int CalculateRequiredExp(int level)
    {
        float requiredExp = 100 * Mathf.Pow(1.1f, level - 1); //初期値100、次のレベルで1.1倍
        return (int)requiredExp;
    }

    public void CalclateAttackNockBackVector(float dir, int ANISTS)
    {
        Vector2 nockBack = Vector2.zero;
        if(ANISTS == PlayerController.ANISTS_Attack_A)
        {
            //攻撃B
            if(conboLevel == 2)
            {  
                isLastAttack = true;
                nockBack = new Vector2(dir * 20, 20);
            }
            else
            {
                isLastAttack = false;
                nockBack = new Vector2(dir * 0, 15);
            }
        }
        else if(ANISTS == PlayerController.ANISTS_Attack_B)
        {
            //攻撃C
            isLastAttack = true;
            nockBack = new Vector2(dir * 30, 30);
         
        }
        else if(ANISTS == PlayerController.ANISTS_Jump)
        {
            //ジャンプ攻撃
            isLastAttack = true;
            nockBack = new Vector2(dir * 30, 30);
        }
        else
        {
            //攻撃A
            if(conboLevel == 1)
            {  
                isLastAttack = true;
                nockBack = new Vector2(dir * 15, 15);
            }
            else
            {
                isLastAttack = false;
                nockBack = new Vector2(dir * 0, 15);
            }
        }

        if(isSmash && isLastAttack) 
        {
            //スマッシュモードなら吹っ飛び力UP
            nockBack *= Mathf.Min(1 + extraSmashValue/1000, 1.5f); //最大1.5倍まで
        }

        attackNockBackVector = nockBack;
    }


}
