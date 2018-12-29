using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class BuildingRect
{
    public byte x;
    public byte y;
    public byte w;
    public byte h;

    public BuildingRect()
    {
        x = 0;
        y = 0;
        w = 0;
        h = 0;
    }

    public BuildingRect(byte x,byte y,byte w=1,byte h=1)
    {
        this.x = x;
        this.y = y;
        this.w = w;
        this.h = h;
    }

    public void SetCoord(byte x,byte y)
    {
        this.x = x;
        this.y = y;
    }

    public override bool Equals(object obj)
    {

        if (obj==null)
        {
            return false;
        }
        else if (obj.GetType()!=typeof(BuildingRect))
        {
            return false;
        }
        else
        {
            var temp = obj as BuildingRect;
            if (temp!=null)
            {
                return x == temp.x && y == temp.y && h == temp.h && w == temp.w;
            }
            else
            {
                return false;
            }
        }
    }

    public override int GetHashCode()
    {
         int key = ((int)x << 24 + (int)y << 16 + (int)w << 8 + h);

        return key.GetHashCode();
    }

    public bool IsEquals(BuildingRect temp)
    {
        if (temp == null)
        {
            return false;
        }
        else
        {
            return x == temp.x && y == temp.y && h == temp.h && w == temp.w;
        }
    }

    public override string ToString()
    {
        return string.Format("({0},{1},{2},{3})", x, y, w, h);
    }
}

