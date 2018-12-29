using System.Collections;
using System.Collections.Generic;
using NSBuilding;
using NSEvent;
using UnityEngine;

public class GUIMain : MonoBehaviour
{
    [SerializeField]
    private GameObject _BuildingPannel;
    void Start()
    {
        Utils.ListenEvent(EvtID.kEnterBuilding,EnterBuilding);
    }

    void EnterBuilding(object arg)
    {
        BuildingBasic building = arg as BuildingBasic;
        if (building != null)
        {
            _BuildingPannel.SetActive(true);
            var pannel = _BuildingPannel.GetComponent<BuildingPannel>();
            if (pannel)
            {
                pannel.Building = building;
            }
        }
    }

    private void OnDestroy()
    {
        Utils.RemoveEvent(EvtID.kEnterBuilding,EnterBuilding);
    }
}
