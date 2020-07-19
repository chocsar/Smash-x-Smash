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

            if (enemyCtrl.nockBackEnabled) 
            {
                if(wallState != WallState.Down)
                {
                    enemyCtrl.expGain *= 0.5f;
                    wallController.Shake(wallState, col.gameObject.transform.position, enemyCtrl.expGain);
                }
                else
                {
                    if(Time.fixedTime - enemyCtrl.nockBackStartTime > 0.1f) //攻撃直後にCollider_Downとの判定を避けるため
                    {
                        enemyCtrl.expGain *= 0.5f;
                        wallController.Shake(wallState, col.gameObject.transform.position, enemyCtrl.expGain);
                    }
                }
            }
        }
    }
}
