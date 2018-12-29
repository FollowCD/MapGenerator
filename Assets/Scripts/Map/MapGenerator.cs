using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    /*  层级               —————— -> 围栏
     *                     —————— -> 汽车
     *                     —————— -> 绿化带
     *                     —————— -> 血迹
     *                     —————— -> 破坏痕迹
     *建筑血迹  <- ————     —————— -> 白线
     *  建筑    <- ————     —————— -> 街道
     *            ——————————————  -> 地面
     */

    #region 各个生成物体的父物体
    Transform GroundParent;
    Transform StreetParent;
    Transform BuildParent;
    Transform WhiteLineParent;
    Transform BrokenParent;
    Transform BuildBloodParent;
    Transform StreetBloodParent;
    Transform GreenBeltParent;
    Transform CarParent;
    Transform FenceParent;
    Transform OuterWallParent;
    Transform OuterWallCornerParent;
    MapConfig mapData;
    #endregion

    #region 各种预制体
    GameObject GroundPrefab;                                //地面
    List<GameObject> StreetPrefabs;                         //街道
    List<MapBuildingContoller> BuildingPrefabs;             //建筑
    List<GameObject> GreenBeltPreafabs;                     //绿化带预制体，只放置在宽的街道中
    List<GameObject> LongFencePrefabs;                      //长围墙
    List<GameObject> ShortFencePrefabs;                     //短围墙
    List<GameObject> OuterWallPrefabs;                      //外围墙
    List<GameObject> OuterCornerWallPrefabs;                //外墙角围墙
    List<GameObject> BloodstainBuildingPrefabs;             //建筑血迹
    List<GameObject> BloodstainStreetPrefabs;               //街道血迹
    List<GameObject> CarsPrefabs;                           //汽车装饰
    List<GameObject> WhiteLinePrefabs;                      //白线
    List<GameObject> StreetBrokenPrefabs;                   //街道破坏痕迹
    #endregion

    #region 创建物体的容器
    [HideInInspector]
    public MapBuildingContoller[,] buildingArray;
    [HideInInspector]
    public List<MapBuildingContoller> rigidPositionBuildingList;
    public Dictionary<int, List<GameObject>> xStreet;
    public Dictionary<int, List<GameObject>> yStreet;
    public Dictionary<int, List<GameObject>> xWideStreet;
    public Dictionary<int, List<GameObject>> yWideStreet;

    [HideInInspector]
    public List<GameObject> whiteLineList;
    [HideInInspector]
    public List<GameObject> greenBeltList;


    //横向的围栏集合 key-第几行，value-第几个obj
    public Dictionary<int, List<GameObject>> xFencesDict;

    //纵向的围栏集合
    public Dictionary<int, List<GameObject>> yFencesDict;

    [HideInInspector]
    public GameObject[,] buildingBloodArray;
    //key-obj的位置信息，value-obj实例
    public Dictionary<Vector3, GameObject> streetBloodDict = new Dictionary<Vector3, GameObject>();
    public Dictionary<Vector3, GameObject> streetCarDict = new Dictionary<Vector3, GameObject>();
    public Dictionary<Vector3, GameObject> streetBrokenDict = new Dictionary<Vector3, GameObject>();

    public Dictionary<Vector2Byte, MapBuildingFence> buildingFenceDict = new Dictionary<Vector2Byte, MapBuildingFence>();
    List<GameObject> outerWallList;
    #endregion

    #region 临时变量
    int remainTileCount;
    Vector2 groundLeftDownPos;
    #endregion 

    #region 基础工作
    public void InitGenerator(MapConfig data)
    {
        if (data == null)
        {
            Debug.LogError("配置信息为空！");
            return;
        }

        mapData = data;

        remainTileCount = mapData.MapWidth * mapData.MapHeight;
        CreateGameObjectContainer();
        LoadAllPrefabs();
        CreateDataContainer(mapData.MapWidth, mapData.MapHeight);
    }

    public void GeneratorMap()
    {
        CreateGround();
        CreateStreet();
        CreateBuildings();
        CreateWhiteLineWithBuildTiles();
        CreateGreenBelt();
        CreateCar();
        CreateFence();
        CreateBlood();
        CreateOutWallAndCorner();
    }

    void LoadAllPrefabs()
    {
        //TODO:加载预制体，并且保存到对应变量中
        GroundPrefab = Resources.Load<GameObject>("ground");
        StreetPrefabs = new List<GameObject>();

        StreetPrefabs.Add(Resources.Load<GameObject>("street_black"));
        //StreetPrefabs.Add(Resources.Load<GameObject>("street_gray"));

        BuildingPrefabs = new List<MapBuildingContoller>();

        BuildingPrefabs.Add(Resources.Load<MapBuildingContoller>("building_1x1"));
        BuildingPrefabs.Add(Resources.Load<MapBuildingContoller>("building_1x2"));
        BuildingPrefabs.Add(Resources.Load<MapBuildingContoller>("building_2x1"));
        BuildingPrefabs.Add(Resources.Load<MapBuildingContoller>("building_2x2"));
        BuildingPrefabs.Add(Resources.Load<MapBuildingContoller>("building_1x1_p"));

        WhiteLinePrefabs = new List<GameObject>();
        WhiteLinePrefabs.Add(Resources.Load<GameObject>("whiteLine"));

        GreenBeltPreafabs = new List<GameObject>();
        GreenBeltPreafabs.Add(Resources.Load<GameObject>("greenBelt"));

        LongFencePrefabs = new List<GameObject>();
        LongFencePrefabs.Add(Resources.Load<GameObject>("longFence"));

        ShortFencePrefabs = new List<GameObject>();
        ShortFencePrefabs.Add(Resources.Load<GameObject>("shortFence"));

        BloodstainBuildingPrefabs = new List<GameObject>();
        BloodstainBuildingPrefabs.Add(Resources.Load<GameObject>("buildBlood"));

        BloodstainStreetPrefabs = new List<GameObject>();
        BloodstainStreetPrefabs.Add(Resources.Load<GameObject>("streetBlood"));

        CarsPrefabs = new List<GameObject>();
        CarsPrefabs.Add(Resources.Load<GameObject>("car"));

        OuterWallPrefabs = new List<GameObject>();
        OuterWallPrefabs.Add(Resources.Load<GameObject>("OuterWall"));

        OuterCornerWallPrefabs = new List<GameObject>();
        OuterCornerWallPrefabs.Add(Resources.Load<GameObject>("OuterWallCorner"));
    }

    void CreateGameObjectContainer()
    {
        GroundParent = CreateTransform("GroundParent", transform, 0);
        StreetParent = CreateTransform("StreetParent", transform, 1);
       
        WhiteLineParent = CreateTransform("WhiteLineParent", transform, 2);
        BrokenParent = CreateTransform("BrokenParent", transform, 3);
        StreetBloodParent = CreateTransform("StreetBloodParent", transform, 4);
        GreenBeltParent = CreateTransform("GreenBeltParent", transform, 5);
        CarParent = CreateTransform("CarParent", transform, 6);
        BuildParent = CreateTransform("BuildParent", transform, 7);
        BuildBloodParent = CreateTransform("BuildBloodParent", transform, 8);
        FenceParent = CreateTransform("FenceParent", transform, 9);
        OuterWallParent = CreateTransform("OuterWallParent", transform, 10);
        OuterWallCornerParent = CreateTransform("OuterWallCornerParent", transform, 11);
    }

    void CreateDataContainer(int mapW, int mapH)
    {
        buildingArray = new MapBuildingContoller[mapW, mapH];
        rigidPositionBuildingList = new List<MapBuildingContoller>();

        xStreet = new Dictionary<int, List<GameObject>>();
        yStreet = new Dictionary<int, List<GameObject>>();

        xWideStreet = new Dictionary<int, List<GameObject>>();
        yWideStreet = new Dictionary<int, List<GameObject>>();

        whiteLineList = new List<GameObject>();
        greenBeltList = new List<GameObject>();

        buildingBloodArray = new GameObject[mapW, mapH];

        streetBloodDict = new Dictionary<Vector3, GameObject>();
        streetCarDict = new Dictionary<Vector3, GameObject>();
        outerWallList = new List<GameObject>();
        buildingFenceDict = new Dictionary<Vector2Byte, MapBuildingFence>();
    }

    Transform CreateTransform(string transName, Transform parent, int childIndex)
    {
        var go = new GameObject(transName);
        go.transform.SetParent(parent);
        var rectTrans = go.AddComponent<RectTransform>();
        go.AddComponent<CanvasRenderer>();


        go.transform.localScale = Vector3.one;
        go.transform.localPosition = Vector3.zero;
        go.transform.eulerAngles = Vector3.zero;

        rectTrans.pivot = new Vector2(0.5f, 0.5f);
        rectTrans.anchorMin = new Vector2(0.5f, 0.5f);
        rectTrans.anchorMax = new Vector2(0.5f, 0.5f);
        rectTrans.sizeDelta = new Vector2(100, 100);


        childIndex = childIndex > 0 ? childIndex : 0;
        childIndex = childIndex < parent.childCount ? childIndex : parent.childCount - 1;

        go.transform.SetSiblingIndex(childIndex);

        return go.transform;
    }


    #endregion

    #region 创建地面
    void CreateGround()
    {
        if (mapData==null)
        {
            return;
        }

        int MapWidth = mapData.MapWidth;
        int MapHeight = mapData.MapHeight;
        Vector2 BuildingSize = mapData.BuildingSize;
        Vector2 StreetSize = mapData.StreetSize;
        GameObject GroundPrefab = this.GroundPrefab;


       var groundWidth = MapWidth * BuildingSize.x + (MapWidth + 3) * StreetSize.x;
        var groundHeight = MapHeight * BuildingSize.y + (MapHeight + 3) * StreetSize.y;

        var go = Instantiate(GroundPrefab);

        go.name = "Ground";
        go.transform.SetParent(GroundParent);
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = Vector3.zero;
        go.transform.eulerAngles = Vector3.zero;
        var rectTrans = go.GetComponent<RectTransform>();

        rectTrans.sizeDelta = new Vector2(groundWidth, groundHeight);

        groundLeftDownPos = new Vector2(-groundWidth / 2, -groundHeight / 2);
    }

    #endregion

    #region 创建建筑
    void CreateBuildings()
    {
        if (mapData == null)
        {
            Debug.LogError("配置数据为空");
            return;
        }

        //1.创建指定位置的建筑
        CreateRigidPositionBuildings(mapData.RigidPositionBuildingDict, mapData.BuildingSize, mapData.WideStreetPos);
        //2.创建配置数量的建筑
        CreateLayerBuildings(mapData.LayerBuidingDict, mapData.BuildingSize, mapData.WideStreetPos);
        //3.将还未填补满的地方填满建筑
        CreateRandomBuildings(mapData.BuildingSize);
    }

    void CreateRandomBuildings(Vector2 buildingSize)
    {
        if (remainTileCount <= 0)
        {
            return;
        }
        else
        {
            if (!BuildingPrefabs.IsNullOrEmpty())
            {
                BuildingPrefabs.Sort((x1, x2) =>
                {
                    if (x1.BuildingSizeCount > x2.BuildingSizeCount)
                    {
                        return -1;
                    }
                    else if (x1.BuildingSizeCount < x2.BuildingSizeCount)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                });

                var index = BuildingPrefabs.FindIndex(x => x.BuildingSizeCount == 1);

                if (index > -1)
                {
                    var coords = FindEmptyCoord();

                    var temp = remainTileCount;

                    for (int i = 0; i < temp; i++)
                    {
                        var prefabIndex = UnityEngine.Random.Range(index, BuildingPrefabs.Count);

                        var prefab = BuildingPrefabs[prefabIndex];

                        if (!coords.IsNullOrEmpty())
                        {
                            var coord = coords[coords.Count - 1];
                            var pos = GetBuildPos(coord);
                            var go = CreateBuildings(prefab, pos, buildingSize);
                           // go.MapArrIndex = coord;

                            go.BuildingRect.x = (byte)coord.x;
                            go.BuildingRect.y = (byte)coord.y;

                            SetBuildRef(go);
                            coords.Remove(coord);
                        }
                    }
                }
            }
        }
    }

    private List<Vector2Int> FindEmptyCoord()
    {
        var result = new List<Vector2Int>();
        for (int i = 0; i < buildingArray.GetLength(0); i++)
        {
            for (int j = 0; j < buildingArray.GetLength(1); j++)
            {
                var item = buildingArray[i, j];

                if (item == null)
                {
                    result.Add(new Vector2Int(i, j));
                }
            }
        }

        return result;

    }

    void CreateLayerBuildings(Dictionary<Vector4Int, Dictionary<string, int>> LayerBuidingDict, Vector2 buildingSize, Vector2Int streetPos)
    {
        var paramRight = CheckCount(LayerBuidingDict);

        if (!paramRight)
        {
            Debug.LogError("生成建筑失败，配置错误");
            return;
        }

        var keys = LayerBuidingDict.Keys;

        var list = new List<Vector4Int>();
        list.AddRange(keys);

        for (int i = 0; i < list.Count; i++)
        {
            var dict = LayerBuidingDict[list[i]];
            var thisLayerCount = list[i].Count;
            var lastLayer = i > 0 ? list[i - 1] : new Vector4Int(0, 0, 0, 0);
            var lastLyaerCount = i > 0 ? lastLayer.Count : 0;

            var realCount = thisLayerCount - lastLyaerCount;

            if (dict.Values.Count > realCount)
            {
                Debug.LogError(string.Format("无法生成层配置建筑，因为第{0}层最少需要{1}位置，实际位置{2}", i, dict.Values.Count, realCount));
                return;
            }
            else
            {
                var prefabList = FindMapControllerByDictKeys(dict.Keys);

                if (!prefabList.IsNullOrEmpty())
                {
                    prefabList.Sort((x1, x2) =>
                    {
                        if (x1.BuildingSizeCount > x2.BuildingSizeCount)
                        {
                            return -1;
                        }
                        else if (x1.BuildingSizeCount < x2.BuildingSizeCount)
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    });

                    CreateOneLayerBuildings(prefabList, dict, list[i], lastLayer, streetPos, buildingSize, i);

                }
                else
                {
                    Debug.LogError(string.Format("第{0}层没有获取到配置预制体信息", i));
                    return;
                }

            }
        }
    }

    void CreateOneLayerBuildings(List<MapBuildingContoller> prefabList, Dictionary<string, int> dict, Vector4Int crtLayerRange, Vector4Int lastLayerRange, Vector2Int streetPos, Vector2 buildingSize, int crtLayerIndex)
    {
        if (crtLayerRange.IsCover(lastLayerRange) || crtLayerIndex == 0)
        {
            for (int i = 0; i < prefabList.Count; i++)
            {
                var item = prefabList[i];

                int itemNeedCount;
                dict.TryGetValue(item.name, out itemNeedCount);

                if (itemNeedCount == 0)
                {
                    Debug.LogError("出错，在配置中，建筑：" + item.name + "的生成数量为0");
                    return;
                }

                var emptyArea = FindLayerEmtpyArea(new Vector2Int(item.BuildingRect.w,item.BuildingRect.h) , crtLayerRange, lastLayerRange, streetPos, crtLayerIndex);

                if (emptyArea == null)
                {
                    Debug.LogError("找不到任何可以容纳建筑的区域，建筑尺寸：" + new Vector2Int(item.BuildingRect.w,item.BuildingRect.h) + "建筑名称：" + item.name);
                    return;
                }

                if (emptyArea.AreaCount < itemNeedCount)
                {
                    Debug.LogError(string.Format("需求生成信息：{0}—{1}，当前容器数量：{2},当前圈层：{3}，前一圈层：{4}", item.name, itemNeedCount, emptyArea.AreaCount, crtLayerRange.ToString(), lastLayerRange.ToString()));
                    return;
                }

                for (int j = 0; j < itemNeedCount; j++)
                {
                    var tempArea = emptyArea.Dequeue();

                    if (!tempArea.IsNullOrEmpty())
                    {
                        var centerPos = GetCenterPos(tempArea[0], new Vector2Int(item.BuildingRect.w, item.BuildingRect.h));
                        var go = CreateBuildings(item, centerPos, buildingSize);
                       // go.MapArrIndex = tempArea[0];

                        go.BuildingRect.x = (byte)tempArea[0].x;
                        go.BuildingRect.y = (byte)tempArea[0].y;
                        SetBuildRef(go);
                    }
                }

            }
        }
        else
        {
            Debug.LogError(string.Format("两层级之间有冲突，外层未完全覆盖内层,外层：{0}，内层：{1}", crtLayerRange.ToString(), lastLayerRange.ToString()));
        }
    }

    private EmptyArea FindLayerEmtpyArea(Vector2Int buildingSize, Vector4Int crtLayerRange, Vector4Int lastLayerRange, Vector2Int wideStreetPos, int CrtLayerIndex)
    {
        var buildCount = buildingSize.x * buildingSize.y;
        var disXmin = Mathf.Abs(crtLayerRange.Xmin - lastLayerRange.Xmin);
        var disXmain = Mathf.Abs(crtLayerRange.Xmax - lastLayerRange.Xmax);
        var disYmin = Mathf.Abs(crtLayerRange.Ymin - lastLayerRange.Ymin);
        var disYmax = Mathf.Abs(crtLayerRange.Ymax - lastLayerRange.Ymax);

        if (CrtLayerIndex == 0)
        {
            disXmin = crtLayerRange.Xmax - crtLayerRange.Xmin + 1;
            disXmain = disXmin;

            disYmin = crtLayerRange.Ymax - crtLayerRange.Ymin + 1;
            disYmax = disYmin;
        }

        var disX = Mathf.Max(disXmin, disXmain);
        var disY = Mathf.Max(disYmin, disYmax);

        if (buildingSize.x > disX && buildingSize.y > disY)
        {
            Debug.LogError(string.Format("该层级最大容纳尺寸({0},{1}),无法容纳建筑尺寸：{2}", disX, disY, buildingSize.ToString()));
            return null;
        }
        else
        {
            var wideStreetInXmidlle = crtLayerRange.Xmin < wideStreetPos.x && crtLayerRange.Xmax >= wideStreetPos.x;
            var wideStreetInYmiddle = crtLayerRange.Ymin < wideStreetPos.y && crtLayerRange.Ymax >= wideStreetPos.y;

            EmptyArea result;

            if (wideStreetInXmidlle && wideStreetInYmiddle)//划分为4块
            {
                EmptyArea leftDown, rightDown, leftUp, rightUp;

                if (CrtLayerIndex == 0)
                {
                    leftDown = GetEmptyArea(new Vector4Int(crtLayerRange.Xmin, wideStreetPos.x - 1, crtLayerRange.Ymin, wideStreetPos.y - 1), buildingSize);
                    rightDown = GetEmptyArea(new Vector4Int(wideStreetPos.x, crtLayerRange.Xmax, crtLayerRange.Ymin, wideStreetPos.y - 1), buildingSize);
                    leftUp = GetEmptyArea(new Vector4Int(crtLayerRange.Xmin, wideStreetPos.x - 1, wideStreetPos.y, crtLayerRange.Ymax), buildingSize);
                    rightUp = GetEmptyArea(new Vector4Int(wideStreetPos.x, crtLayerRange.Xmax, wideStreetPos.y, crtLayerRange.Ymax), buildingSize);
                }
                else
                {
                    leftDown = GetEmptyArea(new Vector4Int(crtLayerRange.Xmin, wideStreetPos.x - 1, crtLayerRange.Ymin, wideStreetPos.y - 1),
                       lastLayerRange, buildingSize);

                    rightDown = GetEmptyArea(new Vector4Int(wideStreetPos.x, crtLayerRange.Xmax, crtLayerRange.Ymin, wideStreetPos.y - 1),
                       lastLayerRange, buildingSize);

                    leftUp = GetEmptyArea(new Vector4Int(crtLayerRange.Xmin, wideStreetPos.x - 1, wideStreetPos.y, crtLayerRange.Ymax),
                        lastLayerRange, buildingSize);

                    rightUp = GetEmptyArea(new Vector4Int(wideStreetPos.x, crtLayerRange.Xmax, wideStreetPos.y, crtLayerRange.Ymax),
                        lastLayerRange, buildingSize);
                }

                result = EmptyArea.Merge(buildCount, leftDown, rightDown, leftUp, rightUp);

            }
            else if (wideStreetInXmidlle && !wideStreetInYmiddle)//划分为左右两块
            {
                EmptyArea left, right;

                if (CrtLayerIndex == 0)
                {
                    left = GetEmptyArea(new Vector4Int(crtLayerRange.Xmin, wideStreetPos.x - 1, crtLayerRange.Ymin, crtLayerRange.Ymax), buildingSize);
                    right = GetEmptyArea(new Vector4Int(wideStreetPos.x, crtLayerRange.Xmax, crtLayerRange.Ymin, crtLayerRange.Ymax), buildingSize);
                }
                else
                {
                    left = GetEmptyArea(new Vector4Int(crtLayerRange.Xmin, wideStreetPos.x - 1, crtLayerRange.Ymin, crtLayerRange.Ymax),
                        lastLayerRange, buildingSize);

                    right = GetEmptyArea(new Vector4Int(wideStreetPos.x, crtLayerRange.Xmax, crtLayerRange.Ymin, crtLayerRange.Ymax),
                        lastLayerRange, buildingSize);
                }


                result = EmptyArea.Merge(buildCount, left, right);
            }
            else if (!wideStreetInXmidlle && wideStreetInYmiddle)//划分为上下两块
            {
                EmptyArea up, down;

                if (CrtLayerIndex == 0)
                {
                    down = GetEmptyArea(new Vector4Int(crtLayerRange.Xmin, crtLayerRange.Xmax, crtLayerRange.Ymin, wideStreetPos.y - 1), buildingSize);
                    up = GetEmptyArea(new Vector4Int(crtLayerRange.Xmin, crtLayerRange.Xmax, wideStreetPos.y, crtLayerRange.Ymax), buildingSize);
                }
                else
                {
                    down = GetEmptyArea(new Vector4Int(crtLayerRange.Xmin, crtLayerRange.Xmax, crtLayerRange.Ymin, wideStreetPos.y - 1),
                            lastLayerRange, buildingSize);

                    up = GetEmptyArea(new Vector4Int(crtLayerRange.Xmin, crtLayerRange.Xmax, wideStreetPos.y, crtLayerRange.Ymax),
                            lastLayerRange, buildingSize);
                }


                result = EmptyArea.Merge(buildCount, down, up);
            }
            else//整体一块
            {
                if (CrtLayerIndex == 0)
                {
                    result = GetEmptyArea(crtLayerRange, buildingSize);
                }
                else
                {
                    result = GetEmptyArea(crtLayerRange, lastLayerRange, buildingSize);

                }
            }

            return result;
        }
    }

    EmptyArea GetEmptyArea(Vector4Int crtLayerRange, Vector2Int buildingSize)
    {
        EmptyArea emptyArea = new EmptyArea();
        for (int i = crtLayerRange.Xmin; i <= crtLayerRange.Xmax; i++)
        {
            for (int j = crtLayerRange.Ymin; j <= crtLayerRange.Ymax; j++)
            {

                List<Vector2Int> result;
                var areaIsEmpty = CheckAreaIsEmpty(i, j, buildingSize, crtLayerRange, out result);
                if (areaIsEmpty)
                {
                    emptyArea.Enqueue(result);
                }


            }
        }

        return emptyArea;
    }

    EmptyArea GetEmptyArea(Vector4Int outerLayerRange, Vector4Int lastLayerRange, Vector2Int buildingSize)
    {
        EmptyArea emptyArea = new EmptyArea();



        for (int i = outerLayerRange.Xmin; i <= outerLayerRange.Xmax; i++)
        {
            for (int j = outerLayerRange.Ymin; j <= outerLayerRange.Ymax; j++)
            {
                if (CheckIndexInVectorInt4Range(i, j, lastLayerRange))
                {
                    continue;
                }
                else
                {
                    List<Vector2Int> result;
                    var areaIsEmpty = CheckAreaIsEmpty(i, j, buildingSize, outerLayerRange, lastLayerRange, out result);
                    if (areaIsEmpty)
                    {
                        emptyArea.Enqueue(result);
                    }
                }

            }
        }

        return emptyArea;
    }

    bool CheckAreaIsEmpty(int x, int y, Vector2Int size, Vector4Int outerBorder, Vector4Int innerBorder, out List<Vector2Int> result)
    {
        int emptyCount = 0;
        Vector2Int pos = new Vector2Int(x, y);
        result = new List<Vector2Int>();
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                pos.x += i;
                pos.y += j;

                var posInOuterRange = CheckIndexInVectorInt4Range(pos.x, pos.y, outerBorder);
                var posInInnerRange = CheckIndexInVectorInt4Range(pos.x, pos.y, innerBorder);

                if (posInOuterRange && !posInInnerRange)
                {
                    var item = buildingArray.GetData(pos);

                    if (item == null)
                    {
                        emptyCount++;
                    }
                    else
                    {
                        return false;
                    }

                    result.Add(pos);
                }
                else
                {
                    return false;
                }

            }
        }

        return emptyCount == size.x * size.y;
    }
    bool CheckAreaIsEmpty(int x, int y, Vector2Int size, Vector4Int outerBorder, out List<Vector2Int> result)
    {
        int emptyCount = 0;
        Vector2Int pos = new Vector2Int(x, y);
        result = new List<Vector2Int>();

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                pos.x += i;
                pos.y += j;

                var posInOuterRange = CheckIndexInVectorInt4Range(pos.x, pos.y, outerBorder);

                if (posInOuterRange)
                {
                    var item = buildingArray.GetData(pos);

                    if (item == null)
                    {
                        emptyCount++;
                    }

                    result.Add(pos);
                }

            }
        }

        return emptyCount == size.x * size.y;
    }

    private bool CheckIndexInVectorInt4Range(int x, int y, Vector4Int layerRange)
    {
        return x >= layerRange.Xmin && x <= layerRange.Xmax && y >= layerRange.Ymin && y <= layerRange.Ymax;
    }

    private List<MapBuildingContoller> FindMapControllerByDictKeys(ICollection<string> keys)
    {
        var nameKey = keys;
        var prefabList = new List<MapBuildingContoller>();

        foreach (var item in nameKey)
        {
            var mapController = GetBuildPrefab(item);
            prefabList.Add(mapController);
        }

        return prefabList;
    }

    private bool CheckCount(Dictionary<Vector4Int, Dictionary<string, int>> layerBuidingDict)
    {
        if (layerBuidingDict != null && layerBuidingDict.Count > 0)
        {
            var keys = layerBuidingDict.Keys;
            var list = new List<Vector4Int>();
            list.AddRange(keys);

            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                var itemLast = i > 0 ? list[i - 1] : new Vector4Int(0, 0, 0, 0);
                var itemLastCount = i > 0 ? itemLast.Count : 0;
                if (item.Xmin >= 0 && item.Xmax < mapData.MapWidth && item.Ymin >= 0 && item.Ymax < mapData.MapHeight)
                {
                    var layerCount = item.Count - itemLastCount;
                    var needCount = layerBuidingDict[item].Values.Sum();

                    if (needCount > layerCount)
                    {
                        Debug.LogError(string.Format("出错啦，第{0}层需求空位数量：{1}，实际空位数量：{2}", i, needCount, layerCount));
                        return false;
                    }
                }
            }

            return true;

        }
        else
        {
            Debug.LogError("配置数据为空");
            return false;
        }
    }

    void CreateRigidPositionBuildings(Dictionary<Vector2Int, string> buildingDict, Vector2 buildingSize, Vector2Int wideStreetPos)
    {
        if (buildingDict == null || buildingDict.Count < 1)
        {
            return;
        }

        var keys = buildingDict.Keys;

        foreach (var item in keys)
        {
            var build = buildingArray.GetData(item);

            if (build != null)
            {
                Debug.LogError(string.Format("在位置{0}上已经存在建筑：{1}", item.ToString(), build.name));
                return;
            }
            else
            {
                var buildPrefabName = buildingDict[item];
                var buildPrefab = GetBuildPrefab(buildPrefabName);

                if (buildPrefab == null)
                {
                    Debug.LogError("找不到名为：" + buildPrefabName + "的建筑预制体，请检查");
                    return;
                }
                else if (IsBuildingCrossWideStreet(new Vector2Int(buildPrefab.BuildingRect.w, buildPrefab.BuildingRect.h), item, wideStreetPos))
                {
                    Debug.LogError(string.Format("配置错误，固定建筑物横跨宽街道,建筑位置：{0}，建筑名称：{1}", item.ToString(), buildPrefab.name));
                    return;
                }
                else
                {
                    var pos = GetCenterPos(item, new Vector2Int(buildPrefab.BuildingRect.w, buildPrefab.BuildingRect.h));
                    var clone = CreateBuildings(buildPrefab, pos, buildingSize);
                    //clone.MapArrIndex = item;
                    clone.BuildingRect.x = (byte)item.x;
                    clone.BuildingRect.y = (byte)item.y;

                    SetBuildRef(clone);
                    rigidPositionBuildingList.Add(clone);
                }
            }


        }
    }

    private bool IsBuildingCrossWideStreet(Vector2Int buildingSize, Vector2Int item, Vector2Int wideStreetPos)
    {
        var atLeft = item.x < wideStreetPos.x;
        var atDown = item.y < wideStreetPos.y;

        if (atLeft)
        {
            return (item.x + buildingSize.x - 1) >= wideStreetPos.x;
        }

        if (atDown)
        {
            return (item.y + buildingSize.y - 1) >= wideStreetPos.y;
        }

        return false;
    }

    private void SetBuildRef(MapBuildingContoller clone)
    {
        if (clone != null)
        {
            //var startPos = clone.MapArrIndex;
            var startPos = new Vector2Int(clone.BuildingRect.x, clone.BuildingRect.y);
            for (int i = 0; i < clone.BuildingRect.w; i++)
            {
                for (int j = 0; j < clone.BuildingRect.h; j++)
                {
                    var pos = new Vector2Int(startPos.x + i, startPos.y + j);
                    if (IndexValid(pos) && buildingArray.GetData(pos) == null)
                    {
                        buildingArray[pos.x, pos.y] = clone;
                        remainTileCount--;
                    }
                    else
                    {
                        Debug.LogError("无法设置数组引用，因为数组越界或者该处已有数据,数组越界：" + IndexValid(pos));
                        return;
                    }
                }
            }
        }
        else
        {
            Debug.LogError("建筑为空，无法设置地图引用，因为克隆体为空");
        }
    }

    private MapBuildingContoller GetBuildPrefab(string buildPrefabName)
    {
        if (BuildingPrefabs != null && BuildingPrefabs.Count > 0)
        {
            return BuildingPrefabs.Find(x => x.name == buildPrefabName);
        }
        else
        {
            Debug.LogError("未初始化建筑预制体信息");
            return null;
        }
    }

    private MapBuildingContoller CreateBuildings(MapBuildingContoller prefab, Vector3 localPos, Vector2 buildingSize)
    {
        var go = Instantiate(prefab.gameObject);
        var sp = go.GetComponent<MapBuildingContoller>();
        go.transform.SetParent(BuildParent);
        go.transform.eulerAngles = Vector3.zero;
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = localPos;

        var rectTran = go.GetComponent<RectTransform>();
        rectTran.sizeDelta = buildingSize * new Vector2Int(sp.BuildingRect.w,sp.BuildingRect.h);
        return sp;
    }

    public Vector2 GetBuildPos(Vector2Int index)
    {
        if (mapData != null /*&& IndexValid(index)*/)
        {
            float x = (index.x + 0.5f) * mapData.BuildingSize.x + (index.x + 1) * mapData.StreetSize.x;
            float y = (index.y + 0.5f) * mapData.BuildingSize.y + (index.y + 1) * mapData.StreetSize.y;

            if (index.x >= mapData.WideStreetPos.x)
            {
                x += (mapData.WideStreetWidth - 1) * mapData.StreetSize.x;

            }

            if (index.y >= mapData.WideStreetPos.y)
            {
                y += (mapData.WideStreetWidth - 1) * mapData.StreetSize.y;
            }

            x += groundLeftDownPos.x;
            y += groundLeftDownPos.y;

            return new Vector2(x, y);
        }
        else
        {
            Debug.LogError("无法获取坐标，mapData为空或者index越界");
            return Vector2.zero;
        }
    }

    private Vector2 GetCenterPos(Vector2Int startPos, Vector2Int size)
    {
        return GetCenterPos(startPos, size.x, size.y);
    }

    private Vector2 GetCenterPos(Vector2Int startPos, int width, int height)
    {
        Vector2 result = Vector2.zero;
        var count = width * height;
        var start = GetBuildPos(startPos);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var pos = new Vector2Int(startPos.x + i, startPos.y + j);
                if (IndexValid(pos))
                {
                    result += (GetBuildPos(pos) - start);
                }
            }
        }

        result.x = count > 0 ? result.x / count : result.x;
        result.y = count > 0 ? result.y / count : result.y;

        return result + start;
    }
    #endregion

    #region 创建街道
    void CreateStreet()
    {
        var mapWidth = mapData.BuildingSize.x * mapData.MapWidth + mapData.StreetSize.x * (mapData.MapWidth + mapData.WideStreetWidth);
        var mapHeight = mapData.BuildingSize.y * mapData.MapHeight + mapData.StreetSize.y * (mapData.MapHeight + mapData.WideStreetWidth);

        var xCount = Mathf.FloorToInt(mapWidth / (mapData.StreetSize.x + mapData.StreetGap)) + 1;
        var yCount = Mathf.FloorToInt(mapHeight / (mapData.StreetSize.x + mapData.StreetGap)) + 1;

        //TODO:开始创建街道

        for (int i = 0; i <= mapData.MapHeight; i++)//创建横向街道
        {
            var list = new List<GameObject>();
            for (int j = 0; j < xCount; j++)
            {
                var pos = GetXStreetUnitPos(i, j);
                var go = CreateStreet(pos);
                go.name = "横向：" + i + "_" + j;
                if (go != null)
                {
                    list.Add(go);
                }
            }

            xStreet.Add(i, list);
        }

        for (int i = 0; i <= mapData.MapWidth; i++)//创建纵向街道
        {
            var list = new List<GameObject>();
            for (int j = 0; j < yCount; j++)
            {
                var pos = GetYStreetUnitPos(i, j);
                var go = CreateStreet(pos);
                go.name = "纵向：" + i + "_" + j;
                if (go != null)
                {
                    list.Add(go);
                }
            }

            yStreet.Add(i, list);
        }


        var mid = mapData.WideStreetWidth / 2;

        for (int i = 0; i < mapData.WideStreetWidth; i++)
        {
            if (i == mid)
            {
                continue;
            }

            var offsetX = -mapData.StreetSize.x * (mid - i);
            var offsetY = -mapData.StreetSize.y * (mid - i);


            var xList = new List<GameObject>();
            var yList = new List<GameObject>();
            for (int j = 0; j < xCount; j++)//创建横向宽街道
            {
                var pos = GetXStreetUnitPos(mapData.WideStreetPos.y, j);
                pos.y += offsetY;

                var go = CreateStreet(pos);
                go.name = string.Format("横向 宽{0}_{1}", i, j);
                xList.Add(go);
            }

            for (int j = 0; j < yCount; j++)//创建纵向宽街道
            {
                var pos = GetYStreetUnitPos(mapData.WideStreetPos.x, j);
                pos.x += offsetX;

                var go = CreateStreet(pos);
                go.name = string.Format("纵向 宽{0}_{1}", i, j);

                yList.Add(go);
            }

            xWideStreet.Add(i, xList);
            yWideStreet.Add(i, yList);
        }

    }

    GameObject CreateStreet(Vector2 localPos)
    {
        var prefab = GetRandomPrefab(StreetPrefabs);

        if (prefab != null && mapData != null)
        {
            var go = Instantiate(prefab);
            go.transform.SetParent(StreetParent);
            go.transform.localScale = Vector3.one;
            go.transform.eulerAngles = Vector3.zero;

            var rectTrans = go.GetComponent<RectTransform>();

            rectTrans.sizeDelta = mapData.StreetSize;
            go.transform.localPosition = localPos;
            return go;
        }
        else
        {
            Debug.LogError("街区预制体或配置文件为空");
            return null;
        }
    }

    Vector2 GetXStreetUnitPos(int yIndex, int order)
    {
        if (mapData != null)
        {
            var y = (yIndex + 0.5f) * mapData.StreetSize.y + yIndex * mapData.BuildingSize.y;
            var x = (mapData.StreetSize.x + mapData.StreetGap + 0.5f) * order;

            if (yIndex == mapData.WideStreetPos.y)
            {
                y += (mapData.WideStreetWidth - 1) / 2 * mapData.StreetSize.y;
            }
            else if (yIndex > mapData.WideStreetPos.y)
            {
                y += (mapData.WideStreetWidth - 1) * mapData.StreetSize.y;
            }

            y += groundLeftDownPos.y;
            x += groundLeftDownPos.x;

            return new Vector2(x, y);
        }
        else
        {
            Debug.LogError("无法计算，因为配置数据为null");
            return Vector2.zero;
        }
    }

    Vector2 GetYStreetUnitPos(int xIndex, int order)
    {
        if (mapData != null)
        {
            var x = (xIndex + 0.5f) * mapData.StreetSize.x + xIndex * mapData.BuildingSize.x;
            var y = (mapData.StreetSize.y + mapData.StreetGap + 0.5f) * order;

            if (xIndex == mapData.WideStreetPos.x)
            {
                x += (mapData.WideStreetWidth - 1) / 2 * mapData.StreetSize.x;
            }
            else if (xIndex > mapData.WideStreetPos.x)
            {
                x += (mapData.WideStreetWidth - 1) * mapData.StreetSize.x;
            }

            x += groundLeftDownPos.x;
            y += groundLeftDownPos.y;

            return new Vector2(x, y);
        }
        else
        {
            Debug.LogError("无法计算，因为配置数据为null");
            return Vector2.zero;
        }
    }
    #endregion

    #region 创建白线
    void CreateWhiteLineWithStreetTiles()
    {
        if (mapData==null)
        {
            return;
        }
        Vector2 streetSize = mapData.StreetSize;
        float WhiteLineWidth = mapData.WhiteLineWidth;
        Vector2Int wideStreetPos = mapData.WideStreetPos;

        if (xStreet.IsNullOrEmpty())
        {
            Debug.LogError("横向街道未生成");
            return;
        }
        else if (yStreet.IsNullOrEmpty())
        {
            Debug.LogError("纵向街道未生成");
            return;
        }
        else if (xWideStreet.IsNullOrEmpty())
        {
            Debug.LogError("横向宽街道未生成");
            return;
        }
        else if (yWideStreet.IsNullOrEmpty())
        {
            Debug.LogError("纵向宽街道未生成");
            return;
        }

        var whiteLineSize = new Vector2(streetSize.x, WhiteLineWidth);

        CreateWhiteLineWithStreetTile(xStreet, whiteLineSize, wideStreetPos.y, false);

        CreateWhiteLineWithStreetTile(yStreet, whiteLineSize, wideStreetPos.x, true);

        CreateWhiteLineWithStreetTile(xWideStreet, whiteLineSize, -1, false);

        CreateWhiteLineWithStreetTile(yWideStreet, whiteLineSize, -1, true);
    }

    void CreateWhiteLineWithBuildTiles()
    {
        if (mapData==null)
        {
            return;
        }

        Vector2 buildSize = mapData.BuildingSize;
        float WhiteLineWidth = mapData.WhiteLineWidth;
        Vector2Int wideStreetPos = mapData.WideStreetPos;

        if (buildingArray==null||buildingArray.Length<=0)
        {
            return;
        }
        else
        {
            var whiteLineSize = new Vector2(buildSize.x, WhiteLineWidth);

            CreataXWhiteLineWithBuildTiles(buildSize, whiteLineSize, wideStreetPos);

            CreateYWhiteLineWithBuildTiles(buildSize, whiteLineSize, wideStreetPos);
        }
    }

    private void CreataXWhiteLineWithBuildTiles(Vector2 buildSize, Vector2 whiteLineSize, Vector2Int wideStreetPos)
    {
        for (int i = 0; i < mapData.MapHeight; i++)
        {
            for (int j = 0; j < mapData.MapWidth; j++)
            {
                var pos = GetBuildPos(new Vector2Int(j, i));

                pos.y -= (buildSize.y + mapData.StreetSize.y) / 2;

                CreateWhiteLine(pos, whiteLineSize, Vector3.zero, whiteLineList);
            }
        }

        if (wideStreetPos.y>0 && wideStreetPos.y<mapData.MapHeight)//补上横向宽街道的下边缘
        {
            for (int i = 0; i < mapData.MapWidth; i++)
            {
                var pos = GetBuildPos(new Vector2Int(i, wideStreetPos.y - 1));

                pos.y += (buildSize.y + mapData.StreetSize.y) / 2;
                CreateWhiteLine(pos, whiteLineSize, Vector3.zero, whiteLineList);
            }
        }

        for (int i = 0; i < mapData.MapWidth; i++)//补上最上一层
        {
            var pos = GetBuildPos(new Vector2Int(i, mapData.MapHeight - 1));
            pos.y += (buildSize.y + mapData.StreetSize.y) / 2;
            CreateWhiteLine(pos, whiteLineSize, Vector3.zero, whiteLineList);
        }

    }

    private void CreateYWhiteLineWithBuildTiles(Vector2 buildSize,Vector2 whitelineSize,Vector2Int wideStreetPos)
    {
        var rotation = new Vector3(0, 0, 90);
        for (int i = 0; i < mapData.MapWidth; i++)
        {
            for (int j = 0; j < mapData.MapHeight; j++)
            {
                var pos = GetBuildPos(new Vector2Int( i, j));

                pos.x -= (buildSize.x + mapData.StreetSize.x) / 2;
                var go = CreateWhiteLine(pos, whitelineSize, rotation, whiteLineList);
                go.name = "竖_" + i + "_" + j;
            }
        }

        if (wideStreetPos.x>0 && wideStreetPos.x<mapData.MapWidth)
        {
            for (int i = 0; i < mapData.MapHeight; i++)
            {
                var pos = GetBuildPos(new Vector2Int(wideStreetPos.x - 1, i));

                pos.x+= (buildSize.x + mapData.StreetSize.x) / 2;
                var go = CreateWhiteLine(pos, whitelineSize, rotation, whiteLineList);
                go.name = "kuan竖_" + (wideStreetPos.x - 1) + "_" + i;
            }
        }

        for (int i = 0; i < mapData.MapHeight; i++)
        {
            var pos = GetBuildPos(new Vector2Int(mapData.MapWidth-1, i));

            pos.x += (buildSize.x + mapData.StreetSize.x) / 2;
            var go = CreateWhiteLine(pos, whitelineSize, rotation, whiteLineList);
            go.name = "Top_" + i;
        }
    }

    private void CreateWhiteLineWithStreetTile(Dictionary<int, List<GameObject>> streetDict, Vector2 whiteLineSize, int skipIndex, bool needRotation)
    {
        var rotation = Vector3.zero;
        rotation.z = needRotation ? 90 : 0;

        var keys = streetDict.Keys;

        if (!keys.IsNullOrEmpty())
        {
            foreach (var item in keys)
            {
                if (skipIndex == item)
                {
                    Debug.Log("跳过Index：" + skipIndex);
                    continue;
                }

                var streetList = streetDict[item];

                if (!streetList.IsNullOrEmpty())
                {
                    foreach (var street in streetList)
                    {
                        var pos = street.transform.localPosition;
                        CreateWhiteLine(pos, whiteLineSize, rotation, whiteLineList);
                    }
                }
            }
        }
    }

    private GameObject CreateWhiteLine(Vector3 pos, Vector2 whiteLineSize, Vector3 rotation, ICollection<GameObject> list)
    {
        var prefab = GetRandomPrefab(WhiteLinePrefabs);

        if (prefab != null)
        {
            var go = Instantiate(prefab);

            go.transform.SetParent(WhiteLineParent);

            go.transform.localScale = Vector3.one;

            go.transform.eulerAngles = rotation;

            go.transform.localPosition = pos;

            var rectTrans = go.GetComponent<RectTransform>();

            rectTrans.sizeDelta = whiteLineSize;

            if (list != null)
            {
                list.Add(go);
            }
            else
            {
                Debug.LogError("出错，list为空");
            }

            return go;
        }
        else
        {
            Debug.LogError("无法获取白线预制体");
            return null;
        }
    }



    #endregion

    #region 创建绿化带
    void CreateGreenBelt()
    {
        if (mapData==null)
        {
            return;
        }

        var greenBeltSize = mapData.GreenBeltSize;
        var width = mapData.MapWidth;
        var height = mapData.MapHeight;

        for (int i = 0; i < width; i++)
        {
            var pos = GetXGreenBeltPos(i);
            var go = CreateGreenBelt(pos, false, greenBeltSize);

            go.name = "竖_" + i;
        }

        for (int i = 0; i < height; i++)
        {
            var pos = GetYGreenBeltPos(i);
            var go = CreateGreenBelt(pos, true, greenBeltSize);

            go.name = "横_" + i;
        }
    }

    private GameObject CreateGreenBelt(Vector3 pos, bool needRotation, Vector2 greenBeltSize)
    {
        var rotation = Vector3.zero;
        rotation.z = needRotation ? 90 : 0;

        var prefab = GetRandomPrefab(GreenBeltPreafabs);

        if (prefab)
        {
            var go = Instantiate(prefab);

            go.transform.SetParent(GreenBeltParent);

            go.transform.localScale = Vector3.one;
            go.transform.eulerAngles = rotation;
            go.transform.localPosition = pos;

            var rectTran = go.GetComponent<RectTransform>();

            rectTran.sizeDelta = greenBeltSize;

            greenBeltList.Add(go);

            return go;
        }
        else
        {
            Debug.LogError("找不到绿化带预制体");

            return null;
        }

    }

    /// <summary>
    /// 获取横着的宽街道上的绿化带位置
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    Vector3 GetXGreenBeltPos(int index)
    {

        if (mapData != null && index >= 0 && index < mapData.MapHeight)
        {
            var pos = GetBuildPos(new Vector2Int( index, mapData.WideStreetPos.y));
            pos.y -= ((mapData.WideStreetWidth / 2f) * mapData.StreetSize.y+mapData.BuildingSize.y/2);
            return pos;
        }
        else
        {
            Debug.LogError("参数错误，无法计算绿化带位置");

            return Vector3.zero;
        }
    }

    Vector3 GetYGreenBeltPos(int index)
    {
        if (mapData != null && index >= 0 && index < mapData.MapWidth)
        {
            var pos = GetBuildPos(new Vector2Int(mapData.WideStreetPos.x,index));
            pos.x -= ((mapData.WideStreetWidth / 2f) * mapData.StreetSize.x+mapData.BuildingSize.x/2);
            return pos;
        }
        else
        {
            Debug.LogError("参数错误，无法计算绿化带位置");

            return Vector3.zero;
        }
    }


    #endregion

    #region 创建围栏
    void CreateFence()
    {
        if (mapData==null)
        {
            return;
        }


        var yHasNarrowFence = mapData.WideStreetPos.y > 0 && mapData.WideStreetPos.y < mapData.MapHeight;


        var xHasNarrowFence = mapData.WideStreetPos.x > 0 && mapData.WideStreetPos.y < mapData.MapWidth;

        CreateXFenceDict(xHasNarrowFence,yHasNarrowFence,false);
        CreateYFenceDict(xHasNarrowFence,yHasNarrowFence,false);

        FillBuildingFenceDict();
    }

    private void FillBuildingFenceDict()
    {
        for (int i = 0; i < mapData.MapWidth; i++)//宽
        {
            for (int j = 0; j < mapData.MapHeight; j++)//高
            {
                FindAndFillFence(i, j);
            }
        }
        Debug.Log("初始化围栏数据完毕");
    }

    private void FindAndFillFence(int index_x, int index_y)
    {
        //查找顺序 下 -左 -上 -右
        var arr = new GameObject[4];
        arr[0] = FindDownFence(index_x, index_y);
        arr[1] = FindLeftFence(index_x, index_y);
        arr[2] = FindUpFence(index_x, index_y);
        arr[3] = FindRightFence(index_x, index_y);
        var coord = new Vector2Byte(index_x, index_y);
        buildingFenceDict.Add(coord, new MapBuildingFence(coord, arr));
    }

    private GameObject FindRightFence(int index_x, int index_y)
    {
        if (mapData.WideStreetPos.x>0 && mapData.WideStreetPos.x-1==index_x)
        {
            return yFencesDict[mapData.MapWidth + 1][index_y];
        }
        else
        {
            return yFencesDict[index_x + 1][index_y];
        }
    }

    private GameObject FindUpFence(int index_x, int index_y)
    {
        if (mapData.WideStreetPos.y>0 && mapData.WideStreetPos.y-1==index_y)
        {
            return xFencesDict[mapData.MapHeight + 1][index_x];
        }
        else
        {
            return xFencesDict[index_y + 1][index_x];
        }
    }

    private GameObject FindLeftFence(int index_x, int index_y)
    {
        return yFencesDict[index_x][index_y];
    }

    private GameObject FindDownFence(int index_x, int index_y)
    {
        return xFencesDict[index_y][index_x];
    }

    void CreateXFenceDict(bool xHasNarrowFence,bool yHasNarrowFence,bool show)
    {
        if (xFencesDict==null)
        {
            xFencesDict = new Dictionary<int, List<GameObject>>();
        }

        for (int i = 0; i < mapData.MapHeight+1; i++)//遍历一行
        {
            var list = new List<GameObject>();
            for (int j = 0; j < mapData.MapWidth; j++)//遍历一行中的每个元素
            {
                var pos = GetBuildPos(new Vector2Int(j, i));
                pos.y -= (mapData.BuildingSize.y + mapData.StreetSize.y) / 2;
                var prefab = GetRandomPrefab(LongFencePrefabs);
                var go = CreateFence(prefab, pos, mapData.LongFenceSize, false);

                go.name = "横向_" + i + "_" + j;
                go.SetActive(show);
                list.Add(go);
            }

            if (yHasNarrowFence)
            {
                var pos = GetBuildPos(new Vector2Int( mapData.WideStreetPos.x - 1, i));
                pos.y-= (mapData.BuildingSize.y + mapData.StreetSize.y) / 2;
                pos.x += (mapData.BuildingSize.x / 2 + mapData.StreetSize.x * mapData.WideStreetWidth / 2f);

                var prefab = GetRandomPrefab(ShortFencePrefabs);
                var go = CreateFence(prefab, pos, mapData.ShortFenceSize, false);
                go.name = "横向窄_" + i;
                go.SetActive(show);
                list.Add(go);
            }

            xFencesDict.Add(i, list);
        }

        if (yHasNarrowFence)
        {
            var list = new List<GameObject>();
            for (int j = 0; j < mapData.MapWidth; j++)//遍历一行中的每个元素
            {
                var pos = GetBuildPos(new Vector2Int(j, mapData.WideStreetPos.y-1));
                pos.y += (mapData.BuildingSize.y + mapData.StreetSize.y) / 2;
                var prefab = GetRandomPrefab(LongFencePrefabs);
                var go = CreateFence(prefab, pos, mapData.LongFenceSize, false);

                go.name = "横向_宽街下_" + j;
                go.SetActive(show);

                list.Add(go);
            }

            {
                var pos = GetBuildPos(new Vector2Int(mapData.WideStreetPos.x - 1, mapData.WideStreetPos.y - 1));
                pos.y += (mapData.BuildingSize.y + mapData.StreetSize.y) / 2;
                pos.x += (mapData.BuildingSize.x / 2 + mapData.StreetSize.x * mapData.WideStreetWidth / 2f);

                var prefab = GetRandomPrefab(ShortFencePrefabs);
                var go = CreateFence(prefab, pos, mapData.ShortFenceSize, false);
                go.name = "横向窄_街下";
                go.SetActive(show);

                list.Add(go);
            }

            xFencesDict.Add(mapData.MapHeight + 1, list);
        }

    }

    void CreateYFenceDict(bool xHasNarrowFence, bool yHasNarrowFence,bool show)
    {
        if (yFencesDict == null)
        {
            yFencesDict = new Dictionary<int, List<GameObject>>();
        }

        for (int i = 0; i < mapData.MapWidth + 1; i++)//遍历一列
        {
            var list = new List<GameObject>();
            for (int j = 0; j < mapData.MapHeight; j++)//遍历一列中的每个元素
            {
                var pos = GetBuildPos(new Vector2Int(i, j));
                pos.x -= (mapData.BuildingSize.x + mapData.StreetSize.x) / 2;
                var prefab = GetRandomPrefab(LongFencePrefabs);
                var go = CreateFence(prefab, pos, mapData.LongFenceSize, true);
                go.name = string.Format("竖_{0}_{1}", i, j);
                go.SetActive(show);

                list.Add(go);
            }

            if (xHasNarrowFence)
            {
                var pos = GetBuildPos(new Vector2Int(i, mapData.WideStreetPos.y - 1));
                pos.x -= (mapData.BuildingSize.x + mapData.StreetSize.x) / 2;
                pos.y += (mapData.BuildingSize.y / 2 + mapData.WideStreetWidth / 2f * mapData.StreetSize.y);
                var prefab = GetRandomPrefab(ShortFencePrefabs);
                var go = CreateFence(prefab, pos, mapData.ShortFenceSize, true);
                go.name = string.Format("竖_宽_{0}", i);
                go.SetActive(show);

                list.Add(go);

            }

            yFencesDict.Add(i, list);
        }

        if (xHasNarrowFence)
        {
            var list = new List<GameObject>();
            for (int j = 0; j < mapData.MapHeight; j++)//遍历一行中的每个元素
            {
                var pos = GetBuildPos(new Vector2Int(mapData.WideStreetPos.x-1, j));
                pos.x += (mapData.BuildingSize.x + mapData.StreetSize.x) / 2;
                var prefab = GetRandomPrefab(LongFencePrefabs);
                var go = CreateFence(prefab, pos, mapData.LongFenceSize, true);
                go.name= "竖向_宽街下_" + j;
                go.SetActive(show);

                list.Add(go);
            }


            {
                var pos = GetBuildPos(new Vector2Int(mapData.WideStreetPos.x - 1, mapData.WideStreetPos.y - 1));
                pos.x += (mapData.BuildingSize.x + mapData.StreetSize.x) / 2;
                pos.y += (mapData.BuildingSize.y / 2 + mapData.WideStreetWidth / 2f * mapData.StreetSize.y);
                var prefab = GetRandomPrefab(ShortFencePrefabs);
                var go = CreateFence(prefab, pos, mapData.ShortFenceSize, true);
                go.name = string.Format("竖_宽_{0}",mapData.MapHeight+1);
                go.SetActive(show);

                list.Add(go);

            }

            yFencesDict.Add(mapData.MapHeight + 1, list);

        }
    }

    private GameObject CreateFence(GameObject prefab, Vector2 pos, Vector2 fenceSize, bool needRotation)
    {
        if (prefab)
        {
            var go = Instantiate(prefab);

            go.transform.SetParent(FenceParent);

            go.transform.eulerAngles = needRotation ? new Vector3(0, 0, 90) : Vector3.zero;

            go.transform.localScale = Vector3.one;

            go.transform.localPosition = pos;

            var rectTran = go.GetComponent<RectTransform>();

            rectTran.sizeDelta = fenceSize;

            return go;
        }
        else
        {
            Debug.LogError("预制体为null");
            return null;
        }
    }

    #endregion

    #region 创建血迹
    void CreateBlood()
    {
        if (mapData==null)
        {
            return;
        }
        CreateBuildingBlood();
        CreateStreetBlood();
    }

    void CreateBuildingBlood()
    {
        for (int i = 0; i < mapData.MapWidth; i++)
        {
            for (int j = 0; j < mapData.MapHeight; j++)
            {
                var prefab = GetRandomPrefab(BloodstainBuildingPrefabs);
                var pos = GetBuildPos(new Vector2Int(i, j));

                if (prefab)
                {
                    var go = Instantiate(prefab);
                    go.transform.SetParent(BuildBloodParent);
                    go.transform.localScale = Vector3.one;
                    go.transform.eulerAngles = Vector3.zero;
                    go.transform.localPosition = pos;

                    var rectTrans = go.GetComponent<RectTransform>();
                    rectTrans.sizeDelta = mapData.BuildingSize;
                    if (buildingBloodArray==null)
                    {
                        buildingBloodArray = new GameObject[mapData.MapWidth, mapData.MapHeight];
                    }

                    buildingBloodArray[i, j] = go;
                }
            }
        }
    }

    void CreateStreetBlood()
    {
        CreateStreetBlood(xStreet);
        CreateStreetBlood(yStreet);
        CreateStreetBlood(xWideStreet);
        CreateStreetBlood(yWideStreet);
    }

    private void CreateStreetBlood(Dictionary<int, List<GameObject>> dict)
    {
        CreateObj(dict, streetBloodDict, StreetBloodParent, BloodstainStreetPrefabs,Vector3.zero,Vector2.zero,-1,mapData.StreetBloodPercentage);
    }

    private void CreateObj(Dictionary<int, List<GameObject>> dict,Dictionary<Vector3,GameObject> target,Transform parent,List<GameObject> prefabList, Vector3 rotation,Vector2 size,int skipIndex=-1,float percentage=1.0f)
    {
        if (!dict.IsNullOrEmpty())
        {
            var keys = dict.Keys;

            foreach (var item in keys)
            {
                if (item==skipIndex)
                {
                    continue;
                }
                var list = dict[item];

                if (!list.IsNullOrEmpty())
                {
                    percentage = Mathf.Clamp(percentage, 0, 1);
                    var creatCount =Mathf.FloorToInt( list.Count * percentage);
                    var randomList = new List<int>();
                    while(creatCount>0)
                    {
                        var index = UnityEngine.Random.Range(0, list.Count);

                        while (randomList.Contains(index))
                        {
                            index = UnityEngine.Random.Range(0, list.Count);

                        }
                        randomList.Add(index);
                        creatCount--;
                        
                        var obj = list[index];
                        if (obj)
                        {
                            var pos = obj.transform.localPosition;

                            var prefab = GetRandomPrefab(prefabList);

                            if (prefab)
                            {
                                var go = Instantiate(prefab);
                                go.transform.SetParent(parent);
                                go.transform.localScale = Vector3.one;
                                go.transform.eulerAngles = rotation;
                                go.transform.localPosition = pos;

                                var rectTrans = go.GetComponent<RectTransform>();

                                if (size!=Vector2.zero)
                                {
                                    rectTrans.sizeDelta = size;

                                }

                                if (target == null)
                                {
                                    target = new Dictionary<Vector3, GameObject>();
                                }
                                else
                                {
                                    target.Add(pos, go);
                                }

                            }
                        }
                    }
                }
            }
        }
    }
    #endregion

    #region 创建汽车
    void CreateCar()
    {
        CreateCar(xStreet,Vector3.zero,mapData.WideStreetPos.y);
        CreateCar(yStreet,new Vector3(0,0,90), mapData.WideStreetPos.x);
        CreateCar(xWideStreet,Vector3.zero);
        CreateCar(yWideStreet,new Vector3(0,0,90));
    }

    private void CreateCar(Dictionary<int, List<GameObject>> dict,Vector3 rotation,int skipIndex=-1)
    {
        CreateObj(dict, streetCarDict, CarParent, CarsPrefabs,rotation,Vector2.zero,skipIndex,mapData.StreetCarPercentage);
    }
    #endregion

    #region 创建外墙
    void CreateOutWallAndCorner()
    {
        if (mapData==null)
        {
            return;
        }

       
        CreateOuterWallCorner();
    }

    private void CreateOuterWallCorner()
    {
        var xOffset = (mapData.BuildingSize.x / 2 + mapData.StreetSize.x + mapData.OuterWallSize.x / 2);
        var yOffset = (mapData.BuildingSize.y / 2 + mapData.StreetSize.y + mapData.OuterWallSize.y / 2);

        var xWideOffset = (mapData.WideStreetWidth - 1) * mapData.StreetSize.x;
        var yWideOffset = (mapData.WideStreetWidth - 1) * mapData.StreetSize.y;

        var pos0 = GetBuildPos(new Vector2Int(0, 0));
        var pos1 = GetBuildPos(new Vector2Int( mapData.MapWidth - 1,0));
        var pos2 = GetBuildPos(new Vector2Int(mapData.MapWidth - 1, mapData.MapHeight - 1));
        var pos3 = GetBuildPos(new Vector2Int(0, mapData.MapHeight - 1));

        if (mapData.WideStreetPos.x==0)
        {
            pos0.y -= yWideOffset;
            pos1.y -= yWideOffset;
        }
        else if (mapData.WideStreetPos.x==mapData.MapWidth)
        {
            pos2.y += yWideOffset;
            pos3.y += yWideOffset;
        }

        if (mapData.WideStreetPos.y==0)
        {
            pos0.x -= xWideOffset;
            pos3.x -= xWideOffset;
        }
        else if (mapData.WideStreetPos.y==mapData.MapHeight)
        {
            pos1.x += xWideOffset;
            pos2.x += xWideOffset;
        }

        pos0.x -= xOffset;
        pos0.y -= yOffset;

        pos1.x += xOffset;
        pos1.y -= yOffset;

        pos2.x += xOffset;
        pos2.y += yOffset;

        pos3.x -= xOffset;
        pos3.y += yOffset;

        CreateOutWallCorner(pos0, Vector3.zero);
        CreateOutWallCorner(pos1, new Vector3(0,0,90));
        CreateOutWallCorner(pos2, new Vector3(0,0,180));
        CreateOutWallCorner(pos3, new Vector3(0,0,270));

        CreateOutWall(pos0, pos2);
    }

   

    void CreateOutWallCorner(Vector2 pos,Vector3 rotation)
    {
        var prefab = GetRandomPrefab(OuterCornerWallPrefabs);

        if (prefab)
        {
            var go = Instantiate(prefab);
            go.transform.SetParent(OuterWallCornerParent);
            go.transform.localScale = Vector3.one;
            go.transform.eulerAngles = rotation;
            go.transform.localPosition = pos;

            var rectTrans = go.GetComponent<RectTransform>();
            rectTrans.sizeDelta = mapData.OuterWallSize;

            outerWallList.Add(go);
        }
    }

    

    private void CreateOutWall(Vector2 leftDown,Vector2 rightUp)
    {
        var xCount = GetMaxIntValue(Mathf.Abs(groundLeftDownPos.x * 2) / mapData.OuterWallSize.x);
        var yCount = GetMaxIntValue(Mathf.Abs(groundLeftDownPos.y * 2) / mapData.OuterWallSize.y);
 

        for (int i = 0; i < xCount; i++)
        {
            var x = leftDown.x + (i + 1) * mapData.OuterWallSize.x - mapData.OuterWallGap;
            var pos0 = new Vector2(x, leftDown.y);
            var pos1 = new Vector2(x, rightUp.y);

            CreateOutWall(pos0, Vector3.zero);
            CreateOutWall(pos1, new Vector3(0,0,180));
        }

        for (int i = 0; i < yCount; i++)
        {
            var y = leftDown.y + (i + 1) * mapData.OuterWallSize.x - mapData.OuterWallGap;
            var pos0 = new Vector2(leftDown.x, y);
            var pos1 = new Vector2(rightUp.x, y);

            CreateOutWall(pos0, new Vector3(0,0,270));
            CreateOutWall(pos1, new Vector3(0, 0, 90));
        }
    }

    void CreateOutWall(Vector2 pos,Vector3 rotation)
    {
        var prefab = GetRandomPrefab(OuterWallPrefabs);

        if (prefab)
        {
            var go = Instantiate(prefab);
            go.transform.SetParent(OuterWallParent);
            go.transform.localScale = Vector3.one;
            go.transform.eulerAngles = rotation;
            go.transform.localPosition = pos;

            var rectTrans = go.GetComponent<RectTransform>();
            rectTrans.sizeDelta = mapData.OuterWallSize;

            outerWallList.Add(go);
        }
    }

    int GetMaxIntValue(float value)
    {
        var intValue = (int)value;

        if (intValue==value)
        {
            return intValue;
        }
        else { return intValue + 1; }
    }
    #endregion

    #region 创建街道破坏痕迹
    void CreateStreetBrokenTails()
    {
        CreateStreetBrokenTails(xStreet, Vector3.zero, mapData.WideStreetPos.y);
        CreateStreetBrokenTails(yStreet, new Vector3(0, 0, 90), mapData.WideStreetPos.x);
        CreateStreetBrokenTails(xWideStreet, Vector3.zero);
        CreateStreetBrokenTails(yWideStreet, new Vector3(0, 0, 90));
    }

    private void CreateStreetBrokenTails(Dictionary<int, List<GameObject>> dict,Vector3 rotation, int skipIndex = -1)
    {
        CreateObj(dict, streetBrokenDict, BrokenParent, StreetBrokenPrefabs,rotation,Vector2.zero,skipIndex,mapData.StreetBrokenPercentage);
    }
    #endregion

    bool IndexValid(Vector2Int index)
    {
        return index.x >= 0 && index.x < mapData.MapWidth && index.y >= 0 && index.y < mapData.MapHeight;
    }

    T GetRandomPrefab<T>(List<T> prefabList) where T: UnityEngine.Object
    {
        if (prefabList.IsNullOrEmpty())
        {
            return null;
        }
        else
        {
            var index = UnityEngine.Random.Range(0, prefabList.Count);
            return prefabList[index];
        }
    }

 
 
}


