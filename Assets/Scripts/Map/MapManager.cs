using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapManager : MonoBehaviour {

    //1.地图生成

    /*2.地图表现维护
     *2.1 新增区域，改变周围围栏状况
     *2.2 新增区域，改变街道/建筑 血迹装饰
     *
     *2.3 缩减区域，改变周围围栏状况
     *2.4 缩减区域，改变街道/建筑 血迹装饰
     * 
     */

    //3.地图位置维护

    /*4.对外接口
     * 
     * 4.1提供激活建筑向外扩展一圈的建筑位置信息
     * 
     * 4.2提供迷雾增量范围和中心位置(一个方框)
     * 
     * 4.3提供 显示/取消显示 建筑任务的方法
     */

    #region 成员变量
    MapGenerator mapGenerator;

    List<Vector2Int> ActivedCoords;//已激活的区域坐标

    List<Vector2Int> ActivedBorderCoords;//已激活区域的边界坐标(属于已激活区域)


    List<BuildingRect> ActiveSurroundBuildings;//已激活区域外面一圈建筑信息（不包含已激活区域）

    List<BuildingRect> ShownBuildings;//显示区域建筑信息(不包含 已激活区域和 已激活外面一圈区域)

    List<MapBuildingContoller> InteractableBuildingList;

    MapConfig mapConfigData;

    Vector2 FogNormalSize = Vector2.zero;
    float FogWideStreetXLength = 0f;
    float FogWideStreetYLength = 0f;
    Vector2 FogWideStreetOffset = Vector2.zero;

    GameObject[] CrossFence = new GameObject[4];//顺序 x0 x1 y0 y1
    Vector2Int[] CrossCoords = new Vector2Int[4];//顺序 leftDown, leftUp, rightDown, rightUp;

    MapBuildingContoller lastChosenBuilding;
    #endregion

    #region mono生命周期函数
    private void Awake()
    {
        
    }

    private void Start()
    {
        
    }

  
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            InitManager();


          //  fogManager.Init(mapConfigData.GetFogOfWarSize());
            Debug.Log("初始化完成");
        }

        if (Input.GetKeyDown(KeyCode.U))
        {

        }

        TestShowFog();
    }
    #endregion

    #region 测试代码,记得删除
    public FogOfWarManager fogManager;
    public List<Vector2Int> activeList;
    public List<Vector2Int> disActiveList;
    void TestShowFog()
    {


        if (Input.GetKeyDown(KeyCode.B))
        {
            var activeOne = FindBuilding(activeList);
            var disActiveOne = FindBuilding(disActiveList);

            UpdateActiveAreas(activeOne, disActiveOne);

        }
    }

    List<BuildingRect> FindBuilding(List<Vector2Int> list)
    {
        if (mapGenerator!=null && list!=null && list.Count>0)
        {
            var result = new List<BuildingRect>();
            foreach (var item in list)
            {
                var data = mapGenerator.buildingArray.GetData(item);
                if (result.Contains(data.BuildingRect))
                {
                    continue;
                }
                else
                {
                    result.Add(data.BuildingRect);
                }

            }

            return result;
        }

        return null;
    }
    #endregion

    #region 初始化&资源加载
    void InitManager()
    {
        mapGenerator = GetComp<MapGenerator>("MapGenerator",transform);
        ActivedCoords = new List<Vector2Int>();
        var fadeData = Resources.Load<FadeDataForConfig>("config");
        mapConfigData = new MapConfig(fadeData);

        if (mapGenerator!=null && mapConfigData!=null)
        {
            mapGenerator.InitGenerator(mapConfigData);
            mapGenerator.GeneratorMap();
        }
        else
        {
            Debug.LogError(string.Format("初始化失败：MapGenerator为空：{0}，MapConfig为空：{1}", mapGenerator == null, mapConfigData == null));
        }

        InitFogSize();
    }

    void InitFogSize()
    {
        if (mapConfigData!=null)
        {
            FogNormalSize = (mapConfigData.BuildingSize + mapConfigData.StreetSize)*2f;
           
            FogWideStreetOffset.x = (mapConfigData.WideStreetWidth - 1) * 0.5f * mapConfigData.StreetSize.x;
            FogWideStreetOffset.y = (mapConfigData.WideStreetWidth - 1) * 0.5f * mapConfigData.StreetSize.y;

            FogWideStreetXLength = FogNormalSize.x + 2 * FogWideStreetOffset.x;
            FogWideStreetYLength = FogNormalSize.y + 2 * FogWideStreetOffset.y;
        }
        else
        {
            Debug.LogError("配置数据为空，无法初始化战争迷雾显示数据");
        }
    }

    void InitBuildingControllers()
    {
        if (mapGenerator!=null && mapGenerator.buildingArray!=null)
        {
            foreach (var item in mapGenerator.buildingArray)
            {
                if (item != null)
                {
                    item.SetMapManager(this);
                }
            }
        }
    }

    #endregion

    #region 围栏显示 & 隐藏
    void ShowFence(List<Vector2Int> activeAreas, List<Vector2Int> disactiveAreas)
    {
        if (ActivedCoords == null)
        {
            ActivedCoords = new List<Vector2Int>();
        }

        if (activeAreas.HasElement())
        {
            CleanList(activeAreas);
            UpdateFenceShow(activeAreas,true);
        }


        if (disactiveAreas.HasElement())
        {
            CleanList(disactiveAreas);
            UpdateFenceShow(disactiveAreas,false);

        }

        UpdateWideStreetFenceShow();

        UpdateCrossFenceShow();


    }

    //更新两条款街道交汇十字路口的四个围栏显示
    private void UpdateCrossFenceShow()
    {
  

        var wideStreetPos = mapConfigData.WideStreetPos;
        var xMax = mapConfigData.MapWidth + 1;
        var yMax = mapConfigData.MapHeight + 1;

        var xInRange = wideStreetPos.x > 0 && wideStreetPos.x < mapConfigData.MapHeight;
        var yInRange = wideStreetPos.x > 0 && wideStreetPos.x < mapConfigData.MapHeight;

        if (xInRange && yInRange)
        {   
            if (CrossFence[0]==null)
            {
                CrossFence[0] = GetFenceObj(mapGenerator.xFencesDict,yMax, mapConfigData.MapWidth);
                CrossFence[1] = GetFenceObj(mapGenerator.xFencesDict, wideStreetPos.y, mapConfigData.MapWidth);
                CrossFence[2] = GetFenceObj(mapGenerator.yFencesDict, xMax, mapConfigData.MapHeight);
                CrossFence[3] = GetFenceObj(mapGenerator.yFencesDict, wideStreetPos.x, mapConfigData.MapHeight);

                CrossCoords[0] = new Vector2Int(wideStreetPos.x - 1, wideStreetPos.y - 1);
                CrossCoords[1] = new Vector2Int(wideStreetPos.x - 1, wideStreetPos.y);
                CrossCoords[2] = new Vector2Int(wideStreetPos.x, wideStreetPos.y - 1);
                CrossCoords[3] = new Vector2Int(wideStreetPos.x, wideStreetPos.y);
            }

            bool leftDownActive, leftUpActive, rightDownActive, rightUpActive;
            bool onlyLeftActive, onlyRightActive, onlyUpActive, onlyDownActive;
            bool onlyLeftUpDisactive, onlyLeftDownDisactive, onlyRightUpDisactive, onlyRightDownDisactive;

            leftDownActive = ActivedCoords.Contains(CrossCoords[0]);
            leftUpActive = ActivedCoords.Contains(CrossCoords[1]);
            rightDownActive = ActivedCoords.Contains(CrossCoords[2]);
            rightUpActive = ActivedCoords.Contains(CrossCoords[3]);

            //只有两个激活
            onlyLeftActive = leftDownActive && leftUpActive && !rightDownActive && !rightUpActive;
            onlyRightActive = !leftDownActive && !leftUpActive && rightDownActive && rightUpActive;
            onlyUpActive = !leftDownActive && leftUpActive && !rightDownActive && rightUpActive;
            onlyDownActive = leftDownActive && !leftUpActive && rightDownActive && !rightUpActive;

            //只有三个激活
            onlyLeftUpDisactive = leftDownActive && !leftUpActive && rightDownActive && rightUpActive;
            onlyLeftDownDisactive = !leftDownActive && leftUpActive && rightDownActive && rightUpActive;
            onlyRightUpDisactive = leftDownActive && leftUpActive && rightDownActive && !rightUpActive;
            onlyRightDownDisactive = leftDownActive && leftUpActive && !rightDownActive && rightUpActive;


            if (onlyLeftUpDisactive)
            {
                ShowCrossFence(true, false, false, true);
            }
            else if (onlyLeftDownDisactive)
            {
                ShowCrossFence(false, true, false, true);
            }
            else if (onlyRightUpDisactive)
            {
                ShowCrossFence(true, false, true, false);
            }
            else if (onlyRightDownDisactive)
            {
                ShowCrossFence(false, true, true, false);
            }
            else
            {
                if (onlyLeftActive)//左激活
                {
                    ShowCrossFence(false, false, true, false);
                }
                else if (onlyRightActive)
                {
                    ShowCrossFence(false, false, false, true);
                }
                else if (onlyUpActive)
                {
                    ShowCrossFence(false, true, false, false);
                }
                else if (onlyDownActive)
                {
                    ShowCrossFence(true, false, false, false);
                }
                else
                {
                    ShowCrossFence(false, false, false, false);
                }
            }

           
        }
    }

    void ShowCrossFence(bool showOne,bool showTwo,bool showThree,bool showFour)
    {
        if (CrossFence!=null && CrossFence.Length>0)
        {
            CrossFence[0].SetActive(showOne);
            CrossFence[1].SetActive(showTwo);
            CrossFence[2].SetActive(showThree);
            CrossFence[3].SetActive(showFour);
        }
    }

    //更新款街道上的围栏显示
    private void UpdateWideStreetFenceShow()
    {
        var wideStreetPos = mapConfigData.WideStreetPos;
        var xMax = mapConfigData.MapWidth + 1;
        var yMax = mapConfigData.MapHeight + 1;

        Vector2Int leftDown, leftUp, rightDown, rightUp;
        bool leftDownActive, leftUpActive, rightDownActive, rightUpActive;
        bool onlyLeftActive, onlyRightActive, onlyUpActive, onlyDownActive;
        bool onlyLeftUpDisactive, onlyLeftDownDisactive, onlyRightUpDisactive, onlyRightDownDisactive;

        //更新y方向的
        if (wideStreetPos.x>0&& wideStreetPos.x<mapConfigData.MapHeight)
        {
            for (int i = -1; i < mapConfigData.MapWidth; i++)
            {
                leftDown = new Vector2Int(i, wideStreetPos.y - 1);
                leftUp = new Vector2Int(i, wideStreetPos.y);
                rightUp = new Vector2Int(i + 1, wideStreetPos.y);
                rightDown = new Vector2Int(i + 1, wideStreetPos.y - 1);

                leftDownActive = ActivedCoords.Contains(leftDown);
                leftUpActive = ActivedCoords.Contains(leftUp);
                rightUpActive = ActivedCoords.Contains(rightUp);
                rightDownActive = ActivedCoords.Contains(rightDown);

                
                onlyLeftActive = leftDownActive && leftUpActive && !rightDownActive && !rightUpActive;
                onlyRightActive = !leftDownActive && !leftUpActive && rightDownActive && rightUpActive;


                onlyLeftUpDisactive = leftDownActive && !leftUpActive && rightDownActive && rightUpActive;
                onlyLeftDownDisactive = !leftDownActive && leftUpActive && rightDownActive && rightUpActive;
                onlyRightUpDisactive = leftDownActive && leftUpActive && rightDownActive && !rightUpActive;
                onlyRightDownDisactive = leftDownActive && leftUpActive && !rightDownActive && rightUpActive;

                var show = onlyLeftActive || onlyRightActive || onlyLeftUpDisactive || onlyLeftDownDisactive || onlyRightUpDisactive || onlyRightDownDisactive;

                var index = i + 1;

                //  var obj = mapGenerator.yFencesDict[index][yMax];
                var obj = GetFenceObj(mapGenerator.yFencesDict, index, yMax-1);

                if (obj)
                {
                    obj.SetActive(show);
                }

            }
        }

        //更新x方向的
        if (wideStreetPos.x > 0 && wideStreetPos.x < mapConfigData.MapHeight)
        {
            for (int i = -1; i < mapConfigData.MapHeight; i++)
            {
                leftDown = new Vector2Int(wideStreetPos.x-1,i);
                leftUp = new Vector2Int(wideStreetPos.x-1,i+1);
                rightUp = new Vector2Int(wideStreetPos.x,i+1);
                rightDown = new Vector2Int(wideStreetPos.x,i);

                leftDownActive = ActivedCoords.Contains(leftDown);
                leftUpActive = ActivedCoords.Contains(leftUp);
                rightUpActive = ActivedCoords.Contains(rightUp);
                rightDownActive = ActivedCoords.Contains(rightDown);

                onlyUpActive = !leftDownActive && leftUpActive && !rightDownActive && rightUpActive;
                onlyDownActive = leftDownActive && !leftUpActive && rightDownActive && !rightUpActive;

                onlyLeftUpDisactive = leftDownActive && !leftUpActive && rightDownActive && rightUpActive;
                onlyLeftDownDisactive = !leftDownActive && leftUpActive && rightDownActive && rightUpActive;
                onlyRightUpDisactive = leftDownActive && leftUpActive && rightDownActive && !rightUpActive;
                onlyRightDownDisactive = leftDownActive && leftUpActive && !rightDownActive && rightUpActive;

                var show = onlyUpActive || onlyDownActive || onlyLeftUpDisactive || onlyLeftDownDisactive || onlyRightUpDisactive || onlyRightDownDisactive;

                var index = i + 1;

                //var obj = mapGenerator.xFencesDict[index][yMax];
                var obj = GetFenceObj(mapGenerator.xFencesDict, index, xMax-1);

                if (obj)
                {
                    obj.SetActive(show);
                }

            }
        }
    }

    GameObject GetFenceObj(Dictionary<int, List<GameObject>> dict, int key, int order)
    {
        if (dict.HasElement())
        {
            List<GameObject> list;

            dict.TryGetValue(key, out list);

            if (list != null && list.Count > order)
            {
                return list[order];
            }
            else
            {
                Debug.LogError(string.Format("无法获取Fence，key:{0},key对应list为空：{1}，或者order越界",
                    key, list == null));
            }
        }
        else
        {
            Debug.LogError("无法获取Fence，字典为空");
        }
        return null;
    }

    private void UpdateFenceShow(List<Vector2Int> activeAreas, bool show)
    {
        foreach (var item in activeAreas)
        {
            var atLeft = AtWideStreetLeft(item);
            var atRight = AtWideStreetRight(item);
            var atUp = AtWideStreetUp(item);
            var atDown = AtWideStreetDown(item);

            var nearWideStreet = atLeft || atRight || atUp || atDown;
            var ambientBuildingCoords = GetAmbientBuildingCoords(item);

            if (nearWideStreet)//挨着宽街道特殊处理
            {
                UpdateBuildingNearWideStreetFenceShow(item, ambientBuildingCoords,show,atLeft, atRight, atUp, atDown);
            }
            else//普通处理
            {
                UpdateNormalBuildingFenecShow(item, ambientBuildingCoords, show);
            }
        }
    }

    /// <summary>
    /// 更新普通建筑周围的围栏显示
    /// </summary>
    /// <param name="item"> 要更新的建筑坐标 </param>
    /// <param name="ambientBuildingCoords"> 周围四个建筑坐标 </param>
    /// <param name="show"> 要更新的建筑是显示还是隐藏 </param>
    private void UpdateNormalBuildingFenecShow(Vector2Int item, List<Vector2Int> ambientBuildingCoords, bool show)
    {
        var buildFenceData = GetBuildingFenceData(item);

        if (buildFenceData==null)
        {
            return;
        }

        for (int i = 0; i < ambientBuildingCoords.Count; i++)
        {
            UpdateBuildingOneFence(ambientBuildingCoords[i], show, i, buildFenceData);
        }
    }

    /// <summary>
    /// 更新临款街道 建筑周围的围栏显示
    /// </summary>
    /// <param name="item">要更新的建筑坐标 </param>
    /// <param name="ambientBuildingCoords"> 周围四个建筑坐标</param>
    /// <param name="show">要更新的建筑是显示还是隐藏</param>
    /// <param name="atLeft"> 在宽街道左边 </param>
    /// <param name="atRight"> 在宽街道右边</param>
    /// <param name="atUp">在宽街道上边 </param>
    /// <param name="atDown"> 在宽街道下边</param>
    private void UpdateBuildingNearWideStreetFenceShow(Vector2Int item, List<Vector2Int> ambientBuildingCoords, bool show, bool atLeft, bool atRight, bool atUp, bool atDown)
    {
        var buildFenceData = GetBuildingFenceData(item);
        var totalCount = 4;
        if (buildFenceData==null)
        {
            return;
        }

        int skipIndex = 0;
        int skipIndex2 = 0;
        FencePosition mainSide = FencePosition.Down;

        bool atLeftDown, atLeftUp, atRightDown, atRightUp;

        atLeftDown = atLeft && atDown;
        atLeftUp = atLeft && atUp;
        atRightDown = atRight && atDown;
        atRightUp = atRight && atUp;

        if (atLeftDown||atLeftUp||atRightDown||atRightUp)
        {
            var  mainSide2 = FencePosition.Right;

            if (atLeftDown)
            {
                mainSide = FencePosition.Right;
                mainSide2 = FencePosition.Up;

            }
            else if (atLeftUp)
            {
                mainSide = FencePosition.Right;
                mainSide2 = FencePosition.Down;
            }
            else if (atRightUp)
            {
                mainSide = FencePosition.Left;
                mainSide2 = FencePosition.Down;
            }
            else if (atRightDown)
            {
                mainSide = FencePosition.Left;
                mainSide2 = FencePosition.Up;
            }

            skipIndex = (int)mainSide;
            skipIndex2 = (int)mainSide2;

            for (int i = 0; i < totalCount; i++)// 下 - 左 - 上 三条边普通处理
            {
                if (i == skipIndex||i==skipIndex2)
                {
                    continue;
                }
                UpdateBuildingOneFence(ambientBuildingCoords[i], show, i, buildFenceData);
            }

            UpdateBothSideWideStreetBuildFence(buildFenceData, show, mainSide, GetOppositePosition(mainSide), ambientBuildingCoords[skipIndex]);
            UpdateBothSideWideStreetBuildFence(buildFenceData, show, mainSide2, GetOppositePosition(mainSide2), ambientBuildingCoords[skipIndex2]);


        }       
        else if(atLeft||atRight||atUp||atDown)
        {         

            if (atLeft)//在街道左边，则要对自己右边建筑的 左边 围栏进行特殊处理（编号为）
            {
                mainSide = FencePosition.Right;
            }
            else if (atRight)//在街道右边，则要对自己左边建筑的  右边 围栏进行特殊处理
            {
                mainSide = FencePosition.Left;
            }
            else if (atUp)//在街道上面，则要对自己下面建筑的 上面 围栏进行特殊处理
            {
                mainSide = FencePosition.Down;
            }
            else if (atDown)//在街道下面，则要对自己上面建筑 的 下边 围栏进行特殊处理
            {
                mainSide = FencePosition.Up;
            }

            skipIndex = (int)mainSide;

            for (int i = 0; i < totalCount; i++)// 下 - 左 - 上 三条边普通处理
            {
                if (i == skipIndex)
                {
                    continue;
                }
                UpdateBuildingOneFence(ambientBuildingCoords[i], show, i, buildFenceData);
            }

            UpdateBothSideWideStreetBuildFence(buildFenceData, show, mainSide, GetOppositePosition(mainSide), ambientBuildingCoords[skipIndex]);

        }


    }

    void UpdateBothSideWideStreetBuildFence(MapBuildingFence mainBuildData,bool showMainBuild,FencePosition mainBuildSide,FencePosition oppositeBuildSide,Vector2Int oppositeBuildCoord)
    {
        if (PosValid(oppositeBuildCoord))//如果对面是在地图里面的
        {
            var oppositeBuildFenceData = GetBuildingFenceData(oppositeBuildCoord);
            if (oppositeBuildFenceData == null)
            {
                return;
            }

            if (ActivedCoords.Contains(oppositeBuildCoord))//如果对面是激活的
            {
                mainBuildData.ShowFence(mainBuildSide, false);//自己则永远不激活临近的一面

                oppositeBuildFenceData.ShowFence(oppositeBuildSide, !showMainBuild);//对面则看自己是否激活来确定激活临近自己这一边
            }
            else
            {
                mainBuildData.ShowFence(mainBuildSide, showMainBuild);
                oppositeBuildFenceData.ShowFence(oppositeBuildSide, false);
            }
        }
        else
        {
            mainBuildData.ShowFence(mainBuildSide, showMainBuild);
        }
    }

    List<Vector2Int> GetAmbientBuildingCoords(Vector2Int item)
    {
        var list = new List<Vector2Int>();

        list.Add(new Vector2Int(item.x, item.y - 1));
        list.Add(new Vector2Int(item.x-1, item.y ));
        list.Add(new Vector2Int(item.x, item.y + 1));
        list.Add(new Vector2Int(item.x+1, item.y));

        return list;
    }

    void UpdateBuildingOneFence(Vector2Int nearBuildingCoord,bool showBuilding,int nearBuildingIndex,MapBuildingFence buildFenceData)
    {
        var nearBy = nearBuildingCoord;
        var showFence = false;
        if (ActivedCoords.Contains(nearBy))//临近建筑激活，那么自己显示则墙不显示，状态相反
        {
            showFence = !showBuilding;
        }
        else
        {
            showFence = showBuilding;
        }

        buildFenceData.ShowFence((FencePosition)nearBuildingIndex, showFence);
    }

    MapBuildingFence GetBuildingFenceData(Vector2Int item)
    {
        var key = new Vector2Byte(item);
        if (mapGenerator.buildingFenceDict != null && mapGenerator.buildingFenceDict.ContainsKey(key))
        {
           return mapGenerator.buildingFenceDict[key];
        }
        else
        {
            Debug.LogError("没有坐标数据：" + item);
            return null;
        }
    }

    bool AtWideStreetLeft(Vector2Int pos)
    {
        return mapConfigData.WideStreetPos.x - 1 == pos.x;
    }

    bool AtWideStreetRight(Vector2Int pos)
    {
        return mapConfigData.WideStreetPos.x == pos.x;
    }

    bool AtWideStreetUp(Vector2Int pos)
    {
        return mapConfigData.WideStreetPos.y == pos.y;
    }

    bool AtWideStreetDown(Vector2Int pos)
    {
        return mapConfigData.WideStreetPos.y - 1 == pos.y;
    }
    #endregion

    #region 管理已激活区域
    void UpdateActivedAreasList(List<Vector2Int> newActiveAreas,List<Vector2Int> newDisactiveAreas)
    {
        if (ActivedCoords==null)
        {
            ActivedCoords = new List<Vector2Int>(100);
        }

        ActivedCoords.MergeCollection(newActiveAreas);
        ActivedCoords.DeleteCollections(newDisactiveAreas);
    }

    public void UpdateLastChosenBuilding(MapBuildingContoller crtOne)
    {
        if (lastChosenBuilding!=crtOne)
        {
            if (lastChosenBuilding!=null)
            {
                lastChosenBuilding.SetBuildingToUnchoseState();
            }

            lastChosenBuilding = crtOne;
        }
    }
    #endregion

    #region 获取建筑外圈层信息
    void UpdateSurroundBuildingsInfo(List<Vector2Int> newActiveCoords,List<Vector2Int> newDisactiveCoords,bool forceUpdate=false)
    {
        if (newActiveCoords.IsNullOrEmpty() && newDisactiveCoords.IsNullOrEmpty() && !forceUpdate)
        {
            return;
        }
        else
        {

            var activedSurroundCoords = GetBuildingSurroundingCoord(ActivedCoords, ActivedCoords);
            var shownCoords = GetBuildingSurroundingCoord(activedSurroundCoords, ActivedCoords, activedSurroundCoords);

            ActiveSurroundBuildings = GetBuildingsSurroundings(activedSurroundCoords);

            ShownBuildings = GetBuildingsSurroundings(shownCoords);
        }
    }

    //获取周围一圈的位置信息
    List<Vector2Int> GetBuildingSurroundingCoord(List<Vector2Int> traverseList, params List<Vector2Int>[] conditionList)
    {
        if (traverseList.IsNullOrEmpty())
        {
            Debug.Log("没有激活区域，无法获取激活区域外圈信息");
            return null;
        }
        else
        {
            List<Vector2Int> result = new List<Vector2Int>();
            for (int i = 0; i < traverseList.Count; i++)
            {
                var surroundings = GetBuildingDisactiveAmbients(traverseList[i], conditionList);
                result.MergeCollection(surroundings);
            }

            return result;
        }
    }

    //根据周围一圈的位置信息，获取对应周围一圈的建筑信息
    List<BuildingRect> GetBuildingsSurroundings(List<Vector2Int> result)
    {
        if (result==null)
        {
            return null;
        }

        if (result.Count > 0)
        {
            List<BuildingRect> targetBuilding = new List<BuildingRect>();
            for (int i = 0; i < result.Count; i++)
            {
                var buildingCoord = result[i];

                var building = mapGenerator.buildingArray.GetData(buildingCoord);

                if (building != null && !targetBuilding.Contains(building.BuildingRect))
                {
                    targetBuilding.Add(building.BuildingRect);
                }
            }

            return targetBuilding;
        }

        return null;

    }

    List<Vector2Int> GetBuildingDisactiveAmbients(Vector2Int pos,params List<Vector2Int>[] conditionList)
    {
        return GetBuildingDisactiveAmbients(pos.x, pos.y,conditionList);
    }

    List<Vector2Int> GetBuildingDisactiveAmbients(int x,int y,params List<Vector2Int>[] conditionList)
    {
        if (conditionList==null)
        {
            return null;
        }

        var startX = x - 1;
        var startY = y - 1;

        List<Vector2Int> list = new List<Vector2Int>(9);
        for (int i = 0; i < 3; i++)// y递增
        {
            for (int j = 0; j < 3; j++)//x 递增
            {
                var data = new Vector2Int(startX + j, startY + i);

                bool dataValid = true;
                if (PosValid(data))
                {
                    foreach (var item in conditionList)
                    {
                        if (item!=null)
                        {
                            if (item.Contains(data))
                            {
                                dataValid = false;
                                break;
                            }
                        }
                    }

                    if (dataValid)
                    {
                        list.Add(data);
                    }
                }
            }
        }

        list.Remove(new Vector2Int(x, y));
        return list;
    }

    #endregion

    #region 获取距离已激活区域N步的位置

    //获取单个坐标距离自己距离为 min-max 的未激活的坐标点
    List<Vector2Int> GetAreasFarFormTarget(Vector2Int pos,int maxDis,int totalCount)
    {
        if (maxDis<=0)
        {
            var result = new List<Vector2Int>(1);
            result.Add(pos);
            return result;
        }
        var xmin = pos.x - maxDis;
        var xmax = pos.x + maxDis;

        var ymin = pos.y - maxDis;
        var ymax = pos.y + maxDis;

        //地图坐标中，左下角为（0,0）点，然后x向右递增，y向上递增
        xmin = Mathf.Clamp(xmin, 0, mapConfigData.MapWidth - 1);
        xmax = Mathf.Clamp(xmax, 0, mapConfigData.MapWidth - 1);

        ymin = Mathf.Clamp(ymin, 0, mapConfigData.MapHeight - 1);
        ymax = Mathf.Clamp(ymax, 0, mapConfigData.MapHeight - 1);


        List<Vector2Int> list = new List<Vector2Int>(totalCount);
        for (int i = xmin; i <=xmax ; i++)
        {
            var tempx =Mathf.Abs(i - pos.x);
            for (int j = ymin; j <=ymax; j++)
            {
                var tempy = Math.Abs(j - pos.y);
                var result = tempx + tempy;
                if (result<=maxDis)
                {
                    var coord = new Vector2Int(i, j);
                    if (!ActivedCoords.Contains(coord))
                    {
                        list.Add(new Vector2Int(i, j));
                    }
                }
            }
        }

        return list;

    }

    //更新已激活区域的边界(包含在已激活区域内)
    void UpdateActivedAreasBorder()
    {
        if (ActivedCoords.IsNullOrEmpty())
        {
            return ;
        }
        else
        {
            var result = new List<Vector2Int>();
            foreach (var item in ActivedCoords)
            {
                var list = GetAmbientBuildingCoords(item);

                if (list!=null && list.Count>0)
                {
                    foreach (var coord in list)
                    {
                        if (!ActivedCoords.Contains(coord))
                        {
                            result.Add(item);
                            break;
                        }
                    }
                }
            }

            ActivedBorderCoords = result;
        }
    }

    //获取所有距离已激活区域距离 为  [minDis,maxDis] 之间的所有坐标
    List<Vector2Int> GetAllAreasInRange(int minDis,int maxDis)
    {
        var valueValid = DealWithMinAndMaxValue(ref minDis, ref maxDis);

        if (!valueValid)
        {
            return null;
        }

        if (ActivedBorderCoords.IsNullOrEmpty())
        {
            if (ActivedCoords!=null && ActivedCoords.Count>0)
            {
                UpdateActivedAreasBorder();
            }
            else { return null; }
        }

        if (ActivedBorderCoords.IsNullOrEmpty())
        {
            return null;
        }

        var count = GetCount(maxDis) - GetCount(minDis - 1);
        var totalCount = ActivedBorderCoords.Count * count;
        var result = new List<Vector2Int>(totalCount);
        
        for (int i = 0; i < ActivedBorderCoords.Count; i++)
        {
            var list = GetAreasFarFormTarget(ActivedBorderCoords[i],maxDis, GetCount(maxDis));

            result.MergeCollection(list);
        }

        for (int i = 0; i < ActivedBorderCoords.Count; i++)
        {
            var list = GetAreasFarFormTarget(ActivedBorderCoords[i], minDis - 1, GetCount(minDis - 1));
            result.DeleteCollections(list);
        }

        return result;

    }

    int GetCount(int dis)
    {
        if (dis<=0)
        {
            return 1;
        }

        var mid = 2 * dis + 1;

        var half = (1 + mid - 2) * dis;

        return mid + half;
    }

    bool DealWithMinAndMaxValue(ref int minDis, ref int maxDis)
    {
        if (minDis < 0)
        {
            Debug.LogError("参数错误！最小值必须>=0");
            return false;
        }
        else if (maxDis > mapConfigData.MapHeight - 1)
        {
            Debug.LogError("参数错误！最大值必须小于:" + (mapConfigData.MapHeight - 1));
            return false;
        }
        else if (minDis > maxDis)
        {
            Debug.LogError("参数错误！最小值必须<=最大值！");
            return false;
        }

        return true;

    }

    #endregion

    #region 僵尸移动
    Vector2Int FindShortestPath(Vector2Int pos)
    {
        if (ActivedBorderCoords.IsNullOrEmpty())
        {
            if (ActivedCoords==null && ActivedCoords.Count>0)
            {
                UpdateActivedAreasBorder();
            }
        }


        if (ActivedBorderCoords.IsNullOrEmpty())
        {
            return Vector2Int.zero;
        }
        else
        {
            int shortestLength = 0;
            int shortestIndex = 0;

            var pos0 = ActivedBorderCoords[0];
            var x = Mathf.Abs(pos.x - pos0.x);
            var y = Mathf.Abs(pos.y - pos0.y);

            shortestLength = x + y;

            for (int i = 1; i < ActivedBorderCoords.Count; i++)
            {
                var tempPos = ActivedBorderCoords[i];
                var tempx = Mathf.Abs(pos.x - tempPos.x);
                var tempy = Mathf.Abs(pos.y - tempPos.y);
                var dis = tempx + tempy;

                if (dis<shortestLength)
                {
                    shortestLength = dis;
                    shortestIndex = i;
                }

            }


            return ActivedBorderCoords[shortestIndex];

        }


    }

    Vector2Int FindNextPosCoord(Vector2Int CrtPos,int step = 1)
    {
        if (ActivedCoords.Contains(CrtPos))
        {
            Debug.Log("出发区域已经在激活区域");
            return CrtPos;
        }

        var DesPos = FindShortestPath(CrtPos);
        Debug.Log("目的地：" + DesPos);
        var disX = CrtPos.x - DesPos.x;
        var disY = CrtPos.y - DesPos.y;
        var moveAlongX = UnityEngine.Random.Range(0, 2) > 0 ? true : false;

        for (int i = 0; i < step; i++)
        {
            if (moveAlongX)
            {
                if (disX != 0)
                {
                    if (disX > 0)
                    {
                        CrtPos.x -= 1;
                        disX -= 1;
                    }
                    else
                    {
                        CrtPos.x += 1;
                        disX += 1;
                    }
                }
                else if (disY != 0)
                {
                    if (disY > 0)
                    {
                        CrtPos.y -= 1;
                        disY -= 1;
                    }
                    else
                    {
                        CrtPos.y += 1;
                        disY += 1;
                    }
                }
            }
            else
            {
                if (disY != 0)
                {
                    if (disY > 0)
                    {
                        CrtPos.y -= 1;
                        disY -= 1;
                    }
                    else
                    {
                        CrtPos.y += 1;
                        disY += 1;
                    }
                }
                else if (disX != 0)
                {
                    if (disX > 0)
                    {
                        CrtPos.x -= 1;
                        disX -= 1;
                    }
                    else
                    {
                        CrtPos.x += 1;
                        disX += 1;
                    }
                }
              
            }

           
           

        }

        return CrtPos;
    }

    #endregion

    #region 建筑物距离安全区最短距离
    Vector2Int testbuilingCoord;
    Vector2Int testactiveCoord;
    int GetShortestDisToActivedAreas(BuildingRect buildingInfo)
    {
        if (buildingInfo==null)
        {
            Debug.LogError("参数错误，建筑信息为空");
            return 0;
        }
        else if (!PosValid(new Vector2Int(buildingInfo.x,buildingInfo.y)))
        {
            Debug.LogError(string.Format("给定建筑坐标已经超出地图范围，建筑位置：({0}{1})，地图大小：{2}x{3}"
                ,buildingInfo.x,buildingInfo.y,mapConfigData.MapWidth,mapConfigData.MapHeight));
            return 0;
        }
        else
        {
            var coordList = GetBuilingCoords(buildingInfo);
            
            int minDis = mapConfigData.MapWidth *mapConfigData.MapHeight;
            for (int i = 0; i < coordList.Count; i++)
            {
                var shortestDes = FindShortestPath(coordList[i]);
                var dis = GetDistance(shortestDes, coordList[i]);

                if (minDis>dis)
                {
                    minDis = dis;
                    testbuilingCoord = coordList[i];
                    testactiveCoord = shortestDes;
                }
            }

            return minDis;
        }
    }

    List<Vector2Int> GetBuilingCoords(BuildingRect builing)
    {
        var list = new List<Vector2Int>();
        for (int i = 0; i < builing.w; i++)
        {
            for (int j = 0; j < builing.h; j++)
            {
                var item = new Vector2Int(builing.x + i, builing.y + j);
                list.Add(item);
            }
        }

        return list;
    }
    #endregion

    #region 更新迷雾区域

    /*
     * 1.收复一个区域，那么地图点亮区域大小为  建筑尺寸*1.5 +街道尺寸
     * 
     * 2.如果这个东西临近宽街，那么宽街也被完全点亮
     * 
     * 
     */
     [Range(0.5f,1f)]
    public float outwallValue;
    Vector4 GetSingleFogAreaInfo(Vector2Int pos)
    {
        if (!PosValid(pos))
        {
            Debug.LogError("坐标越界,无法获取迷雾散开信息");
            return Vector4.zero;
        }

        var atVerticalStreetLeft = mapConfigData.PosAtVerticalWideStreetLeft(pos.x);
        var atVerticalStreetRight = mapConfigData.PosAtVerticalWideStreetRight(pos.x);

        var atHorizontalStreetUp = mapConfigData.PosAtHorizontalWideStreetUp(pos.y);
        var atHorizontalStreetDown = mapConfigData.PosAtHorizontalWideStreetDown(pos.y);

       

        var offsetX = 0f;
        var offsetY = 0f;
        Vector2 size = FogNormalSize;

        if (atVerticalStreetLeft)
        {
            offsetX = FogWideStreetOffset.x;
            size.x = FogWideStreetXLength;
        }
        else if (atVerticalStreetRight)
        {
            offsetX = -FogWideStreetOffset.x;
            size.x = FogWideStreetXLength;
        }

        if (atHorizontalStreetDown)
        {
            offsetY = FogWideStreetOffset.y;
            size.y = FogWideStreetYLength;
        }
        else if (atHorizontalStreetUp)
        {
            offsetY = -FogWideStreetOffset.y;
            size.y = FogWideStreetYLength;
        }

        //var atLeft = pos.x == 0;
        //var atRight = pos.x == mapConfigData.MapWidth - 1;
        //var atUp = pos.y == mapConfigData.MapHeight - 1;
        //var atDown = pos.y == 0;
        //var outwallX = mapConfigData.OuterWallSize.x * outwallValue;
        //var outwallY = mapConfigData.OuterWallSize.y * outwallValue;

        //if (atLeft)
        //{
        //    offsetX += -outwallX / 2;
        //    size.x += outwallX;
        //}
        //else if (atRight)
        //{
        //    offsetX += outwallX / 2;
        //    size.x += outwallX;
        //}

        //if (atUp)
        //{
        //    offsetY += outwallY / 2;
        //    size.y += outwallY;
        //}
        //else {
        //    offsetY += -outwallY / 2;
        //    size.y += outwallY;
        //}



        var buildPos = mapGenerator.GetBuildPos(pos);

        return new Vector4(buildPos.x + offsetX, buildPos.y + offsetY, size.x, size.y);

    }

    List<Vector4> GetFogOfWarUpdateAreas(List<Vector2Int> newAreas)
    {
        if (newAreas.IsNullOrEmpty())
        {
            return null;
        }
        else
        {
            var result = new List<Vector4>();
            foreach (var item in newAreas)
            {
                var data = GetSingleFogAreaInfo(item);

                if (data != Vector4.zero)
                {
                    result.Add(data);
                }
            }

            return result;
        }
    }

    #endregion

    #region 维护可以交互的建筑列表
    void UpdateInteractableBuildingList(List<Vector2Int> newActivedList,List<Vector2Int> newDisactivedList)
    {
        if (newActivedList!=null&& newActivedList.Count>0)
        {
            UpdateInteractableBuildingList(newActivedList,true);
        }

        if (newDisactivedList != null && newDisactivedList.Count > 0)
        {
            UpdateInteractableBuildingList(newDisactivedList, false);
        }
    }

    void UpdateInteractableBuildingList(List<Vector2Int> newlist,bool interactable)
    {
        for (int i = 0; i < newlist.Count; i++)
        {
            var surrondingList = GetBuildingDisactiveAmbients(newlist[i], ActivedCoords);

            if (surrondingList!=null && surrondingList.Count>0)
            {
                for (int j = 0; j < surrondingList.Count; j++)
                {
                    var buildingController = mapGenerator.buildingArray.GetData(surrondingList[j]);

                    if (buildingController!=null)
                    {
                        var existed = InteractableBuildingList.Contains(buildingController);

                        if (existed && !interactable)
                        {
                            InteractableBuildingList.Remove(buildingController);
                            buildingController.SetInteractable(interactable);
                        }
                        else if (interactable && !existed)
                        {
                            InteractableBuildingList.Add(buildingController);
                            buildingController.SetInteractable(interactable);
                        }
                    }
                }
            }
        }
    }
    #endregion

    #region 对外接口
    /// <summary>
    /// 获取已解锁建筑周围一圈的建筑信息
    /// </summary>
    /// <returns></returns>
    public List<BuildingRect> GetActivedBuildingsSurroundBuildings()
    {
        if (ActiveSurroundBuildings.IsNullOrEmpty())
        {
            if (ActivedCoords.IsNullOrEmpty())
            {
                Debug.Log("无法获取已激活区域周围一圈建筑信息，因为没有激活区域");
                return null;
            }
            else
            {
                UpdateSurroundBuildingsInfo(null, null, true);
                return ActiveSurroundBuildings;
            }
        }
        else
        {
            return ActiveSurroundBuildings;
        }
    }

    /// <summary>
    /// 获取显示区域最外一圈的建筑信息
    /// </summary>
    /// <returns></returns>
    public List<BuildingRect> GetShownBuildings()
    {
        if (ShownBuildings.IsNullOrEmpty())
        {
            if (ActivedCoords.IsNullOrEmpty())
            {
                Debug.Log("无法获取显示最外圈建筑信息，因为没有激活区域");
                return null;
            }
            else
            {
                UpdateSurroundBuildingsInfo(null, null, true);
                return ShownBuildings;
            }
        }
        else
        {
            return ShownBuildings;
        }
    }

    /// <summary>
    /// 获取新消失的迷雾区域
    /// </summary>
    /// <returns></returns>
    public List<Vector4> GetFogOfWarDecreaseAreas(List<Vector2Int> newDisactiveAreas)
    {
        return GetFogOfWarUpdateAreas(newDisactiveAreas);
    }

    /// <summary>
    /// 获取新的增加的迷雾区域
    /// </summary>
    /// <returns></returns>
    public List<Vector4> GetFogOfWarIncreaseAreas(List<Vector2Int> newActiveAreas)
    {
        return GetFogOfWarUpdateAreas(newActiveAreas);
    }

    /// <summary>
    /// 更新显示激活区域
    /// </summary>
    /// <param name="coords"></param>
    public void UpdateActiveAreas(List<BuildingRect> newActiveBuildings,List<BuildingRect> newDisactiveBuildings)
    {
        var newActiveCoords = ReverseBuilingRectToVector2Int(newActiveBuildings);
        var newDisactiveCoords = ReverseBuilingRectToVector2Int(newDisactiveBuildings);
        //更新已激活位置集合
        UpdateActivedAreasList(newActiveCoords, newDisactiveCoords);

        //更新已激活位置边界集合
        UpdateActivedAreasBorder();

        //更新圈层信息
        UpdateSurroundBuildingsInfo(newActiveCoords, newDisactiveCoords);

        //更新围墙显示
        ShowFence(newActiveCoords, newDisactiveCoords);

        //更新迷雾区域
        if (fogManager!=null)
        {
            var activeAreas = GetFogOfWarIncreaseAreas(newActiveCoords);
            var disactiveAreas = GetFogOfWarDecreaseAreas(newDisactiveCoords);

            fogManager.ShowArea(activeAreas, disactiveAreas);
        }

        //更新可以交互的建筑
        UpdateInteractableBuildingList(newActiveCoords, newDisactiveCoords);
    }

    /// <summary>
    /// 更新任务显示
    /// </summary>
    /// <param name="taskInfos"></param>
    public void UpdateTaskInfos(Dictionary<BuildingRect,TaskInfo> taskInfos)
    {
        if (taskInfos.IsNullOrEmpty())
        {
            return;
        }
        else
        {
            foreach (var item in taskInfos)
            {
                if (item.Key!=null && item.Value!=null)
                {
                    var buildingController = mapGenerator.buildingArray.GetData(new Vector2Int(item.Key.x, item.Key.y));

                    if (buildingController)
                    {
                        buildingController.ShowTaskInfo(item.Value.TaskId, item.Value.TaskId);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 获取距离已激活建筑群 距离为[minDis,maxDis]的范围内随机建筑(包含两端)
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public List<Vector2Int> GetNearbyCoords(int minDis,int maxDis) {
        return GetAllAreasInRange(minDis, maxDis);
    }

    /// <summary>
    /// 获取僵尸下一个到达的地点
    /// </summary>
    /// <param name="crtPos"> 僵尸起始位置 </param>
    /// <param name="MoveStep"> 僵尸移动距离 </param>
    /// <returns></returns>
    public Vector2Int GetNextCoord(Vector2Int crtPos,int MoveStep)
    {
        return FindNextPosCoord(crtPos, MoveStep);
    }

    /// <summary>
    /// 获取目标建筑物距离激活区域的最短距离
    /// </summary>
    /// <param name="buildInfo"> 建筑信息 </param>
    /// <returns></returns>
    public int GetDistanceToActivedArea(BuildingRect buildInfo)
    {
        return GetShortestDisToActivedAreas(buildInfo);
    }
    
    #endregion

    T GetComp<T>(string childName,Transform parent) where T : Component
    {
        if (parent)
        {

            var child = parent.Find(childName);

            if (child)
            {
                var t = child.GetComponent<T>();

                if (t==null)
                {
                    Debug.LogError(string.Format("在{0}下的子物体{1}上，找不到组件：{2}", parent.name, childName, typeof(T).Name));
                }
                return t;
            }
            else
            {
                Debug.LogError(string.Format("在{0}下找不到子物体：{1}", parent.name, childName));
            }

        }
        else
        {
            Debug.LogError("父物体为空！");
        }

        return null;
    }

    bool PosValid(Vector2Int pos)
    {
        if (mapConfigData==null)
        {
            return false;
        }
        else
        {
            return pos.x >= 0 && pos.x < mapConfigData.MapWidth && pos.y >= 0 && pos.y < mapConfigData.MapHeight;
        }
    }

    void CleanList(List<Vector2Int> list)
    {
        if (list.HasElement())
        {
            for (int i = list.Count-1; i >-1; i--)
            {
                var item = list[i];

                if (!PosValid(item))
                {
                    list.RemoveAt(i);
                }
            }
        }
    }

    int GetDistance(Vector2Int one, Vector2Int two)
    {
        var disx = Mathf.Abs(one.x - two.x);
        var disy = Mathf.Abs(one.y - two.y);

        return disx + disy;
    }

    FencePosition GetOppositePosition(FencePosition pos)
    {
        switch (pos)
        {
            case FencePosition.Down:
                return FencePosition.Up;
            case FencePosition.Left:
                return FencePosition.Right;
            case FencePosition.Up:
                return FencePosition.Down;
            case FencePosition.Right:
                return FencePosition.Left;
            default:
                return FencePosition.Left;
        }
    }

    List<Vector2Int> ReverseBuilingRectToVector2Int(List<BuildingRect>  list)
    {
        if (list.IsNullOrEmpty())
        {
            return null;
        }
        else if(mapGenerator==null)
        {
            Debug.LogError("地图生成器为空");
            return null;
        }
        else
        {
            List<Vector2Int> result = new List<Vector2Int>();
            foreach (var item in list)
            {
                if (item!=null)
                {
                    var myData = mapGenerator.buildingArray.GetData(item.x, item.y);

                    if (myData!=null )
                    {
                        if (myData.BuildingRect.IsEquals(item))
                        {
                            for (int i = 0; i < item.w; i++)
                            {
                                var x = item.x + i;
                                for (int j = 0; j < item.h; j++)
                                {
                                    var coord = new Vector2Int(x, item.y + j);
                                    result.Add(coord);
                                }
                            }
                        }
                        else
                        {
                            Debug.LogError(string.Format("数据不匹配，传入数据:{0},实际数据：{1}", item.ToString(), myData.ToString()));

                        }
                    }
                    else
                    {
                        Debug.LogError("在地图生成器中，找不到建筑信息，给定位置：" + item.x + "," + item.y);
                    }
                }
            }

            return result;
        }

    }

  
}
