using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapConfig
{
    /* 1.1地图生成所需：
    *  prefab：
    *      建筑prefab
    *      地块prefab
    *      街道prefab
    *      围墙prefab
    *      外围prefab
    *  数据：
    *      地图大小(n*m)
    *      地块间隙(x*y)
    *      街道间隙(x*y)
    *      围墙间隙(x*y)
    *      外围间隙(x*y)
    *      -------------
    *      详细配置建筑信息(建筑类型-建筑位置)
    *      建筑数量(建筑类型-建筑数量)
    *      建筑分布范围(范围-建筑数量)
    */

    /*  层级
    *                     —————— -> 汽车
    *                     —————— -> 绿化带
    *                     —————— -> 血迹
    *建筑血迹  <- ————     —————— -> 白线
    *  建筑    <- ————     —————— -> 街道
    *            ————————————————  -> 地面
    */


    #region 建筑配置
    public readonly Dictionary<Vector2Int, string> RigidPositionBuildingDict;               //key-具体位置， value-建筑预制体名称
    public readonly Dictionary<Vector4Int, Dictionary<string, int>> LayerBuidingDict;       //key-圈层范围(从中间开始往两边走)， value-该圈层各个建筑数量(key-建筑名称，value-该建筑数量)
    #endregion

    public readonly int MapWidth;
    public readonly int MapHeight;

    public readonly Vector2 BuildingSize;           //建筑地图所占尺寸
    public readonly Vector2 StreetSize;             //街道地图所占尺寸
    public readonly Vector2 WallSize;               //墙体地图所占尺寸
    public readonly Vector2 OuterWallSize;          //外墙地图所占尺寸
    public readonly Vector2 GreenBeltSize;          //绿化带尺寸
    public readonly Vector2 LongFenceSize;
    public readonly Vector2 ShortFenceSize;

    public readonly float WhiteLineWidth;          //白线的宽度

    public readonly float StreetGap;                //街道拼接时两个街道的间距
    public readonly float WallGap;                  //墙体间距
    public readonly float OuterWallGap;             //外墙间距
    public readonly float StreetBloodPercentage;
    public readonly float StreetCarPercentage;
    public readonly float StreetBrokenPercentage;

    public readonly int WideStreetWidth;            //由几个街道组成一个宽街道                                        街0   房0  街1  房1  街2  
    public readonly Vector2Int WideStreetPos;       //宽街道的位置，x代表 第 x列，y代表第y行，目前只支持1横1竖两条款街道  |    口    |   口   |   



    public MapConfig(FadeDataForConfig data)
    {
        if (data!=null)
        {
            RigidPositionBuildingDict = data.GetRigData();
            LayerBuidingDict = data.GetLayerDict();

            MapWidth = data.Width;
            MapHeight = data.Height;

            BuildingSize = data.BuildingSize;
            StreetSize = data.StreetSzie;
            WallSize = data.WallSize;
            OuterWallSize = data.OuterWallSize;
            GreenBeltSize = data.GreenBeltSize;
            WhiteLineWidth = data.WhiteLineWidth;
            LongFenceSize = data.LongFenceSize;
            ShortFenceSize = data.ShortFenceSize;

            StreetGap = data.StreetGap;
            WallGap = data.WallGap;
            OuterWallGap = data.OuterWallGap;
            WideStreetWidth = data.WideStreetWidth;
            WideStreetPos = data.WideStreetPos;

            StreetBloodPercentage = data.StreetBloodPercentage;
            StreetCarPercentage = data.StreetCarPercentage;
        }

    }


    public Vector2 GetFogOfWarSize()
    {
        var x = BuildingSize.x * MapWidth + (MapWidth + WideStreetWidth - 1) * StreetSize.x + 4 * OuterWallSize.x;
        var y = BuildingSize.y * MapHeight + (MapHeight + WideStreetWidth - 1) * StreetSize.y + 4 * OuterWallSize.y;
        return new Vector2(x, y);
    }

    public bool PosAtVerticalWideStreetLeft(int x)
    {
        return WideStreetPos.x - 1 == x && x >= 0;
    }

    public bool PosAtVerticalWideStreetRight(int x)
    {
        return WideStreetPos.x == x && x < MapWidth;
    }

    public bool PosAtVerticalWideStreetSide(int x)
    {
        return PosAtVerticalWideStreetLeft(x) || PosAtVerticalWideStreetRight(x);
    }

    public bool PosAtHorizontalWideStreetDown(int y)
    {
        return WideStreetPos.y - 1 == y && y >= 0;
    }

    public bool PosAtHorizontalWideStreetUp(int y)
    {
        return WideStreetPos.y == y && y < MapHeight;
    }

    public bool PosAtHorizontalWideStreetSide(int y)
    {
        return PosAtHorizontalWideStreetUp(y) || PosAtHorizontalWideStreetDown(y);
    }

    public bool PosAtWideStreetSide(Vector2Int pos)
    {
        return PosAtVerticalWideStreetSide(pos.x) || PosAtHorizontalWideStreetSide(pos.y);
    }


}
[System.Serializable]
public struct Vector4Int
{
    public int Xmin;
    public int Xmax;
    public int Ymin;
    public int Ymax;

    public Vector4Int(int xmin, int xmax, int ymin, int ymax)
    {
        if (xmax < xmin || ymax < ymin)
        {
            Debug.LogError("参数错误，最大值必须大于等于最小值");
            xmin = 0;
            xmax = 0;
            ymin = 0;
            ymax = 0;

        }

        Xmin = xmin;
        Xmax = xmax;
        Ymin = ymin;
        Ymax = ymax;


    }

    public int Count
    {
        get { return (Xmax - Xmin + 1) * (Ymax - Ymin + 1); }
    }

    public bool IsCover(Vector4Int smallOne)
    {
        return Xmin < smallOne.Xmin && Xmax > smallOne.Xmax && Ymin < smallOne.Ymin && Ymax > smallOne.Ymax;
    }

    public static Vector4Int Zero
    {
        get { return new Vector4Int(0, 0, 0, 0); }
    }

    public override string ToString()
    {
        return string.Format("x=({0},{1}),y=({2},{3})", Xmin, Xmax, Ymin, Ymax);
    }

    public override bool Equals(object obj)
    {
        if (obj.GetType() != typeof(Vector4Int))
        {
            return false;
        }

        var data = (Vector4Int)obj;

        return data.Xmin == Xmin && data.Xmax == Xmax && data.Ymin == Ymin && data.Ymax == Ymax;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }


    public static bool operator==(Vector4Int one, Vector4Int other){
        return one.Equals(other);
    }

    public static bool operator!=(Vector4Int one,Vector4Int other)
    {
        return !one.Equals(other);
    }
}