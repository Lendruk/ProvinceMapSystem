using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class Religion  {
    public string religionName;
    public ReligionEnum religionEnum;
    [TextArea]
    public string religionDescription;
    public Color religionColour;
    public Image religionImage;
}
