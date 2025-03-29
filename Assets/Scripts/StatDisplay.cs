using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatDisplay : MonoBehaviour
{
    public TextMeshProUGUI statNameText, statValueText;

    public void DisplayStat(string statName, int statValue)
    {
        statNameText.text = statName;
        statValueText.text = statValue.ToString();
    }
}
