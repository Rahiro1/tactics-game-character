using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsDisplayPanel : MonoBehaviour
{
    [SerializeField]
    public List<StatDisplay> statDisplays;

    public void DisplayAllStats()
    {
        // Param will be StatSet

        int index = 0;

        foreach(StatDisplay display in statDisplays)
        {
            // if index < StatSet.Stats.count
            // display stst(SS.S[index];
        }
    }
}
