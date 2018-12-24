using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class ImageSplitter{

    [HideInInspector]
    public Texture2D bmpImage;

    public List<ImageCell> cells = new List<ImageCell>();

    
    public void SplitImage(){
        for (int i = 0; i < bmpImage.width; i++)
        {
            for (int j = 0; j < bmpImage.height; j++)
            {
                Color pixel = bmpImage.GetPixel(i,j);
                if(pixel == Color.white){
                    continue;
                }
				GetCell (pixel).points.Add (new Vector2 (i, j));
                
            }
        }
        Debug.Log(cells.Count);
    }
    public void SplitByBorder()
    {
        cells.Clear();
        ImageCell cell = new ImageCell();
        Color curPixel = bmpImage.GetPixel(0, 0);
        cell.cell_Color = curPixel;

        for (int i = 0; i < bmpImage.width; i++)
        {
            for (int j = 0; j < bmpImage.height; j++)
            {
                Color pixel = bmpImage.GetPixel(i, j);
                if (pixel == Color.black)
                {
                    Color prevPixel = bmpImage.GetPixel(i - 1, j);
                    if (HasColour(prevPixel))
                    {
                        ImageCell prevCell = GetCell(bmpImage.GetPixel(i - 1, j));
                        if (cell.seed_Location == Vector3.zero || prevCell.seed_Location == Vector3.zero)
                        {
                            prevCell.SetSeedLoc(i, j, 0);   
                        }
                        if (IsBorderCell(prevCell.cell_Color, i, j))
                        {
                            prevCell.borderPoints.Add(new Vector3(i, j, 0));
                        }
                    }
                    else if (cell.seed_Location == Vector3.zero || cell.seed_Location == Vector3.zero)
                    {
                        cell.seed_Location = new Vector3(i, j, 0); // Alterar o y pelo z pois quero o mapa horizontal
                        if (IsBorderCell(prevPixel, i, j))
                        {
                            cell.borderPoints.Add(new Vector3(i, j, 0));
                        }
                    }
                    continue;
                }
                if (IsBorderCell(pixel, i, j))
                {
                    if (pixel != curPixel && pixel != Color.black)
                    {

                        if (HasColour(pixel))
                        {
                            GetCell(pixel).borderPoints.Add(new Vector3(i, j, 0));
                        }
                        else
                        {
                            curPixel = pixel;
                            cells.Add(cell);
                            cell = new ImageCell
                            {
                                borderPoints = new List<Vector3>(),
                                cell_Color = curPixel
                            };
                            cell.borderPoints.Add(new Vector3(i, j, 0));
                        }
                    }
                    else
                    {
                        cell.borderPoints.Add(new Vector3(i, j, 0));
                    }
                }
            }
        }
        cells.Add(cell);
        Debug.Log(cells.Count);
    }
    public void SplitByBorderNoSeed(){
        cells.Clear();
        ImageCell cell;
        for(int y = 0 ; y < bmpImage.height; y++){
            for(int x = 0; x < bmpImage.width; x++){
                Color curPixel = bmpImage.GetPixel(x,y);
                if (curPixel == Color.white)
                    continue;
               
                if(IsBorderCell(curPixel,x,y)){
                    //Debug.Log(x +" | "+ y);
                    if(HasColour(curPixel)){
                        GetCell(curPixel).borderPoints.Add(new Vector3(x,y,0));
                    }else{
                        cell = new ImageCell
                            {
                                borderPoints = new List<Vector3>(),
                                cell_Color = curPixel
                            };
                        cell.borderPoints.Add(new Vector3(x, y, 0));
                        cells.Add(cell);
                    }
                }
            }
        }
    }
    public void SplitByBorderOrdered()
    {
        string[] data = File.ReadAllLines("Assets/ProvinceMap/Resources/provBasic.txt");
        
        foreach(string str in data)
        {
            string[] rgb = str.Split('#');
            ImageCell cell = new ImageCell();
            cell.cell_Color = new Color32((byte)int.Parse(rgb[1]), (byte)int.Parse(rgb[2]), (byte)int.Parse(rgb[3]), 255);
            cells.Add(cell);
        }
		for (int i = 0; i < bmpImage.width; i++) {
			for (int j = 0; j < bmpImage.height; j++) {
				Color curPixel = bmpImage.GetPixel (i, j);
				if (curPixel == Color.white)
					continue;
				if (HasColour (curPixel)) {
					ImageCell cell = GetCell (curPixel);
					if (cell.searched == false) {
						List<Vector3> points = new List<Vector3> ();
						points.Add(new Vector3(i,j));
						SearchNearby (i, j, curPixel, points);
						points.RemoveAt(points.Count-1);
						cell.borderPoints = points;
						cell.searched = true;
					}

				}
			}
		}
		//SplitImage ();


        
    }
    private List<Vector3> SearchNearby(int x,int y,Color clr,List<Vector3> points)
    {
        
        int width = bmpImage.width;
        int height = bmpImage.height;
        

        //Top
        if(y+1 <= height && bmpImage.GetPixel(x, y+1) == clr)
		{
            if (IsBorderCell(clr, x, y+1) && !points.Contains(new Vector3(x, y+1)))
            {
                points.Add(new Vector3(x, y+1));
                SearchNearby(x, y+1, clr,points);
            }
        }
        //Right
        if (x+1 <= width && bmpImage.GetPixel(x + 1, y) == clr)
        {
            if (IsBorderCell(clr, x + 1, y) && !points.Contains(new Vector3(x + 1, y)))
            {
                points.Add(new Vector3(x + 1, y));
                SearchNearby(x + 1, y, clr, points);
            }
        }
        //Down
        if( y-1 >= 0 && bmpImage.GetPixel(x,y-1) == clr)
        {
			if (IsBorderCell(clr, x, y - 1)&& !points.Contains(new Vector3(x , y-1)))
            {
                points.Add(new Vector3(x, y-1));
                SearchNearby(x, y-1, clr, points);
            }
        }
        //Left
        if (x-1 >= 0 && bmpImage.GetPixel(x-1, y) == clr)
        {
			if (IsBorderCell(clr, x-1, y)&& !points.Contains(new Vector3(x -1, y)))
            {
                points.Add(new Vector3(x-1, y));
                SearchNearby(x-1, y, clr, points);
            }
        }
        //Bottom Left
        if (y - 1 >= 0 && x-1 >= 0 &&  bmpImage.GetPixel(x - 1, y - 1) == clr)
        {
			if (IsBorderCell(clr, x-1, y - 1)&& !points.Contains(new Vector3(x - 1, y-1)))
            {
                points.Add(new Vector3(x-1, y - 1));
                SearchNearby(x-1, y - 1, clr, points);
            }
        }
        //Bottom Right
        if (y - 1 >= 0 && x + 1 <= width && bmpImage.GetPixel(x + 1, y - 1) == clr)
        {
			if (IsBorderCell(clr, x + 1, y - 1)&& !points.Contains(new Vector3(x + 1, y-1)))
            {
				points.Add(new Vector3(x + 1, y - 1));
                points = SearchNearby(x + 1, y - 1, clr, points);
            }
        }
        //Top Right
        if (y + 1 <= height && x + 1 <= width && bmpImage.GetPixel(x + 1, y + 1) == clr)
        {
			if (IsBorderCell(clr, x + 1, y + 1)&& !points.Contains(new Vector3(x + 1, y+1)))
            {
                points.Add(new Vector3(x + 1, y + 1));
                SearchNearby(x + 1, y + 1, clr, points);
            }
        }

        //Top Left
        if (y + 1 <= height && x - 1 >= 0 && bmpImage.GetPixel(x - 1, y + 1) == clr)
        {
			if (IsBorderCell(clr, x - 1, y + 1)&& !points.Contains(new Vector3(x - 1, y+1)))
            {
                points.Add(new Vector3(x - 1, y + 1));
                SearchNearby(x - 1, y + 1, clr, points);
            }
        }
		return points;
    }
    public void SplitOrdered()
    {
        int height = bmpImage.height;
        int width = bmpImage.width;
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Color pixel = bmpImage.GetPixel(j, i);

                if (pixel == Color.white)
                    continue;

                if (HasColour(pixel))
                {
                    ImageCell cell = GetCell(pixel);
                    
                    for (int l = i; l < height; l++)
                    {
                        for (int k = j; k < width; k++)
                        {
                            Color temp = bmpImage.GetPixel(k, l);
                            if(pixel == temp)
                                cell.points.Add(new Vector3(k, l));
                        }
                    }

                }
                else
                {
                    Debug.Log("oopsie i just made a fucko boingo!");
                }
            }
        }
    }

    
    public void AddMedianPoint()
    {
        foreach(ImageCell cell in cells)
        {
            float xPoints = 0;
            float yPoints = 0;
            float zPoints = 0;
            for (int i = 0; i < cell.borderPoints.Count; i++)
            {
                Vector3 cur = cell.borderPoints[i];
                xPoints += cur.x;
                yPoints += cur.y;
                zPoints += cur.z;
            }
            cell.median_point = new Vector3(xPoints / cell.borderPoints.Count, yPoints / cell.borderPoints.Count, zPoints / cell.borderPoints.Count);
        }
    }

    
    public bool IsBorderCell(Color clr,int x,int y)
    {
        //Check for image borders
        if (x - 1 < 0) return true;
        if (x + 1 >= bmpImage.width) return true;
        if (y - 1 < 0) return true;
        if (y + 1 >= bmpImage.height) return true;

        //Check for color borders
        // Top
        if (bmpImage.GetPixel(x, y + 1) != clr)
            return true;
        // Right
        if (bmpImage.GetPixel(x + 1, y) != clr)
            return true;
        // Bottom
        if (bmpImage.GetPixel(x, y - 1) != clr)
            return true;
        // Left
        if (bmpImage.GetPixel(x - 1, y) != clr)
            return true;

        return false;
    }
    public bool HasColour(Color clr)
    {
        foreach(ImageCell cell in cells)
        {
            if (cell.cell_Color == clr)
                return true;
        }
        return false;
    }
    public ImageCell GetCell(Color clr)
    {
        foreach(ImageCell cell in cells)
        {
            if (cell.cell_Color == clr)
                return cell;
        }
        return new ImageCell();
    }
}
