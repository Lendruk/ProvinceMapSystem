using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ActionDelegate();
public class WorldTickManager : MonoBehaviour
{
    public static WorldTickManager Instance { get; set; }
    public event ActionDelegate Actions;
    //public event 


    public void Tick()
    {
        Actions.Invoke();
    }

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
