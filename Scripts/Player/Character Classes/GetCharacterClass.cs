using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[System.Serializable]
public class CharacterClass
{
    // Class Properties
    public string className;
    public bool isUsing;

    public CharacterClass(string className, bool isUsing)
    {
        this.className = className;
        this.isUsing = isUsing;
    }
}

public class GetCharacterClass : MonoBehaviour
{
    // Character List
    public List<CharacterClass> characterClasses = new List<CharacterClass>()
    {
        new CharacterClass("HooHa", false)
    };

    // Modules
    private CharacterClassManager characterClassesScript;

    private void Start()
    {
        characterClassesScript = GetComponent<CharacterClassManager>();
    }

    public void FindCharacterClass(string characterName)
    {
        foreach (CharacterClass characterClass in characterClasses)
        {
            if (characterName ==  characterClass.className)
            {
                characterClassesScript.FindClass(characterClass.className);
                Debug.Log("Found listed class");
            }
        }
    }

    public void Init(string characterName)
    {
        FindCharacterClass(characterName);
    }
}
