using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsDisplayPanel : MonoBehaviour
{
    [SerializeField]
    public List<StatDisplay> statDisplays;
    public CharacterSO displayCharacter;
    public Character character; 


    private void Start()
    {
        if (displayCharacter != null)
        {
            character = new Character(displayCharacter);
            DisplayAllStats(character);
        }
    }

    public void DisplayAllStats(Character character)
    {
        statDisplays[0].DisplayStat("MaxHP", character.MaxHP.GetModifiedValue());
        statDisplays[1].DisplayStat("Strength", character.Strength.GetModifiedValue());
        statDisplays[2].DisplayStat("Magic", character.Magic.GetModifiedValue());
        statDisplays[3].DisplayStat("OFF", character.Offence.GetModifiedValue());
        statDisplays[4].DisplayStat("DEF", character.Defence.GetModifiedValue());
        statDisplays[5].DisplayStat("RES", character.Resistance.GetModifiedValue());
        statDisplays[6].DisplayStat("Speed", character.Speed.GetModifiedValue());
        statDisplays[7].DisplayStat("Move", character.Move.GetModifiedValue());
        statDisplays[8].DisplayStat("Armour", character.MaxArmour);
        statDisplays[9].DisplayStat("Attack", character.Attack);
        statDisplays[10].DisplayStat("OFH", character.OffensiveHit);
        statDisplays[11].DisplayStat("DFH", character.DefensiveHit);
        statDisplays[12].DisplayStat("Crit", character.CriticalRate);
        statDisplays[13].DisplayStat("CritAvoid", character.CriticalAvoid);
        statDisplays[14].DisplayStat("Guard", character.Guard);
        statDisplays[15].DisplayStat("Wield", character.Wield);
        statDisplays[16].DisplayStat("Rending", character.Rending);
        statDisplays[17].DisplayStat("Range", character.Range);
    }

    public void LevelUp()
    {
        character.TriggerExperienceGain(100);
        DisplayAllStats(character);
    }
}
