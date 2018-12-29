using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "创建测试建筑资源")]
public class FadeDataForConfig :ScriptableObject {

    [Header("宽")]
    public int Width;

    [Header("高")]
    public int Height;

    [Header("指定位置建筑")]
    public List<RigidBuild> RigidPositionBuildingDict;

    [Header("圈层建筑")]
    public List<LayerBuild> layerBuildDict;

    [Header("建筑尺寸")]
    public Vector2 BuildingSize;

    [Header("街道尺寸")]
    public Vector2 StreetSzie;

    [Header("墙体尺寸")]
    public Vector2 WallSize;

    [Header("外墙尺寸")]
    public Vector2 OuterWallSize;

    [Header("绿化带尺寸")]
    public Vector2 GreenBeltSize;

    [Header("长围墙尺寸")]
    public Vector2 LongFenceSize;

    [Header("短围墙尺寸")]
    public Vector2 ShortFenceSize;

    public float StreetGap;                //街道拼接时两个街道的间距
    public float WallGap;                  //墙体间距
    public float OuterWallGap;             //外墙间距
    public float WhiteLineWidth;

    [Range(0,1)]
    public float StreetBloodPercentage;

    [Range(0, 1)]
    public float StreetCarPercentage;
    public  int WideStreetWidth;            //由几个街道组成一个宽街道                                        街0   房0  街1  房1  街2  
    public  Vector2Int WideStreetPos;

    public Dictionary<Vector2Int, string> GetRigData()
    {
        if (RigidPositionBuildingDict!=null)
        {
            var dict = new Dictionary<Vector2Int, string>();

            foreach (var item in RigidPositionBuildingDict)
            {
                dict.Add(item.pos, item.buildResName);
            }

            return dict;
        }

        return null;
    }

    public Dictionary<Vector4Int, Dictionary<string, int>> GetLayerDict()
    {
        if (layerBuildDict!=null)
        {
            var dict = new Dictionary<Vector4Int, Dictionary<string, int>>();
            foreach (var item in layerBuildDict)
            {
                var dict2 = new Dictionary<string, int>();
                foreach (var item2 in item.layerBuildInfo)
                {
                    dict2.Add(item2.buildResName, item2.buildCount);
                }

                dict.Add(item.layerRange, dict2);
            }

            return dict;
        }

        return null;
    }
}

[Serializable]
public struct RigidBuild
{
    public Vector2Int pos;
    public string buildResName;
}

[Serializable]
public struct LayerBuild
{
    public Vector4Int layerRange;
    public List<BuildCount> layerBuildInfo;
}

[Serializable]
public struct BuildCount
{
    public string buildResName;
    public int buildCount;
}