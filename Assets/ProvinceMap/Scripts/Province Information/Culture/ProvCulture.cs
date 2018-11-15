using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProvCulture {
    public int ruralPop;
    public int urbanPop;
    public int aristoPop;

    public CultureEnum culture;
    public Race race;
    public ReligionEnum religion;
    

    public int GetTotalPop()
    {
        return ruralPop + urbanPop + aristoPop;
    }
}
