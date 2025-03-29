using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsDisplayPanel : MonoBehaviour
{
    [SerializeField]
    public List<StatDisplay> statDisplays;

    public void DisplayAllStats(Character character)
    {
        statDisplays[0].DisplayStat("MaxHP", character.MaxHP.GetModifiedValue());
        statDisplays[0].DisplayStat("Strength", character.Strength.GetModifiedValue());
        statDisplays[0].DisplayStat("Magic", character.Magic.GetModifiedValue());
        statDisplays[0].DisplayStat("OFF", character.Offence.GetModifiedValue());
        statDisplays[0].DisplayStat("DEF", character.Defence.GetModifiedValue());
        statDisplays[0].DisplayStat("RES", character.Resistance.GetModifiedValue());
        statDisplays[0].DisplayStat("Speed", character.Speed.GetModifiedValue());
        statDisplays[0].DisplayStat("Move", character.Move.GetModifiedValue());
        statDisplays[0].DisplayStat("Armour", character.MaxArmour);
        statDisplays[0].DisplayStat("Attack", character.Attack);
        statDisplays[0].DisplayStat("OFH", character.OffensiveHit);
        statDisplays[0].DisplayStat("DFH", character.DefensiveHit);
        statDisplays[0].DisplayStat("Crit", character.CriticalRate);
        statDisplays[0].DisplayStat("CritAvoid", character.CriticalAvoid);
        statDisplays[0].DisplayStat("Guard", character.Guard);
        statDisplays[0].DisplayStat("Wield", character.Wield);
        statDisplays[0].DisplayStat("Rending", character.Rending);
        statDisplays[0].DisplayStat("Range", character.Range);
    }
}
