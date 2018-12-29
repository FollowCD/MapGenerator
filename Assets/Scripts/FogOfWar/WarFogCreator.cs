using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[Obsolete("user FogOfWarCreator Instead")]
public class WarFogCreator : MonoBehaviour {



    /*
     * 方案一思路：
     * 1.每一个地图块看作是一个tile
     * 2.每一个Tile，由4个迷雾遮挡
     * 3.采用拼接算法，更新空出来的区域
     * 方案一步骤：
     * 1.根据地图块数量，生成迷雾tile
     * 2.每当建筑变化/探索变化，刷新地图显示
     * 
     * 方案二思路：
     * 1.采用屏幕后期处理，首先生成一张覆盖屏幕的区域图
     * 2.将生成的区域图，进行模糊处理
     * 3.将该模糊图，与正常生成的图，进行混合
     * 4.赋给摄像机显示
     */




    #region 版本2 战争迷雾实现


    #region 预设参数 & 组件
    [Header("贴图高度")]
    public int TexHeight=128;
    [Header("贴图宽度")]
    public int TexWidth=128;
    [Header("战争迷雾展示区")]
    public Image textureCotainter;
    [Header("未探索区域颜色")]
    public Color32 UnexploreColor= new Color32(0,0,0,255);
    [Header("已探索区域颜色")]
    public Color32 ExploredColor= new Color32(255,255,255,0);
    [Header("边缘过渡像素宽度")]
    public int TransitionPixelCount = 1;
    [Header("边缘过渡层次数量")]
    public int TransitionLayerCount = 1;
    [Header("图像缩放比例")]
    public Vector2 SpriteScale;//图像缩放比例
    [Header("贴图模糊处理材质")]
    public Material GaussianBlurMaterial;
    #endregion

    #region 运算时数据
    int texturePixelCount = 0;
    Vector2 textureSize;//贴图尺寸
    Vector2 pixelSize;//像素尺寸
    #endregion

    #region 存储数据容器
    protected Color32[] mBuffer0;
    protected Texture2D mTexture;
    protected RenderTexture mRenderTexture;
    protected Sprite mSprite;
    #endregion

    #region 运行标志位
    bool NeedRecreateSp = false;
    #endregion

    #region 功能代码

    void CreateTexture(int width,int height)
    {
        if (mTexture==null)
        {
            mTexture = new Texture2D(width, height);
        }
    }

    void AppleColorBuffer()
    {
        if (mBuffer0==null)
        {
            mBuffer0 = new Color32[TexWidth * TexHeight];
        }

        if (mTexture == null)
        {
            CreateTexture(TexWidth, TexHeight);
        }

        mTexture.SetPixels32(mBuffer0);
        mTexture.Apply();
        NeedRecreateSp = true;
    }

    Sprite CreateSprite(Texture2D tex)
    {
        if (mSprite==null||NeedRecreateSp)
        {
            AppleColorBuffer();
            mSprite = Sprite.Create(tex, new Rect(Vector2.zero, textureSize), Vector2.zero,100);
            NeedRecreateSp = false;
        }

        return mSprite;
    }


    void ShowSprite()
    {
        if (textureCotainter!=null)
        {
            textureCotainter.sprite = CreateSprite(mTexture);
            textureCotainter.SetNativeSize();
            textureCotainter.rectTransform.localScale = SpriteScale;
        }
    }

    /// <summary>
    /// 获取一个菱形范围内的像素点的index
    /// </summary>
    /// <param name="RhombusPosInTexture"> 菱形在贴图坐标的位置 </param>
    /// <param name="RhombusSize"> 菱形的宽和高 </param>
    /// <param name="pixelSize"> 一个像素点的尺寸 </param>
    /// <returns></returns>
    List<int> GetRhombusRangePixelIndex(Vector2 RhombusPosInTexture,Vector2 RhombusSize,Vector2 pixelSize)
    {
        return null;
    }

    /// <summary>
    /// 获取一个矩形范围内的像素点的index
    /// </summary>
    /// <param name="RectPosInTexture"></param>
    /// <param name="rectSize"></param>
    /// <param name="pixelSize"></param>
    /// <returns></returns>
    List<int> GetRectRangePixelIndex(Vector2 RectPosInTexture,Vector2 rectSize,Vector2 pixelSize)
    {
        var x_left = Mathf.FloorToInt((RectPosInTexture.x -rectSize.x/2)/ pixelSize.x);//矩形左
        var x_right = Mathf.FloorToInt((RectPosInTexture.x + rectSize.x / 2) / pixelSize.x);//矩形右
        var y_up = Mathf.FloorToInt((RectPosInTexture.y + rectSize.y / 2) / pixelSize.y);//矩形上
        var y_down = Mathf.FloorToInt((RectPosInTexture.y - rectSize.y / 2) / pixelSize.y);//矩形下

        x_left = x_left >= 0 ? x_left : 0;
        x_right = x_right < TexWidth ? x_right : TexWidth - 1;

        y_down = y_down >= 0 ? y_down : 0;
        y_up = y_up < TexHeight ? y_up : TexHeight - 1;

        List<int> result = new List<int>();
        for (int i = x_left; i <= x_right; i++)
        {
            for (int j = y_down; j <=y_up ; j++)
            {
                result.Add(ReverseCoordToIndex(i, j));
            }
        }


        return result; 
    }



