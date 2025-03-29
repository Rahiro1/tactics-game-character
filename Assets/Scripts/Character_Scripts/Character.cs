//using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character 
{
    public string characterName;
    public int characterID = -1;
    public int classID;
    public Define.UnitAllignment unitAllignment;
    public Define.AIType AIType;
    public Define.AIType secondaryAIType;
    public Define.UnitType unitType;
    public int battalionNumber;
    public bool isSubscribedToSkills;

    // stats
    private CharacterStats characterStats;
    public LevelCounter Level { get { return characterStats.Level; } }
    public int currentHP;
    public int currentArmour;

    // equipment
    public Weapon EquippedWeapon { get; set; }
    public Armour EquippedArmour { get; set; }
    public HealingMagic EquippedHeal { get; set; }
    public List<Item> CharacterInventory = new();

    //skills
    public List<int> skillIDList { get; set; }
    private List<SkillSO> skillList;

    // basic stat getters
    public Stat MaxHP { get { return characterStats.HP; } }
    public Stat Strength { get { return characterStats.Strength; } }
    public Stat Magic { get { return characterStats.Magic; } }
    public Stat Offence { get { return characterStats.Offence; } }
    public Stat Defence { get { return characterStats.Defence; } }
    public Stat Resistance { get { return characterStats.Resistance; } }
    public Stat Speed { get { return characterStats.Speed; } }
    public Stat Move { get { return characterStats.Move; } }

    // derived stats
    public int MaxArmour { get { return GetBaseMaxArmour() + characterStats.BonusMaxArmour.GetModifiedValue(); } }
    public int Attack { get { return GetBaseAttack() + characterStats.BonusAttack.GetModifiedValue(); } }
    public int OffensiveHit {  get { return GetBaseOffensiveHit() + characterStats.BonusOffensiveHit.GetModifiedValue(); } }
    public int DefensiveHit { get { return GetBaseDefensiveHit() + characterStats.BonusDefensiveHit.GetModifiedValue(); } }
    public int Avoid { get { return GetBaseAvoid() + characterStats.BonusAvoid.GetModifiedValue(); } }
    public int CriticalRate { get { return GetBaseCriticalRate() + characterStats.BonusCriticalRate.GetModifiedValue(); } }
    public int CriticalAvoid { get { return GetBaseCriticaAvoid() + characterStats.BonusCriticalAvoid.GetModifiedValue(); } }
    public int Guard { get { return GetBaseGuard() + characterStats.BonusGuard.GetModifiedValue(); } }
    public int Wield { get { return GetBaseWield() + characterStats.BonusWield.GetModifiedValue(); } }
    public int Rending { get { return GetBaseRending() + characterStats.BonusRending.GetModifiedValue(); } }
    public int Range { get { return GetBaseRange() + characterStats.BonusRange.GetModifiedValue(); } }

    // weapon ranks
    public List<LevelCounter> WeaponRanks { get { return characterStats.WeaponRanks; } }

    public CharacterSO GetCharacterSO()
    {
        if (characterID != -1)
        {
            return RPGDatabase.Instance.CharacterDictionary[characterID];
        }
        return null;
    }

    public ClassSO GetClassSO()
    {
        return RPGDatabase.Instance.classDictionary[classID];
    }

    public List<SkillSO> GetSkillList()     // TODO find a solution for not having to make this everytime ( problem is svae/load serialisation)
    {
        if (skillList == null)
        {
            skillList = new List<SkillSO>();
        }
        if (skillIDList == null)
        {
            skillIDList = new List<int>();
        }

        if (skillList.Count == 0)
        {
            foreach (int skillID in skillIDList)
            {
                skillList.Add(RPGDatabase.Instance.skillDictionary[skillID]);
            }
        }


        return skillList;
    }

    public Sprite GetCharacterSprite()
    {
        if (GetCharacterSO() == null)
        {
            return GetClassSO().genericClassSprite;
        }
        else
        {
            return GetCharacterSO().characterSprite;
        }
    }

    public List<SkillSO> GetAllskills()
    {
        List<SkillSO> temp = new List<SkillSO>();

        temp.AddRange(GetSkillList());
        temp.AddRange(GetClassSO().classSkillList);
        if (EquippedWeapon != null)
        {
            temp.AddRange(EquippedWeapon.skillList);
        }
        if (EquippedArmour != null)
        {
            temp.AddRange(EquippedArmour.skillList);
        }

        return GetSkillList(); // TODO return ALL skills from this, from class, weapon, etc.
    }
    public int GetBaseMaxArmour()
    {
        return 0;
    }

    public int GetBaseAttack()
    {
        if (EquippedWeapon == null)
        {
            return 0;
        }
        else if (EquippedWeapon.IsMagical)
        {
            return Magic.GetModifiedValue();
        }
        else
        {
            return Strength.GetModifiedValue();
        }
    }

    public int GetBaseOffensiveHit()
    {
        if (EquippedWeapon != null)
        {
            return Offence.GetModifiedValue() * 4 + Define.BASEHITRATEOFHANDDFH; //TODO review hitrate specifics
        }
        else
        {
            return 0;
        }
    }

    public int GetBaseDefensiveHit()
    {
        if (EquippedWeapon != null)
        {
            return Defence.GetModifiedValue() * 4 + Define.BASEHITRATEOFHANDDFH;
        } else
        {
            return 0;
        }
    }

    public int GetBaseAvoid()
    {
        return Speed.GetModifiedValue() * 3 + Defence.GetModifiedValue();
    }

    public int GetBaseCriticalRate()
    {
        return Mathf.FloorToInt(characterStats.Offence.GetModifiedValue() / 2);
    }

    public int GetBaseCriticaAvoid()
    {
        // to avoid divide by zero errors and negative crit rates
        return Mathf.Max(Defence.GetModifiedValue(), 1);
    }

    public int GetBaseGuard()
    {
        return Mathf.CeilToInt(Defence.GetModifiedValue() / 2f) + currentArmour;
    }

    public int GetBaseWield()
    {
        int temp = 0;
        int wLvl;

        if (EquippedWeapon != null)
        {
            wLvl = SelectWeaponLevelType(EquippedWeapon.weaponType).Level;
            wLvl -= EquippedWeapon.weaponRank;

            temp += wLvl >= 0 ? wLvl : wLvl * 2;

            if (EquippedWeapon.secondaryWeaponType != Define.WeaponType.none)
            {
                wLvl = SelectWeaponLevelType(EquippedWeapon.secondaryWeaponType).Level;
                wLvl -= EquippedWeapon.secondaryWeaponRank;

                temp += wLvl >= 0 ? wLvl : wLvl * 2;
            }
            if (EquippedWeapon.tertiaryWeaponType != Define.WeaponType.none)
            {
                wLvl = SelectWeaponLevelType(EquippedWeapon.tertiaryWeaponType).Level;
                wLvl -= EquippedWeapon.tertiaryWeaponRank;

                temp += wLvl >= 0 ? wLvl : wLvl * 2;
            }
        }
        if (temp > 5) // TODO - add skill influence here
        {
            temp = 5;
        }

        //TODO - decide on what to do with this complicated stat
        return temp;

    }

    public int GetBaseRending()
    {
        return 0;
    }

    public int GetBaseRange()
    {
        return 0;
    }
   

    public void AddSkill(SkillSO skill)
    {
        GetSkillList().Add(skill);
        skillIDList.Add(skill.skillID);
    }

    public void RemoveSkill(SkillSO skill)
    {
        if (GetSkillList() == null)
        {
            skillList = new List<SkillSO>();
        }
        if (GetSkillList().Contains(skill))
        {
            GetSkillList().Remove(skill);
        }
        if (skillIDList.Contains(skill.skillID))
        {
            skillIDList.Remove(skill.skillID);
        }
    }
    public void OnTriggerSkill(Define.SkillTriggerType skillTriggerType)
    {
        foreach (SkillSO skill in GetSkillList())
        {
            if (skill.UpdateActivityAndIsChanged(skillTriggerType))
            {
                if (skill.IsActive(this))
                {
                    characterStats.AddModifiersFromComplexEquippable(skill, this);
                }
                else
                {
                    characterStats.RemoveModifiers(skill.skillName);
                }
            }

        }
    }
    public Character(CharacterSO character)
    {
        isSubscribedToSkills = false;
        OnUnitCreated();

        ClassSO startingClass = character.baseClass;
        classID = startingClass.classID;
        // TODO - review character constructor
        characterID = character.characterID;
        characterName = character.characterName;
        unitAllignment = character.unitAllignment;
        unitType = startingClass.unitType;
        AIType = character.AIType;
        secondaryAIType = character.SecondaryAIType;
        battalionNumber = character.battalionNumber;

        characterStats = new CharacterStats(character);

        // equip weapon, armour and set inventory
        if (character.equippedStartingWeapon != null)
        {
            Weapon tempWeapon = new Weapon(character.equippedStartingWeapon);
            if (CanEquipWeapon(tempWeapon))
            {
                EquipWeapon(tempWeapon);

            }
            CharacterInventory.Add(tempWeapon);
        }

        if(character.equippedStartingArmour != null)
        {
            Armour tempArmour = new Armour(character.equippedStartingArmour);
            if (CanEquipArmour(tempArmour))
            {
                EquipArmour(tempArmour);
            }
            CharacterInventory.Add(tempArmour);
        }
        
        foreach (ItemSO itemSO in character.startingInventory)
        {
            CharacterInventory.Add(itemSO.CreateItem());
        }

        // add skills
        foreach(SkillSO skill in startingClass.classSkillList)
        {
            AddSkill(skill);
        }
        foreach(SkillSO skill in character.uniqueSkillList)
        {
            AddSkill(skill);
        }

    }

    public Character(Define.GenericEnemyData unitData) // constructor for genric units including difficulty options to influence stats
    {
        isSubscribedToSkills = false;
        OnUnitCreated();

        characterID = -1;
        ClassSO classSO = unitData.unitClass;
        classID = classSO.classID;
        characterName = classSO.className;
        unitAllignment = unitData.allignment;
        unitType = classSO.unitType;
        AIType = unitData.AIType;
        secondaryAIType = unitData.SecondaryAIType;
        battalionNumber = unitData.battalionNumber;

        characterStats = new CharacterStats(unitData);

        CharacterInventory = new List<Item>(); // is this neccessary?
        EquipWeapon(new Weapon(unitData.equippedWeapon));
        CharacterInventory.Add(EquippedWeapon);
        EquipArmour(new Armour(unitData.equippedArmour));
        CharacterInventory.Add(EquippedArmour);
        foreach (ItemSO itemSO in unitData.startingInventory)
        {
            CharacterInventory.Add(itemSO.CreateItem());
        }

        // add skills
        foreach (SkillSO skill in classSO.classSkillList)
        {
            AddSkill(skill);
        }
    }

    private List<int> LevelUp()
    {
        List<int> levelups = new();

        int hpIncrease, strIncrease, magIncrease, offIncrease, defIncrease, resIncrease, spdIncrease;

        hpIncrease = characterStats.HP.LevelUp();
        strIncrease = characterStats.Strength.LevelUp();
        magIncrease = characterStats.Magic.LevelUp();
        offIncrease = characterStats.Offence.LevelUp();
        defIncrease = characterStats.Defence.LevelUp();
        resIncrease = characterStats.Resistance.LevelUp();
        spdIncrease = characterStats.Speed.LevelUp();

        levelups.Add(hpIncrease);
        levelups.Add(strIncrease);
        levelups.Add(magIncrease);
        levelups.Add(offIncrease);
        levelups.Add(defIncrease);
        levelups.Add(resIncrease);
        levelups.Add(spdIncrease);


        return levelups;
    }
    // og path for function calll BattleManager -> character
    // other functionality will be moved: BattleManager -> GM -> UIManager -> Character


    public void OnUnitCreated()
    {
        if (!isSubscribedToSkills)
        {
            SkillEvents.onTriggerSkill += OnTriggerSkill;
            isSubscribedToSkills = true;
        }

    }
    public void OnUnitDestroyed()
    {
        if (isSubscribedToSkills)
        {
            SkillEvents.onTriggerSkill -= OnTriggerSkill;
            isSubscribedToSkills = false;
        }

    }

    public void ClassChange(ClassSO newClass)
    {
        characterStats.ClassChange(GetClassSO(), newClass);
    }

    // also include method for calculating the amount of experience to be gained

    private List<int> GainExperience(int amount)
    {
        // TODO implement max level and possible class change at certain levels+
        if (characterStats.Level.GainExperience(amount))
        {
            return LevelUp();
        }

        return null;
    }

    public List<ExpGainInfo> TriggerBothExpGain(Character character, bool isDefeated)
    {
        List<ExpGainInfo> expGainInfos = new();

        expGainInfos.Add(TriggerExperienceGain(character, isDefeated));
        expGainInfos.AddRange(TriggerWeaponExperienceGain(character, isDefeated));

        return expGainInfos;
    }

    public ExpGainInfo TriggerExperienceGain(Character character, bool isDefeated)
    {
        

        if(unitAllignment != Define.UnitAllignment.Player)
        {
            return null;
        }

        ExpGainInfo expGainInfo;

        int expGain = CalcultateExpFromUnit(character, isDefeated);
        List<int> levelGains = GainExperience(expGain);
        if (levelGains != null)
        {
            expGainInfo = new(Level,expGain, true);
            expGainInfo.SetLevelGains(levelGains);
        }
        else
        {
            expGainInfo = new(Level, expGain, false);
        }
        

        return expGainInfo;
        
    }

    private List<ExpGainInfo> TriggerWeaponExperienceGain(Character character, bool isDefeated)
    {
        int wexpGainAmount = 0;
        ExpGainInfo pExpGainInfo = null, sExpGainInfo = null, tExpGainInfo = null;
        List<ExpGainInfo> gainList = new();
        LevelCounter weaponRank = null;
        bool hasLeveled = false;
        

        if (EquippedWeapon != null)
        {
            wexpGainAmount = CalcultateWExpFromCharacter(character, isDefeated);


            weaponRank = SelectWeaponLevelType(EquippedWeapon.weaponType);
            hasLeveled = weaponRank.GainExperience(wexpGainAmount);
            pExpGainInfo = new(weaponRank,wexpGainAmount,hasLeveled);

            if (EquippedWeapon.secondaryWeaponType != Define.WeaponType.none)
            {
                weaponRank = SelectWeaponLevelType(EquippedWeapon.secondaryWeaponType);
                hasLeveled = weaponRank.GainExperience(wexpGainAmount);
                sExpGainInfo = new(weaponRank, wexpGainAmount, hasLeveled);
            }
            if (EquippedWeapon.tertiaryWeaponType != Define.WeaponType.none)
            {
                weaponRank = SelectWeaponLevelType(EquippedWeapon.tertiaryWeaponType);
                hasLeveled = weaponRank.GainExperience(wexpGainAmount);
                tExpGainInfo = new(weaponRank, wexpGainAmount, hasLeveled);
            }

            gainList.Add(pExpGainInfo);
            gainList.Add(sExpGainInfo);
            gainList.Add(tExpGainInfo);
            return gainList;
        }
        else
        {
            return null;
        }
    }

    private int CalcultateWExpFromCharacter(Character character, bool isDestroyed)
    {
        int wexpAmount = character.CalculateWeaponProficiency() - CalculateWeaponProficiency() +1;

        int weaponTypesNumber = 1;

        if(EquippedWeapon.secondaryWeaponType != Define.WeaponType.none)
        {
            weaponTypesNumber++;
        }
        if (EquippedWeapon.tertiaryWeaponType != Define.WeaponType.none)
        {
            weaponTypesNumber++;
        }

        if (isDestroyed)
        {
            wexpAmount += wexpAmount;
        }

        wexpAmount = Mathf.CeilToInt((float)wexpAmount / weaponTypesNumber);
        wexpAmount *= Define.WEXPMULTPLIER;

        // CONSDIER option to turn off weapon exp from not destroyed units

        return wexpAmount;
    }

    public int CalculateWeaponProficiency()
    {
        int weaponTypesNumber = 1;
        if(EquippedWeapon == null)
        {
            return 0;
        }

        int weaponexp = SelectWeaponLevelType(EquippedWeapon.weaponType).Level;

        if(EquippedWeapon.secondaryWeaponType != Define.WeaponType.none)
        {
            weaponexp += SelectWeaponLevelType(EquippedWeapon.secondaryWeaponType).Level;
            weaponTypesNumber++;
        }
        if (EquippedWeapon.tertiaryWeaponType != Define.WeaponType.none)
        {
            weaponexp += SelectWeaponLevelType(EquippedWeapon.tertiaryWeaponType).Level;
            weaponTypesNumber++;
        }

        return Mathf.FloorToInt((float)weaponexp / weaponTypesNumber);
    }

    private int CalcultateExpFromUnit(Character character, bool isDefeated)
    {
        // 
        if (isDefeated)
        {
            return (characterStats.Level.Level - character.characterStats.Level.Level + 5) * Define.DESTROYEDENEMYEXPMULTPLIER;
        }
        else
        {
            return characterStats.Level.Level - character.characterStats.Level.Level + 5;
        }
    } 

    public bool CanEquipWeapon(Weapon weapon)
    {
        bool canEquip = false;

        if (weapon.weaponType == Define.WeaponType.none)
        {
            return canEquip;
        } 
        LevelCounter rankToCheck = SelectWeaponLevelType(weapon.weaponType);
        if (rankToCheck.MasteryLevel >= weapon.weaponMasteryLevel)
        {
            canEquip = true;
        }

        if(weapon.secondaryWeaponType == Define.WeaponType.none)
        {
            return canEquip;
        }
        rankToCheck = SelectWeaponLevelType(weapon.secondaryWeaponType);
        if (rankToCheck.MasteryLevel >= weapon.weaponMasteryLevel)
        {
            canEquip = true;
        }

        if (weapon.tertiaryWeaponType == Define.WeaponType.none)
        {
            return canEquip;
        }
        rankToCheck = SelectWeaponLevelType(weapon.tertiaryWeaponType);
        if (rankToCheck.MasteryLevel >= weapon.weaponMasteryLevel)
        {
            canEquip = true;
        }

        return canEquip;
    }

    public void EquipWeapon(Weapon weapon)
    {
        if (CanEquipWeapon(weapon))
        {
            EquippedWeapon = weapon;
            characterStats.AddModifiersFromEquippable(weapon);
        }
    }

    public void UnEquipWeapon()
    {
        if(EquippedWeapon != null)
        {
            characterStats.RemoveModifiers(EquippedWeapon.ItemName);
        }
        EquippedWeapon = null;
    }

    public void EquipArmour(Armour armour)
    {
        if (CanEquipArmour(armour))
        {
            EquippedArmour = armour;
            characterStats.AddModifiersFromEquippable(armour);
        }
    }

    public void UnEquipArmour()
    {
        if (EquippedArmour != null)
        {
            characterStats.RemoveModifiers(EquippedArmour.ItemName);
        }
        EquippedArmour = null;
    }

    public bool CanEquipArmour(Armour armour)
    {
        // CONSIDER - add allowedArmourList variable to character?
        List<Define.ArmourType> allowedList = RPGDatabase.Instance.classDictionary[classID].allowedArmourTypes;
        if (allowedList.Contains(armour.allowedArmourUser))
        {
            return true;
        }
        return false;
    }

    public void OnItemUse(Item item)
    {
        if (item.ReduceDurability()) // true if item use <1
        {
            if (EquippedWeapon == item)
            {
                UnEquipWeapon();
            }
            CharacterInventory.Remove(item);
        }
    }
    public void EquipHeal(HealingMagic heal)
    {
        EquippedHeal = heal;
    }

    public bool AddItemToInventory(Item item)
    {
        if(CharacterInventory.Count < 8)
        {
            CharacterInventory.Add(item);
            return true;
        } else
        {
            return false;
        }
    }

    public void RemoveItemFromInventory(Item item)
    {
        if (CharacterInventory.Contains(item))
        {
            CharacterInventory.Remove(item);
            if (EquippedWeapon == item)
            {
                UnEquipWeapon();
            }
            if (EquippedArmour == item)
            {
                UnEquipArmour();
            }
        }
    }

    public bool AddItemToInventory(ItemSO itemSO)
    {
        if (CharacterInventory.Count < 8)
        {
            CharacterInventory.Add(itemSO.CreateItem());
            return true;
        } else
        {
            return false;
        }
    }

    private LevelCounter SelectWeaponLevelType(Define.WeaponType weaponType)
    {
        // TODO
        // add foreach here

        switch (weaponType)
        {
            case Define.WeaponType.Sword:
                return characterStats.SwordRank;
            case Define.WeaponType.Polearm:
                return characterStats.SpearRank;
            case Define.WeaponType.Axe:
                return characterStats.AxeRank;
            case Define.WeaponType.Bow:
                return characterStats.BowRank;
            case Define.WeaponType.Elemental:
                return characterStats.ElementalRank;
            case Define.WeaponType.Decay:
                return characterStats.DecayRank;
            case Define.WeaponType.Armour:
                return characterStats.ArmourWeaponRank;
            case Define.WeaponType.Healing:
                return characterStats.HealRank;
            case Define.WeaponType.Creation:
                return characterStats.CreationRank;
            default:
                return null;
        }
    }

    public int DetermineSkillBonusDamage(int damage, int rending, int skillroll, out int newRending)  // maybe add status or change function
    {
        int newDamage = damage;
        newRending = rending;

        foreach(SkillSO skill in GetAllskills())
        {
            newDamage = skill.OffensiveSkillActivationBonus(newDamage, newRending, skillroll, out newRending);
        }

        return newDamage;
    } 

    //[JsonConstructor]
    public Character(string characterName, int characterID, int classID, Define.UnitAllignment unitAllignment, Define.AIType AIType, Define.AIType secondaryAIType, Define.UnitType unitType,
        int battalionNumber, bool isSubscribedToSkills, CharacterStats characterStats, int currentHP, int currentArmour, Weapon EquippedWeapon, Armour EquippedArmour, HealingMagic EquippedHeal,
        List<Item> CharacterInventory, List<int> skillIDList, List<SkillSO> skillList)  // only used for loading perposes - do not use
    {
        this.characterName = characterName;
        this.characterID = characterID;
        this.classID = classID;
        this.unitAllignment = unitAllignment;
        this.AIType = AIType;
        this.secondaryAIType = secondaryAIType;
        this.unitType = unitType;
        this.battalionNumber = battalionNumber;
        this.isSubscribedToSkills = isSubscribedToSkills;
        this.characterStats = characterStats;
        this.currentHP = currentHP;
        this.currentArmour = currentArmour;
        this.EquippedWeapon = EquippedWeapon;
        this.EquippedArmour = EquippedArmour;
        this.EquippedHeal = EquippedHeal;
        this.CharacterInventory =  CharacterInventory;
        this.skillIDList = skillIDList;
        this.skillList = skillList;
}
}
