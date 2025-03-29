using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGDatabase : MonoBehaviour
{
    public static RPGDatabase Instance;
    // this class is simply for storing assets that will be used by playerdata, such as creating character for the first time - has been extended to initialise dictionaries for the project
    public List<CharacterSO> characterList;
    public Dictionary<int, CharacterSO> CharacterDictionary { get; private set; }
    public List<ClassSO> classList;
    public Dictionary<int, ClassSO> classDictionary { get; private set; }
    public List<SkillSO> skillsList;
    public Dictionary<int, SkillSO> skillDictionary { get; private set; }
    public List<ItemSO> itemList;
    public Dictionary<int, ItemSO> itemDictionary { get; private set; }

    private void Awake()
    {
        // singleton code
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;

        //assigning to dictionaries
        CharacterDictionary = new Dictionary<int, CharacterSO>();
        classDictionary = new Dictionary<int, ClassSO>();
        itemDictionary = new Dictionary<int, ItemSO>();
        skillDictionary = new Dictionary<int, SkillSO>();

        foreach (CharacterSO character in characterList)
        {
            CharacterDictionary.Add(character.characterID, character);
        }
        foreach (ClassSO classSO in classList)
        {
            classDictionary.Add(classSO.classID, classSO);
        }
        foreach (ItemSO item in itemList)
        {
            itemDictionary.Add(item.itemID, item);
        }
        foreach (SkillSO skill in skillsList)
        {
            skillDictionary.Add(skill.skillID, skill);
        }
    }

    private void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }
}
