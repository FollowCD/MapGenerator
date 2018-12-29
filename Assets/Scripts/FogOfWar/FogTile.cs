using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FogTile : MonoBehaviour {
    Image img;
    int spriteIndex = 0;
    private void Awake()
    {
        FindImg();
    }

    private void FindImg()
    {
        if (img==null)
        {
            img = GetComponent<Image>();
        }
    }

    public void SetRecieveRayCast(bool rec)
    {
        FindImg();
        img.raycastTarget = rec;
    }

    public void SetSpriteAndRaycastHit(Sprite sp,bool rec=true)
    {
        FindImg();

        if (sp!=null && img.sprite!=sp)
        {
            img.sprite = sp;
        }

        spriteIndex = GetSpriteOrder(sp);
        img.raycastTarget = rec;
    }

    public int GetSpOrder
    {
        get
        {
            return spriteIndex;
        }
    }

    int GetSpriteOrder(Sprite sp)
    {
        var order = -1;
        if (sp != null)
        {
            var index = sp.name.Split('_')[1];
            int.TryParse(index, out order);
        }
        else
        {
            Debug.LogError("无法获取图片资源的序列号,图片为空");
        }

        return  order;
    }

    public void Show(bool show)
    {
        if (gameObject.activeSelf!=show)
        {
            gameObject.SetActive(show);
        }
    }
}
