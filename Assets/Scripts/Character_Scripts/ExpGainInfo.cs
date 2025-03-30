using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpGainInfo
{
    public LevelCounter LevelCounter { get; private set; }
    public int ExpGained { get; private set; }
    public bool HasLeveled { get; private set; }
    public List<int> LevelUpGains { get; private set; }

    public ExpGainInfo(LevelCounter levelCounter, int expGained, bool hasLeveled)
    {
        LevelCounter = levelCounter;
        ExpGained = expGained;
        HasLeveled = hasLeveled;
        LevelUpGains = new();
    }

    public void SetLevelGains(List<int> gains)
    {
        LevelUpGains = gains;
    }

}
