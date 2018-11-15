using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum WeatherEnum
{
    CLEAR,RAIN,STORM
}


[CreateAssetMenu(menuName ="Province Weather")]
public class Weather :ScriptableObject
{

    public WeatherEnum weatherType;
    public GameObject weatherEffect;
    public float movementModifier;
    [TextArea]
    public string weatherDescription;

    // Add missing atributes here;
}
