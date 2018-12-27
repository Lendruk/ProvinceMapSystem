using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public float xSpeed;

    public float zSpeed;

    public float mapBounds;

    public float zoomSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float x = 0;
        float z = 0;
        float y = 0;

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            z = zoomSpeed;
            y = -zoomSpeed;
        }else if(Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            z = -zoomSpeed;
            y = +zoomSpeed;
        }

        //Handles arrow key movement
        
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            x = -xSpeed;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            x = xSpeed;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            z = zSpeed;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            z = -zSpeed;
        }
        
        if(!(transform.position.z + z < 0 || transform.position.z + z > mapBounds) && !(transform.position.x + x < 0 || transform.position.x + x > mapBounds)
            && !(transform.position.y + y <33))
            transform.position = transform.position + new Vector3(x, y, z);
    }
}
