using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WallController : MonoBehaviour
{
    public GameObject expEffect;
    private Animator animator;

    private PlayerStatusManager playerStatusManager;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerStatusManager = GameObject.Find("Player").GetComponent<PlayerStatusManager>();
    }

    public void Shake(WallCollider.WallState wallState, Vector3 effectPosition, float expGain)
    {
        switch (wallState)
        {
            case WallCollider.WallState.Up:
                animator.SetTrigger("Up");
                break;
            case WallCollider.WallState.Down:
                animator.SetTrigger("Down");
                break;
            case WallCollider.WallState.Right:
                animator.SetTrigger("Right");
                break;
            case WallCollider.WallState.Left:
                animator.SetTrigger("Left");
                break;
        }

        //エフェクト表示
        if((int)expGain != 0)
        {
            GameObject effect = Instantiate(expEffect, effectPosition, Quaternion.identity) as GameObject;
            Text effectText = effect.GetComponentInChildren<Text>();
            effectText.text = "<size=2>" + ((int)expGain).ToString() + "</size><size=1>EXP</size>";
            effectText.rectTransform.localScale = new Vector3(0.3f, 0.3f, 1);
        }

        //EXP反映
        playerStatusManager.GetExpFromWall(expGain);

    }

}
