using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Religions : MonoBehaviour {
    public List<Religion> religions = new List<Religion>();
    private static List<Religion> privateReligions;

    private void Awake()
    {
        privateReligions = religions;
    }
    public static List<Religion> getList()
    {
        return privateReligions;
    }
    public static List<string> getReligionNames()
    {
        List<string> names = new List<string>();
        for(int i = 0;i< privateReligions.Count; i++)
        {
            names.Add(privateReligions[i].religionName);
        }
        return names;
    }
    public static Religion GetReligion(ReligionEnum relenum)
    {
        for (int j = 0; j < privateReligions.Count; j++)
        {
            if (privateReligions[j].religionEnum == relenum)
            {
                return privateReligions[j];
            }
        }
        Debug.LogError("Invalid culture enum");
        return null;
    }
    public static Religion GetReligionByName(string name)
    {
        for (int i = 0; i < privateReligions.Count; i++)
        {
            if(privateReligions[i].religionName == name)
            {
                return privateReligions[i];
            }
        }
        return null;
    }
}
