using System.Collections;
using System.Collections.Generic;
using NSBuilding;
using NSData;
using UnityEngine;
using UnityEngine.UI;

public class MapArea : MonoBehaviour
{
    [SerializeField]
    private GameObject BuildingTemplate;
    [SerializeField]
    private Button BtnGenerate;

    void Awake()
    {
        BtnGenerate.onClick.AddListener(OnGenerateClick);
    }

    void OnGenerateClick()
    {
        BuildingRect rect = new BuildingRect(1,1);
        BuildingRect rect1 = new BuildingRect(1, 1);
        var building = GameData.Instance.BuildingMgr.RequestBuilding(BuildingType.kFarmLit,rect);
        GameData.Instance.BuildingMgr.RequestBuilding(BuildingType.kFarmLit, rect1);
        if (building!=null)
        {
            var go = Instantiate(BuildingTemplate);
            go.transform.SetParent(BuildingTemplate.transform.parent);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            go.SetActive(true);
            var b = go.GetComponent<Building>();
            if (b!=null)
            {
                b.Data = building;
            }
        }
    }
    void Start()
    {

    }

    void Update()
    {
    }
}
