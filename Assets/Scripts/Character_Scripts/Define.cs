using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define 
{
    public const int WEXPMULTPLIER = 5;
    public const int BASEHITRATEOFHANDDFH = 80;


    public enum WeaponType
    {
        none = 0,
        Sword = 1,
        Polearm = 2,
        Axe = 3,
        Bow = 4,
        Elemental = 5,
        Healing = 6,
        Armour = 7,
        Creation = 8,
        Decay = 9,
        Natural = 10
    }

    public enum ArmourType
    {
        Cloth = 0,
        Light = 1,
        Medium = 2,
        Heavy = 3,
        LightBarding = 4,
        MediumBarding = 5,
        HeavyBarding = 6,
        Hide = 7,
        Dragonskin = 8
    }

    public enum UnitType // currently being used for both unit type and unit move type, consider changig. when changing this remember to change the scriptable objects assigned to the player data reference and the mapTileController switch statement
    {
        Foot = 0,
        Armour = 1,
        Water = 2,
        Flying = 3,
        Beast = 4
    }

    public enum UnitAllignment
    {
        Player = 0,
        Enemy = 1,
        Ally = 2,
        Other = 3
    }

    public enum AIType // to add new AI update the AI manager, including the switch statement
    {
        Wait = 0,
        charge = 1,
        AttackRange = 2,
        Thief = 3
    }

    public enum SkillTriggerType
    {
        BattleStart = 0,
        BattleEnd = 1,
        TakeDamage = 2,
        LevelStart = 3,
        LevelEnd = 4,
        None = 5
    }

    [System.Serializable]
    public struct GenericEnemyData // can be used for any generic unit
    {
        public int level;
        public ClassSO unitClass;
        public UnitAllignment allignment;
        public AIType AIType;
        public AIType SecondaryAIType;
        public int battalionNumber;
        public Vector3Int position;
        public WeaponSO equippedWeapon;
        public ArmourSO equippedArmour;
        public List<ItemSO> startingInventory;
    }
}
