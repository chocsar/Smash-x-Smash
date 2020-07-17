using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusManager : MonoBehaviour
{
    private int playerLevel = 1;
    private int playerExp = 0;
    private int totalPlayerExp = 0;
    public Text levelText;
    public Slider expSlider;
    private int requiredExp;

    public Slider smashSlider;
    private float smashValue;
    [System.NonSerialized] public bool smashFlag = false;
    Animator smashUIAnimator;

    void Start()
    {
        levelText.text = "Lv." + playerLevel.ToString();
        requiredExp = CalcurateRequiredExp(playerLevel);
        expSlider.maxValue = requiredExp;
        expSlider.value = playerExp;

        smashSlider.value = 0;

        smashUIAnimator = smashSlider.transform.parent.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!smashFlag)
        {
            smashValue -= 5 * Time.deltaTime;
            if(smashValue < 0)
            {
                smashValue = 0;
            }
            smashSlider.value = (int)smashValue;
        }
        
    }

    public void OnAttackHit(int exp, int smash)
    {

        //経験値
        playerExp += exp;
        totalPlayerExp += exp;

        if (playerExp >= requiredExp)
        {
            //レベルアップ処理
            LevelUp(playerExp - requiredExp);
        }
        else
        {
            expSlider.value = playerExp;
        }

        //スマッシュゲージ
        if (smashValue + smash >= 100)
        {
            smashValue = 100;
            smashSlider.value = smashValue;
            StartCoroutine(SmashMode());
        }
        else
        {
            smashValue += smash;
        }
    }

    private void LevelUp(int extraExp)
    {
        playerLevel++;
        levelText.text = "Lv." + playerLevel.ToString();

        playerExp = extraExp;
        expSlider.value = playerExp;
        requiredExp = CalcurateRequiredExp(playerLevel);
        expSlider.maxValue = requiredExp;
    }

    private int CalcurateRequiredExp(int level)
    {
        return 1000 + (level - 1) * 200;
    }

    private IEnumerator SmashMode()
    {
        smashFlag = true;
        smashUIAnimator.SetBool("SmashMode", smashFlag);
        yield return new WaitForSeconds(10);
        smashValue = 0;
        smashSlider.value = 0;
        smashFlag = false;
        smashUIAnimator.SetBool("SmashMode", smashFlag);
    }
}
