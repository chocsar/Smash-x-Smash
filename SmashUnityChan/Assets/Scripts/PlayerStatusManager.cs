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
    
    //public Slider smashSlider;
    //private int smashValue;

    void Start()
    {
        levelText.text = "Lv." + playerLevel.ToString();
        requiredExp = CalcurateRequiredExp(playerLevel);
        expSlider.maxValue = requiredExp;
        expSlider.value = playerExp;

        //smashSlider.value = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddExp(int exp)
    {
        playerExp += exp;
        totalPlayerExp += exp;

        if(playerExp >= requiredExp)
        {
            LevelUp(playerExp - requiredExp);
        }
        else
        {
            expSlider.value = playerExp;
        }
    }

    private void LevelUp(int exp)
    {
        playerLevel++;
        levelText.text = "Lv." + playerLevel.ToString();

        playerExp = exp;
        expSlider.value = playerExp;
        requiredExp = CalcurateRequiredExp(playerLevel);
        expSlider.maxValue = requiredExp;

        
    }

    private int CalcurateRequiredExp(int level)
    {
        return 1000 + (level-1) * 200;
    }
}
