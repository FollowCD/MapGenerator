using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarConfig {

    
    public int TextureWidth;        //贴图宽度 取值越大，性能越低，建议取值 64-256
    public int TextureHeight;       //贴图高度

    public FogOfWarShape FogShape;  //迷雾窗口形状
    public float TexturePixelPerUnit = 100;//最好不要修改
    public Color32 UnexploreColor;  //未探索区域迷雾颜色
    public Color32 ExploredColor;   //已探索区域迷雾颜色

}
