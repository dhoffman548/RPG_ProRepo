using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class HeaderInfo : MonoBehaviourPun
{
    public TextMeshProUGUI NameText;
    public Image bar;
    private float maxValue;
    public TextMeshProUGUI LevelUpText;

    public void Initialze (string text, int maxVal)
    {
        NameText.text = text;
        maxValue = maxVal;
        bar.fillAmount = 1.0f;
    }

    [PunRPC]
    void UpdateHealthBar (int value)
    {
        bar.fillAmount = (float)value / maxValue;
    }
    
    public void DisplayLevelText ()
    {
        LevelUpText.text = "Leveled Up!";
        Invoke("HideLevelText", 3.0f);
    }

    void HideLevelText ()
    {
        LevelUpText.text = "";
    }

    public void GhostText ()
    {
        LevelUpText.text = "Ghost Speed!";
        Invoke("HideLevelText", 3.0f);
    }    
}
