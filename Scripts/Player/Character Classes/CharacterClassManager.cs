using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;    
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class ClassBool
{
    public string name;
    public bool value;

    public ClassBool(string characterName, bool value) 
    {
        this.name = characterName;
        this.value = value;
    }
}

public class CharacterClassManager : MonoBehaviour
{
    // Character List
    [SerializeField] public List<ClassBool> characterNames = new List<ClassBool>()
    {
        new ClassBool("HooHa", false)
    };

    public void FindClass(string className)
    {
        if (className == null) return;

        // Player
        GameObject player = GameObject.FindGameObjectWithTag("Player").gameObject;

        Type type = Type.GetType(className + ", Assembly-CSharp");

        // Checks if class exists
        foreach (var characterName in characterNames)
        {
            if (characterName.name == className)
            {
                player.AddComponent(type);
                characterName.value = true;
            }
        }
    }
}
