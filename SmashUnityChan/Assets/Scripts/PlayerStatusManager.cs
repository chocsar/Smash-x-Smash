﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusManager : MonoBehaviour
{
    //＝＝＝＝＝外部パラメータ（Inspector表示）＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

    //＝＝＝＝＝外部パラメータ＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    [System.NonSerialized] public bool smashFlag = false; //trueでスマッシュモード

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

    private GameObject generatedExpEffect;
    private Text generatedExpEffectText;
    private float prevGenerateEffectTime;
    private float totalExpGet;

   
    //＝＝＝＝＝コード（Monobehavior基本機能の実装）＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    private void Awake()
    {
        //レベルの表示
        levelText.text = "Lv." + playerLevel.ToString();

        //次のレベルアップに必要な経験値の計算
        requiredExp = CalcurateRequiredExp(playerLevel);

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
        if(smashFlag)
        {
            //スマッシュゲージの時間減少
            smashValue -= 20 * Time.deltaTime;

            //スマッシュモードの解除
            if(smashValue < 0)
            { 
                smashValue = 0;
                extraSmashValue = 0;

                smashFlag = false;
                smashUIAnimator.SetBool("SmashMode", smashFlag);
            }

            //UI表示
            smashSlider.value = (int)smashValue;

        }
        //スマッシュモードじゃない時
        else
        {
            //スマッシュゲージの時間減少
            smashValue -= 5 * Time.deltaTime;
            if(smashValue < 0) smashValue = 0;
        }

        //UI表示
        smashSlider.value = (int)smashValue;
        
    }



    //＝＝＝＝＝コード（サポート関数）＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    public void OnAttackHit()
    {
        //経験値,スマッシュゲージ計算
        float expGet = 0;
        if(smashFlag)
        {
            //経験値計算
            expGet = basExpGet * (5 + extraSmashValue/20); //スマッシュ値によって5倍~に変動
            
            //スマッシュ値計算
            smashValue += basSmashGet;
            if(smashValue >= 100) smashValue = 100;
            smashSlider.value = smashValue;
            
            //EXスマッシュ値計算
            extraSmashValue += basSmashGet;
        }
        else
        {
            //経験値計算
            expGet = basExpGet * (1 + smashValue/100); //スマッシュ値によって1倍~2倍に変動
            
            //スマッシュ値計算
            smashValue += basSmashGet;
            if(smashValue >= 100)
            {
                smashValue = 100;

                //スマッシュモード
                smashFlag = true;
                smashUIAnimator.SetBool("SmashMode", smashFlag);
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
        if(Time.fixedTime > prevGenerateEffectTime + 0.1f)
        {
            prevGenerateEffectTime = Time.fixedTime;
            generatedExpEffect = Instantiate(expEffect, transform.position + new Vector3(0, 2, 0) ,Quaternion.identity) as GameObject;
            generatedExpEffectText = generatedExpEffect.GetComponentInChildren<Text>();
            totalExpGet = expGet;
        }
        else
        {
            totalExpGet += expGet;
        }
        generatedExpEffectText.text = ((int)totalExpGet).ToString() + "EXP";   

    }

    private void LevelUp(int extraExp)
    {
        //レベルアップ処理
        playerLevel++;
        requiredExp = CalcurateRequiredExp(playerLevel);

        while(extraExp > requiredExp) //同時に2以上レベルが上がる時の備えて、ループ処理
        {
            playerLevel++;
            extraExp -= requiredExp;
            requiredExp = CalcurateRequiredExp(playerLevel);
        }

        //UI設定
        levelText.text = "Lv." + playerLevel.ToString();
        expSlider.maxValue = requiredExp;

        //余り分を今の経験値量に反映
        playerExp = extraExp; //あまり分を今の経験値量に反映
    }

    private int CalcurateRequiredExp(int level)
    {
        float requiredExp = 100 * Mathf.Pow(1.1f, level-1); //初期値100、次のレベルで1.1倍
        return  (int)requiredExp;
    }
    
    // private IEnumerator SmashMode()
    // {
    //     smashFlag = true;
    //     smashUIAnimator.SetBool("SmashMode", smashFlag);
    //     yield return new WaitForSeconds(10);
    //     smashValue = 0;
    //     smashSlider.value = 0;
    //     smashFlag = false;
    //     smashUIAnimator.SetBool("SmashMode", smashFlag);
    // }

}
