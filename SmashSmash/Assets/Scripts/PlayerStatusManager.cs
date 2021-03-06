﻿using System.Collections;
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
    [System.NonSerialized] public float nockBackTimer = 0.1f;

    //＝＝＝＝＝キャッシュ＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    [SerializeField] private Text levelText; //レベル用のTextUI
    [SerializeField] private Slider expSlider; //経験値用のSliderUI
    [SerializeField] private Slider smashSlider; //スマッシュゲージ用のSliderUI
    [SerializeField] private Animator smashUIAnimator; //スマッシュゲージのアニメータ
    [SerializeField] private GameObject expEffect; //ヒット時の経験値エフェクト

    //＝＝＝＝＝内部パラメータ＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    private int playerLevel = 1; //今のレベル
    private int playerExp = 0; //今のレベルにおける経験値量 
    private int totalPlayerExp = 0; //経験値の総量
    private int requiredExp; //次のレベルアップに必要な経験値

    private float smashValue; //今のスマッシュ値（0 ~ 100）
    private float extraSmashValue; //スマッシュモード中も加算されるEXスマッシュ値

    private float basExpGet = 10; //攻撃により得られる基本経験値
    private float basSmashGet = 10; //攻撃により得られる基本スマッシュ値
    private float lossSmash = 5; //スマッシュ値の時間減少
    private float lossSmashInSmashMode = 15; //スマッシュ値の時間減少（スマッシュモード時）

    private int conboLevel = 3; //コンボ攻撃の回数（1 ~ 3）

    private float basNocBackPower = 10; //ノックバックの基本速度
    //public float nockBackTimer_Debug = 0.1f;

    private Text generatedExpEffectText; //生成したエフェクトのTextへの参照
    private float prevGenerateEffectTime; //一つ前のエフェクト生成時刻
    private float totalExpGet; //攻撃により得られる経験値の累計



    //＝＝＝＝＝コード（Monobehavior基本機能の実装）＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    private void Awake()
    {
        //レベルの表示
        levelText.text = "Lv." + playerLevel.ToString();

        //次のレベルアップに必要な経験値の計算
        requiredExp = CalculateRequiredExp(playerLevel);

        //経験値Sliderの設定
        expSlider.maxValue = requiredExp;
        expSlider.value = playerExp;

        //スマッシュSliderの設定
        smashSlider.value = 0;

    }

    void Update()
    {
        //スマッシュモードの時
        if (isSmash)
        {
            //スマッシュゲージの時間減少
            smashValue -= lossSmashInSmashMode * Time.deltaTime;

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
            smashValue -= lossSmash * Time.deltaTime;
            if (smashValue < 0) smashValue = 0;
        }

        //UI表示
        smashSlider.value = (int)smashValue;

    }



    //＝＝＝＝＝コード（サポート関数）＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    public float GetExp()
    {
        //経験値の計算
        float expGet = 0;
        if (isSmash)
        {
            expGet = basExpGet * (5 + extraSmashValue / 20); //スマッシュ値によって5倍~に変動
        }
        else
        {
            expGet = basExpGet * (1 + smashValue / 100); //スマッシュ値によって1倍~2倍に変動
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

        return expGet;
    }

    public void GetExpFromWall(float expGain)
    {
        //経験値を加算
        playerExp += (int)expGain;
        totalPlayerExp += (int)expGain;

        //レベルアップ処理
        if (playerExp >= requiredExp)
        {
            LevelUp(playerExp - requiredExp);
        }

        //UI表示
        expSlider.value = playerExp;
    }

    public void GetSmash()
    {
        //スマッシュゲージ計算
        if (isSmash)
        {
            //スマッシュ値計算
            smashValue += basSmashGet;
            if (smashValue >= 100) smashValue = 100;
            smashSlider.value = smashValue;

            //EXスマッシュ値計算
            extraSmashValue += basSmashGet;
        }
        else
        {
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
        playerExp = extraExp;
    }

    private int CalculateRequiredExp(int level)
    {
        float requiredExp = 100 * Mathf.Pow(1.1f, level - 1) + 100 * (level - 1); //初期値100、次のレベルで1.1倍
        return (int)requiredExp;
    }

    public void CalclateAttackNockBackVector(float dir, int animationHash)
    {
        Vector2 nockBack = Vector2.zero;

        //ラストアタックかどうか判定
        if (animationHash == PlayerAnimationHash.AttackB ||
            animationHash == PlayerAnimationHash.Jump)
        {
            //攻撃Cとジャンプ攻撃
            isLastAttack = true;
        }
        else if (animationHash == PlayerAnimationHash.AttackA)
        {
            //攻撃B
            isLastAttack = (conboLevel == 2) ? true : false;
        }
        else
        {
            //攻撃A
            isLastAttack = (conboLevel == 1) ? true : false;
        }

        //ノックバック速度・時間の計算
        if (isLastAttack)
        {
            if (isSmash)
            {
                nockBack = new Vector2(dir * Mathf.Cos(Mathf.PI / 4), Mathf.Sin(Mathf.PI / 4)) *
                    basNocBackPower * Mathf.Min(2f + extraSmashValue / 100, 10.0f); //2~10倍まで変動（限界なしにする？）

                // nockBack = new Vector2(dir * Mathf.Cos(Mathf.PI / 4), Mathf.Sin(Mathf.PI / 4)) *
                //     basNocBackPower * (2f + extraSmashValue / 100);

                //nockBack = new Vector2(dir * Mathf.Cos(Mathf.PI / 4), Mathf.Sin(Mathf.PI / 4)) * 10;

                nockBackTimer = (nockBack.magnitude <= 30) ? 0.1f : nockBack.magnitude / 100 - 0.2f; //最大でも1sくらいにする？

                //nockBackTimer = 3;
            }
            else
            {
                nockBack = new Vector2(dir * Mathf.Cos(Mathf.PI / 4), Mathf.Sin(Mathf.PI / 4)) *
                    basNocBackPower * (1 + smashValue / 100); //1~2倍まで変動

                nockBackTimer = 0.1f;
            }
        }
        else
        {
            nockBack = new Vector2(0, 1) * 15;
        }

        attackNockBackVector = nockBack;

        //デバッグ用
        //isLastAttack = true;
        //nockBackTimer = nockBackTimer_Debug;
        //attackNockBackVector = new Vector2(dir * Mathf.Cos(Mathf.PI / 4), Mathf.Sin(Mathf.PI / 4)) * basNocBackPower;

    }


}
