using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageRayDetector : MonoBehaviour {
    public static ImageRayDetector instance;

    [HideInInspector]
    public Camera cam;

	// Use this for initialization
	void Start () {
       instance = this;
	}
	
	// Update is called once per frame
	void Update () {
        if (!Input.GetMouseButtonUp(0))
            return;

        RaycastHit hit;
        int layerMask = 1 << 11;
        

        if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, layerMask))
            return;

        Renderer rend = hit.transform.GetComponent<Renderer>();
        MeshCollider meshCollider = hit.collider as MeshCollider;

        if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
            return;

        Texture2D tex = rend.material.mainTexture as Texture2D;
        Vector2 pixelUV = hit.textureCoord;
        pixelUV.x *= tex.width;
        pixelUV.y *= tex.height;
        Color32 clr = tex.GetPixel((int)pixelUV.x,(int) pixelUV.y);


        //if (KeybindManager.singleton.isBusy())
         //   return;

       // UIHelper.instance.OpenProv(ProvinceController.instance.GetProvinceByColour(clr));

        //Debug.Log((int)pixelUV.x);
        //Debug.Log((int)pixelUV.y);

        //tex.SetPixel((int)pixelUV.x, (int)pixelUV.y, Color.black);
        //tex.Apply();
    }
}
