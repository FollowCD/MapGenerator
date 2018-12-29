using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 负责创建贴图，更新贴图Buffer
/// </summary>
public class FogOfWarCreator {

    public Texture2D Texture { get { return mTexture; } }
    public Vector2Int TexSize { get; private set; }
    Color32[] colorBuffer;
    Texture2D mTexture;
    int pixelAmount;

    public FogOfWarCreator(int width,int height)
    {
        InitTexture(width, height);
    }
    public void UpdateAllTexture(ref Color32 color)
    {
        if (colorBuffer!=null)
        {
            for (int i = 0; i < pixelAmount; i++)
            {
                colorBuffer[i] = color;
            }

            mTexture.SetPixels32(colorBuffer);
            mTexture.Apply();
        }
    }
    public void UpdateTexture(List<int> indexes,ref Color32 color)
    {
        UpdateColorBuffer(indexes, ref color);
        ApplyColorBuffer();
      
    }
    public void UpdateColorBuffer(List<int> indexes, ref Color32 color)
    {

        if (colorBuffer != null)
        {
            if (!indexes.IsNullOrEmpty())
            {
                for (int i = 0; i < indexes.Count; i++)
                {
                    var item = indexes[i];
                    if (item >= 0 && item < pixelAmount)
                    {
                        colorBuffer[item] = color;
                    }
                }
            }
        }
        else
        {
            Debug.LogError("颜色Buffer为空");
        }
     
    }
    public void ApplyColorBuffer()
    {
        if (mTexture != null)
        {
            mTexture.SetPixels32(colorBuffer);
            mTexture.Apply();
        }
        else
        {
            Debug.LogError("贴图为空");
        }
       
    }
    void InitTexture(int xPixelCount, int yPixelCount)
    {
        if (mTexture == null)
        {
            mTexture = new Texture2D(xPixelCount, yPixelCount);
            pixelAmount = xPixelCount * yPixelCount;

            colorBuffer = new Color32[pixelAmount];
            TexSize = new Vector2Int(xPixelCount, yPixelCount);
        }
    }
   
}
