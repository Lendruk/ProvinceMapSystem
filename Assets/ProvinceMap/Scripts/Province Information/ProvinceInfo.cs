using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class ProvinceInfo
{
    #region provinceInfo
    [ReadOnly]
    public int provinceID;
    public Color32 provinceColour;
   
    [System.NonSerialized] 
    public Vector3 center;

    // Name and Description;
    public string provinceName;
    [TextArea]
    public string provinceDescription = "Default";
    public bool isTransversable = true;

    //TODO redo weather System
    //public Weather weather;
    #endregion

    #region population
    public enum PopulationType
    {
        RURAL,URBAN,ARISTOCRATIC
    }

    [Header("Population")]

    // Main population
    [ReadOnly]
    public int ruralPopulation;

    [ReadOnly]
    public int urbanPopulation;

    [ReadOnly]
    public int aristocraticPopulation;

    [ReadOnly]
    public int totalPopulation;
    // Secondary
    //public int slavePopulation;
    //public int provinceGarrisonSize;

    //public List<ProvinceDemographic> cul;

    public List<ProvCulture> provCultures;

    [System.NonSerialized]
    public int ownerID;
    //public CulturePopulationDictionary cultureList;

    //Religion
    //public ReligionPercentageDictionary religions;

    // Incremental Factors;
    //public float provinceRuralAtraction; // indicates rural population  immigration
    //public float provinceUrbanAtraction; // indicates urban population immigration

    //public float naturalGrowth; // Population growth

    public ProvinceInfo(int rural,int urban,int aristo,string provinceName,string description,bool isTransversable)
    {
        ruralPopulation = rural;
        urbanPopulation = urban;
        aristocraticPopulation = aristo;
        this.provinceName = provinceName;
        this.provinceDescription = description;
        this.isTransversable = isTransversable;
        UpdatePopulation();
    }
    public void UpdatePopulation()
    {

        totalPopulation = 0;
        ruralPopulation = 0;
        urbanPopulation = 0;
        aristocraticPopulation = 0;
        foreach(ProvCulture pv in provCultures)
        {
            ruralPopulation += pv.ruralPop;
            urbanPopulation += pv.urbanPop;
            aristocraticPopulation += pv.aristoPop;
        }
        totalPopulation = ruralPopulation + urbanPopulation + aristocraticPopulation;
    }
    public int GetCulturePopulation(CultureEnum culture)
    {
        int totalPop = 0;
        foreach(ProvCulture p in provCultures)
        {
            if (p.culture == culture)
            {
                totalPop += p.GetTotalPop();
                break;
            }
        }
        return totalPop;
    }
    
    public float getTotalReligionPercentage()
    {
        /*
        float sum = 0.0f;
        foreach(ReligionPercentage percent in religions.Values)
        {
            sum += percent.percentage;
        }
        return sum;
        */
        return 0;
    }

    public ProvinceInfo()
    {
        ruralPopulation = 0;
        urbanPopulation = 0;
        totalPopulation = 0;
        aristocraticPopulation = 0;
    }
    public string GetPopulationString()
    {
        return "Rural: " + ruralPopulation + "\n" + "Urban: " + urbanPopulation + "\n" + "Aristocratic: " + aristocraticPopulation;
    }
    public int GetTotalPopulation()
    {
        
        return ruralPopulation + urbanPopulation + aristocraticPopulation;
    }
    public Religion GetMajorityReligion()
    {
        ReligionEnum relEnum = ReligionEnum.None;
        int pop = 0;

		foreach(ProvCulture p in provCultures)
        {
            int temp = p.GetTotalPop();
            if(temp > pop)
            {
                relEnum = p.religion;
                pop = temp;
            }
        }

        return Religions.GetReligion(relEnum);
		/*
        float tempPercent = 0.0f;
        foreach(ReligionPercentage percent in religions.Values)
        {
            if(percent.percentage > tempPercent)
            {
                tempPercent = percent.percentage;
            }
        }
        ReligionPercentage val;
        foreach(ReligionEnum relenum in religions.Keys)
        {
            religions.TryGetValue(relenum, out val);
            if (val.percentage == tempPercent)
            {
                return Religions.GetReligion(relenum);
            }
        }
        return null;
		*/
    }
    #endregion

    #region terrain
    public enum TerrainType
    {
        Plains,Forest,Coast,Ocean,Lake,River,Desert,Grasslands
    }
    public TerrainType terrain;
    #endregion

}
