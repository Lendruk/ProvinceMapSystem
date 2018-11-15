using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cultures : MonoBehaviour {
    public List<Culture> cultures = new List<Culture>();
    private static List<Culture> privateCulture;
    


    private void Awake()
    {
        privateCulture = cultures;
    }
    public static Culture GetCulture(CultureEnum cultureEnum)
    {
        
        for(int j =0;j< privateCulture.Count; j++)
        {
            if(privateCulture[j].cultureEnum == cultureEnum)
            {
                return privateCulture[j];
            }
        }
        Debug.LogError("Invalid culture enum");
        return null;

    }
    public static Culture GetCulture(int pos)
    {
        return privateCulture[pos];
    }
}
