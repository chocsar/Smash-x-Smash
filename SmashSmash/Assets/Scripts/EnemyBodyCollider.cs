using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBodyCollider : MonoBehaviour
{
    EnemyController enemyCtrl;
    Animator playerAnim;
    PlayerController playerCtrl;
    PlayerStatusManager playerStatusManager;

    int attackHash = 0; //ダメージ判定を一度きりに制限するため
    
    void Awake()
    {
        enemyCtrl = GetComponentInParent<EnemyController>();
        playerAnim = GameObject.FindWithTag("Player").GetComponent<Animator>();
        playerCtrl = playerAnim.GetComponent<PlayerController>();

        playerStatusManager = playerAnim.GetComponent<PlayerStatusManager>();
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = playerAnim.GetCurrentAnimatorStateInfo(0);
        if(attackHash != 0 && stateInfo.fullPathHash == PlayerController.ANISTS_Idle)
        {
            attackHash = 0;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "PlayerArm")
        {
            AnimatorStateInfo stateInfo = playerAnim.GetCurrentAnimatorStateInfo(0);
            if(attackHash != stateInfo.fullPathHash)
            {
                attackHash = stateInfo.fullPathHash;
                enemyCtrl.ActionDamage();
                
                enemyCtrl.NockBack
                (
                    playerStatusManager.attackNockBackVector,
                    playerStatusManager.isSmash,
                    playerStatusManager.isLastAttack,
                    playerStatusManager.nockBackTimer
                );
                enemyCtrl.expGain = playerStatusManager.GetExp();
                playerStatusManager.GetSmash();
            }
        }
    }



}
