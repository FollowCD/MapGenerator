using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuildingFence{
    //顺序：下-左-上-右
    GameObject[] fences = new GameObject[4];
    public Vector2Byte Coord { get; private set; }
    public void ShowFence(FencePosition dir, bool show)
    {
        if (fences!=null)
        {
            var obj = fences[(byte)dir];
            if (obj!=null)
            {
                if (obj.activeSelf != show)
                {
                    obj.SetActive(show);

                }
            }
            else
            {
                Debug.LogError(string.Format("坐标：（{0}，{1}），第{2}围栏为空", Coord.x,Coord.y, (byte)dir));
            }
        }
        else
        {
            Debug.LogError(string.Format("坐标：（{0}，{1} 建筑围栏数据为空", Coord.x, Coord.y));
        }
    }

    public void ShowAll(bool show)
    {
        if (fences!=null)
        {
            foreach (var item in fences)
            {
                if (item!=null && item.activeSelf!=show)
                {
                    item.SetActive(show);
                }
            }
        }
    }

    public MapBuildingFence(byte x,byte y ,GameObject[] fenceArray)
    {
        Coord = new Vector2Byte(x, y);
        fences = fenceArray;
    }

    public MapBuildingFence(Vector2Int pos, GameObject[] fenceArray)
    {
        Coord = new Vector2Byte(pos);
        fences = fenceArray;
  
    }

    public MapBuildingFence(Vector2Byte pos, GameObject[] fenceArray)
    {
        Coord = pos;
        fences = fenceArray;
    }

    public void Destroy()
    {
        fences = null;
    }
}

public enum FencePosition:byte
{
    Down=0,
    Left=1,
    Up=2,
    Right=3,
}
