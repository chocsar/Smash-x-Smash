using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollider : MonoBehaviour
{
    public enum WallState
    {
        Up, Down, Right, Left
    }
    public WallState wallState = WallState.Up;
    private WallController wallController;

    private void Awake()
    {
        wallController = GetComponentInParent<WallController>();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            EnemyController enemyCtrl = col.gameObject.GetComponent<EnemyController>();

            if (enemyCtrl.isSmashNockBack) 
            {
                //攻撃直後のCollider_Downとの判定を避ける
                if(wallState == WallState.Down)
                {
                    if(Time.fixedTime - enemyCtrl.nockBackStartTime > 0.1f) 
                    {
                        enemyCtrl.expGain *= 0.5f;
                        wallController.Shake(wallState, col.gameObject.transform.position, enemyCtrl.expGain);
                    }
                }
                else
                {
                    enemyCtrl.expGain *= 0.5f;
                    wallController.Shake(wallState, col.gameObject.transform.position, enemyCtrl.expGain);
                }

            }
        }
    }
}
