using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FogOfWarViewer : MonoBehaviour
{
    Image imgComponent;
    float pixelPerUnit=100f;
    
    public void Init(Image img,Vector2 scale, float pixelPerUnit=100f)
    {
        imgComponent = img;
        imgComponent.transform.localScale = new Vector3(scale.x, scale.y, 1);
        this.pixelPerUnit = pixelPerUnit;
    }

    public void ShowFogOfWar(Texture2D texure,Vector2 texSize,Vector2 spriteScale)
    {
        var sp = CreateSprite(texure, texSize);

        if (imgComponent!=null)
        {
            imgComponent.sprite = sp;
            imgComponent.SetNativeSize();
            imgComponent.rectTransform.localScale = new Vector3(spriteScale.x,spriteScale.y,1);
            
        }
        else
        {
            Debug.LogError("没有显示组件，无法显示战争迷雾");
        }
    }

    Sprite CreateSprite(Texture2D tex,Vector2 texSize)
    {
        return Sprite.Create(tex, new Rect(Vector2.zero, texSize), Vector2.zero, pixelPerUnit);
    }
}
