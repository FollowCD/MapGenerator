using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FogOfWarManager : MonoBehaviour {

    #region 预设参数
    int TexW;
    int TexH;
    FogOfWarShape FogShape;
    float TexturePixelPerUnit;//图像缩放比例
    Image DisPlayImg;
    Color32 UnexploreColor;
    Color32 ExploredColor;
    Vector2 SpriteScale;//图像缩放比例
   
    #endregion

    #region 组件
    FogOfWarViewer viewer;
    FogOfWarCreator creator;
    #endregion

    #region 运算参数
    Vector2 WorldToSpriteSpaceVector;
    #endregion

    #region 标志位
    bool hasSetWorldToSpriteVector = false;
    #endregion

    private void Awake()
    {

    }

    public void Init(FogOfWarConfig config, Vector2 size)
    {
        if (config==null)
        {
            Debug.LogError("迷雾配置数据为空！战争迷雾初始化失败");
            return;
        }

        SetConfigData(config);
        creator = new FogOfWarCreator(TexW,TexH);
        InitFogOfWarViewer(size);
        DisPlayImg = CreateDisPlayImg();
        creator.UpdateAllTexture(ref UnexploreColor);
        viewer.ShowFogOfWar(creator.Texture, creator.TexSize,SpriteScale);
    }

    void SetConfigData(FogOfWarConfig config)
    {
        TexW = config.TextureWidth;
        TexH = config.TextureHeight;
        FogShape = config.FogShape;
        TexturePixelPerUnit = config.TexturePixelPerUnit;
        UnexploreColor = config.UnexploreColor;
        ExploredColor = config.ExploredColor;
    }

    void InitFogOfWarViewer(Vector2 size)
    {
        SpriteScale.x = size.x / TexW;
        SpriteScale.y = size.y / TexH;
        SpriteScale *= 1.1f;

        viewer = DisPlayImg.gameObject.AddComponent<FogOfWarViewer>();
        viewer.Init(DisPlayImg, SpriteScale);
    }

    Image CreateDisPlayImg()
    {
        var go = new GameObject("fogOfWarViewer");

        var img= go.AddComponent<Image>();

        go.AddComponent<RectTransform>();

        go.transform.SetParent(transform);

        go.transform.localScale = Vector3.one;

        go.transform.localPosition = Vector3.zero;

        go.transform.eulerAngles = Vector3.zero;

        return img;
    }

    //key：位置信息，value：像素的索引列表
    Dictionary<Vector4, List<int>> showAreaDict = new Dictionary<Vector4, List<int>>();

    public void InitFogOfWar(List<Vector3> initActiveArea)
    {

    }

    public void ShowArea(List<Vector4> activeArea,List<Vector4> disActiveArea)
    {
        if (!activeArea.IsNullOrEmpty())
        {
            ShowArea(activeArea, ExploredColor);
        }

        if (!disActiveArea.IsNullOrEmpty())
        {
            ShowArea(disActiveArea, UnexploreColor);
        }
    }

    void ShowArea(List<Vector4> targetPos,Color32 targetColor)
    {
       
        for (int i = 0; i < targetPos.Count; i++)
        {
            List<int> list;
            var item = targetPos[i];
            var pos = new Vector3(item.x, item.y, 0);

            if (!showAreaDict.ContainsKey(item))
            {
               
                switch (FogShape)
                {
                    case FogOfWarShape.Rect:
                        list = GetBufferIndexInRect(pos, new Vector2(item.z, item.w));
                        break;
                    case FogOfWarShape.Circle:
                        var radious = Mathf.Max(item.z, item.w)/2;
                        list = GetBufferIndexInCircle(pos, radious);
                        break;
                    default:
                        list = new List<int>();
                        break;
                }

                showAreaDict.Add(item, list);

            }
            else
            {
                list = showAreaDict.GetValue(item);
            }

            //更新Buffer
            creator.UpdateColorBuffer(list, ref targetColor);
        }

        creator.ApplyColorBuffer();
        viewer.ShowFogOfWar(creator.Texture, creator.TexSize, SpriteScale);
    }
    
    List<int> GetBufferIndexInRect(Vector3 rectWorldPos,Vector2 rectSize)
    {
        var uvPos = ReverseWorldPosToUVPos(rectWorldPos);

        var xMinIndex = Mathf.FloorToInt( (uvPos.x - rectSize.x / 2) / SpriteScale.x);
        var xMaxIndex = Mathf.FloorToInt((uvPos.x + rectSize.x / 2) / SpriteScale.x);
        var yMinIndex = Mathf.FloorToInt((uvPos.y - rectSize.y / 2) / SpriteScale.y);
        var yMaxIndex = Mathf.FloorToInt((uvPos.y + rectSize.y / 2) / SpriteScale.y);

        var xMax = creator.TexSize.x - 1;
        var yMax = creator.TexSize.y - 1;

        xMinIndex = Mathf.Clamp(xMinIndex, 0, xMax);
        xMaxIndex = Mathf.Clamp(xMaxIndex, 0, xMax);
        yMinIndex = Mathf.Clamp(yMinIndex, 0, yMax);
        yMaxIndex = Mathf.Clamp(yMaxIndex, 0, yMax);

        var list = new List<int>();

        for (int i = xMinIndex; i <= xMaxIndex; i++)
        {
            for (int j = yMinIndex; j <= yMaxIndex; j++)
            {
                list.Add(j*(xMax+1)+i);
            }
        }

        return list;
    }
    List<int> GetBufferIndexInCircle(Vector3 centerWorldPos,float radius)
    {
        var uvPos = ReverseWorldPosToUVPos(centerWorldPos);

        var xMinIndex = Mathf.FloorToInt((uvPos.x - radius) / SpriteScale.x);
        var xMaxIndex = Mathf.FloorToInt((uvPos.x + radius) / SpriteScale.x);
        var yMinIndex = Mathf.FloorToInt((uvPos.y - radius) / SpriteScale.y);
        var yMaxIndex = Mathf.FloorToInt((uvPos.y + radius) / SpriteScale.y);

        var xMax = creator.TexSize.x - 1;
        var yMax = creator.TexSize.y - 1;

        xMinIndex = Mathf.Clamp(xMinIndex, 0, xMax);
        xMaxIndex = Mathf.Clamp(xMaxIndex, 0, xMax);
        yMinIndex = Mathf.Clamp(yMinIndex, 0, yMax);
        yMaxIndex = Mathf.Clamp(yMaxIndex, 0, yMax);

        var x = (int)(uvPos.x / SpriteScale.x);
        var y = (int)(uvPos.y/SpriteScale.y);

        var radiusSq = radius * radius;

        var list = new List<int>();
        for (int i = xMinIndex; i <= xMaxIndex; i++)
        {
            for (int j = yMinIndex; j <=yMaxIndex; j++)
            {
                var lenRow = Mathf.Abs((i - x)*SpriteScale.x)-SpriteScale.x/2;
                var lenCol = Mathf.Abs((j - y)*SpriteScale.y)-SpriteScale.y/2 ;

                var longLen = lenRow * lenRow + lenCol * lenCol;

                if (radiusSq > longLen)
                {
                    list.Add(j * (xMax + 1) + i);
                }
            }
        }

        return list;
    }
    Vector2 ReverseWorldPosToUVPos(Vector2 objLocalPos)
    {
        if (!hasSetWorldToSpriteVector)
        {
            Vector2 spritePos = DisPlayImg.transform.localPosition;
            var texLeftDownPos = spritePos - creator.TexSize * SpriteScale / 2 + SpriteScale / 2;//获得贴图左下角的世界坐标
            WorldToSpriteSpaceVector = -texLeftDownPos;
            hasSetWorldToSpriteVector = true;
        }
      
        return WorldToSpriteSpaceVector + objLocalPos;
    }

}
