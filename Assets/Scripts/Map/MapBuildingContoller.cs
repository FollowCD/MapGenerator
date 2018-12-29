using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 建筑逻辑处理单元
/// 1.保存相关状态(这个状态是显示状态)
/// 2.向外界抛出相关事件
/// 3.接受外界相关事件并作出响应
/// </summary>
[RequireComponent(typeof(MapBuildingView))]
public class MapBuildingContoller : MonoBehaviour {

    /* y
     * ^
     * |
     * 0 ——> x 建筑尺寸分布情况
     */
    public BuildingRect BuildingRect;
    MapManager mapManager;
    [HideInInspector]
    public bool IsRigidPositionBuilding { get; set; }
    public int BuildingSizeCount { get { return BuildingRect.w * BuildingRect.h; } }
    MapBuildingView view;
    bool BtnIsChosenState =false;

    private void Start()
    {
        Init();
    }

    public void SetMapManager(MapManager manager)
    {
        mapManager = manager;
    }

    public void Init()
    {
        view = GetComponent<MapBuildingView>();
        view.Init(OnBuildingClick);

    }

    public void SetInteractable(bool interactable)
    {
        if (view!=null)
        {
            view.SetInteractable(interactable);
        }
    }

    public void SetBuildingToUnchoseState()
    {
        if (BtnIsChosenState)
        {
            OnBuildingClick();
        }

    }

    /// <summary>
    /// 当建筑被点击，做出对应的响应
    /// </summary>
    /// <param name="isOn"></param>
    void OnBuildingClick()
    {
        BtnIsChosenState = !BtnIsChosenState;
        ChangeBuildingView(BtnIsChosenState);
        TriggerEvent(BtnIsChosenState);

        if (BtnIsChosenState && mapManager)
        {
            mapManager.UpdateLastChosenBuilding(this);
        }
    }

    private void ChangeBuildingView(bool isOn)
    {
        if (view!=null)
        {
            view.ShowChosenState(isOn);
        }
        
    }

    private void TriggerEvent(bool isOn)
    {
        //TODO:触发点击事件
    }

    public void ChangeBuildingImage(int imgIndex)
    {
        var sp = GetBuildingSp(imgIndex);

        if (sp!=null  && view !=null)
        {
            view.ChangeBuildingImage(sp);
        }
    }

    public void ShowTaskInfo(int taskId,int content)
    {
        var sp = GetTaskSprite(taskId);
        var contentStr = GetTaskContent(content);
        if (sp!=null && view!=null)
        {
            view.ShowTaskImage(sp, contentStr);
        }
    }

    public void SetTransform(Transform parent, Vector2 pos, Vector2 scale, Vector3 euler)
    {
        if (view!=null)
        {
            view.SetTransform(parent, pos, scale, euler);
        }
    }


    //TODO 
    private string GetTaskContent(int content)
    {
        return "";
    }

    private Sprite GetTaskSprite(int taskId)
    {
        //TODO:加载任务图片
        return null;
    }

    private Sprite GetBuildingSp(int imgIndex)
    {
        //TODO:根据资源编号加载building图片
        return null;
    }
}
