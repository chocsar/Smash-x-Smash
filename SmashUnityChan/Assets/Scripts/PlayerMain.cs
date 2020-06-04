﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : MonoBehaviour
{
    //＝＝＝キャッシュ＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    PlayerController playerCtrl;

    //＝＝＝コード（Monobehabior基本機能の実装）＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

    void Awake()
    {
        playerCtrl = GetComponent<PlayerController>();
    }

    void Update()
    {
        //パッド処理
        float joyMv = Input.GetAxis("Horizontal");
        playerCtrl.ActionMove(joyMv);

        //ジャンプ
        if(Input.GetButtonDown("Jump"))
        {
            playerCtrl.ActionJump();
        }                
    }
}
