using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI skillText;
    public Button BlueButton;
    public Button RedButton;

    // instance
    public static GameUI instance;

    void Awake ()
    {
        instance = this;
    }

    public void UpdateGoldText (int gold)
    {
        goldText.text = "<b>Gold:</b> " + gold;
    }

    public void SkillMenu ()
    {
        skillText.enabled = true;
        BlueButton.enabled = true;
        RedButton.enabled = true;
    }
    void OnRedButton ()
    {

        Invoke("CloseSkillMenu", 0.1f);
    }

    void OnBlueButton ()
    {
        Invoke("CloseSkillMenu", 0.1f);
    }

    void CloseSkillMenu ()
    {
        skillText.enabled = false;
        BlueButton.enabled = false;
        RedButton.enabled = false;
    }
}
