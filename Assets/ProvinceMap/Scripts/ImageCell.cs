using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ImageCell {

	public List<Vector3> borderPoints;
	public List<Vector2> points;
	public Color cell_Color;
    public Vector3 median_point;
	public Vector3 seed_Location;

	[System.NonSerialized]
	public bool searched;

	public ImageCell(List<Vector3> points,Color cell_Color,Vector3 seed_Location){
		this.borderPoints = points;
		this.cell_Color = cell_Color;
		this.seed_Location = seed_Location;
	}
    public ImageCell()
    {
		points = new List<Vector2> ();
        borderPoints = new List<Vector3>();
    }
    public void SetSeedLoc(float x,float y,float z)
    {
        seed_Location = new Vector3(x, y, z);
    }
}