    bool CheckDisBigThanRadious(Vector2Int pixelPos,Vector2 centerPos,float radious,Vector2 pixelSize)
    {
        var result = pixelPos * pixelSize - centerPos;

        var length = result.magnitude;

        return length > radious;
    }

    Vector2Int GetPixelCoordByIndex(int index)
    {
        //坐标转index公式  result = y * TexWidth + x;
        var y = Mathf.FloorToInt(index / TexWidth);
        var x = index - y;

        return new Vector2Int(x, y);
    }
    /// <summary>
    /// 显示带渐变的空白区域（未完成）
    /// </summary>
    /// <param name="RectPosInTexture"></param>
    /// <param name="rectSize"></param>
    /// <param name="pixelSize"></param>
    void ShowJianBianArea(Vector2 RectPosInTexture, Vector2 rectSize, Vector2 pixelSize)
    {
        
        for (int i = TransitionLayerCount-1; i >-1; i--)//过渡层次,从外层到内层
        {
            var addWidth = TransitionPixelCount * i * pixelSize.x;
            var addHeight = TransitionPixelCount * i * pixelSize.y;
            var realRectSize = new Vector2(rectSize.x + addWidth, rectSize.y + addHeight);

            var outerList = GetRectRangePixelIndex(RectPosInTexture, realRectSize, pixelSize);

            var realColorApha = (byte)(255.0 / TransitionLayerCount * i);
            var color = new Color32(0,0,0, realColorApha);

            UpdateColorBuffer(outerList, color);
            Debug.Log(string.Format("赋值完第：{0}层，赋值颜色：{1}", i, color.ToString()));
        }

        AppleColorBuffer();
        ShowSprite();


    }
    /// <summary>
    /// 获取世界物体在纹理空间中的坐标
    /// 需要确保纹理 与 物体的父物体为同一物体
    /// </summary>
    /// <param name="objLocalPos"> 世界物体的本地坐标 </param>
    /// <param name="textureLocalPos"> 贴图物体的本地坐标 </param>
    /// <param name="textureSize"> 贴图尺寸 </param>
    /// <param name="textureScale"> 贴图缩放比例 </param>
    /// <returns></returns>
    Vector2 ReverseWorldPosToUVPos(Vector2 objLocalPos,Vector2 textureLocalPos,Vector2 textureSize,Vector2 textureScale)
    {
        var texLeftDownPos = textureLocalPos - textureSize * textureScale / 2+textureScale/2;//获得贴图左下角的世界坐标
        var worldToTexVecotr = -texLeftDownPos;
        return worldToTexVecotr + objLocalPos;
    }

    /// <summary>
    /// 创建贴图，并且初始化各个值
    /// </summary>
    void InitTextureAndParam()
    {
        CreateTexture(TexWidth, TexHeight);
        texturePixelCount = TexWidth * TexHeight;
        mBuffer0 = new Color32[texturePixelCount];
        textureSize = new Vector2(TexWidth, TexHeight);
        //SpriteScale = new Vector2(1,1);
        pixelSize = SpriteScale;
        if (textureCotainter!=null)
        {
            textureCotainter.transform.localScale = SpriteScale;
        }
    }

    int ReverseCoordToIndex(int x,int y)
    {

        int result = 0;
        if (x>=0 && x<TexWidth && y<TexHeight&& y>=0)
        {
            result = y * TexWidth + x;
        }
        else
        {
            Debug.LogError("无法转换坐标为数组index");
        }

        return result;

    }

    void UpdateColorBuffer(List<int> changedIndexList,Color32 showColor)
    {
        if (mBuffer0==null)
        {
            mBuffer0 = new Color32[texturePixelCount];
        }

        if (changedIndexList==null||changedIndexList.Count<1)
        {
            return;
        }

         
        //1.刷新颜色缓冲区
        foreach (var item in changedIndexList)
        {
            if (item>=0&& item<texturePixelCount&& mBuffer0[item].a>showColor.a)
            {
                
                mBuffer0[item] = showColor;
            }
        }

        NeedRecreateSp = true;

        
    }

    #endregion

    #endregion



    #region Mono生命周期函数


    private void Awake()
    {
       
    }

    private void Update()
    {

    }

    #endregion
}
