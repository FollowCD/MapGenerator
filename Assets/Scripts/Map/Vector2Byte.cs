using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector2Byte  {
    public byte x;
    public byte y;

    public Vector2Byte(byte x,byte y)
    {
        this.x = x;
        this.y = y;
    }
    
    public Vector2Byte(int x,int y)
    {
        this.x = (byte)x;
        this.y = (byte)y;
    }

    public Vector2Byte(Vector2Int pos)
    {
        if (pos.x>=0 && pos.x<=255 && pos.y>=0 && pos.y<=255)
        {
            x = (byte)pos.x;
            y = (byte)pos.y;
        }
        else
        {
            Debug.LogError("无法转换，值超出0-255范围：pos=" + pos);
        }
    }

   public bool IsEqual(Vector2Int pos)
    {
        return pos.x == this.x && pos.y == this.y;
    }

   public bool IsEqual(Vector2Byte pos)
    {
        if (pos!=null)
        {
            return pos.x == this.x && pos.y == this.y;
        }
        else { return false; }
    }

    public override bool Equals(object obj)
    {
        if (obj==null)
        {
            return false;
        }
        else if (obj.GetType()!=typeof(Vector2Byte))
        {
            return false;
        }
        else 
        {
            var temp = (Vector2Byte)obj;

            if (temp!=null)
            {
                return temp.IsEqual(this);
            }
            else
            {
                return false;
            }
        }
    }

    public override int GetHashCode()
    {
        int key = (int)x >> 8 + y;

        return key.GetHashCode();
    }

}
