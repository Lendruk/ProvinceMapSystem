using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CountryInfo{
    private static int ids = 0;

    public int countryID;

    public string countryName;

    public string tag;

    public Color countryColour;

   
    //public List<CultureEnum> acceptedCultures;

    public ReligionEnum stateReligion;

    public CountryInfo(string name,string tag,ReligionEnum stateRel,Color colour)
    {
        countryName = name;
        this.tag = tag;
        stateReligion = stateRel;
        countryColour = colour;
        this.countryID = ids++;
       
    }
    
}
