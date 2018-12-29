using System.Collections;
using System.Collections.Generic;
using NSBuilding;
using NSEvent;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{

    private BuildingBasic _Building;

    public BuildingBasic Data
    {
        get { return _Building; }
        set
        {
            if (value != _Building)
            {
                _Building = value;
                Refresh();
            }
        }
    }

    void Refresh()
    {
        if (Data != null)
        {
            var txt = GetComponentInChildren<Text>();
            if (txt != null)
            {
                txt.text = Data.Name;
            }
        }
    }

    void Start()
    {
        Button btn = GetComponent<Button>();
        if (btn!=null)
        {
            btn.onClick.AddListener(OnClick);
        }
    }

    void OnClick()
    {
        if (Data!=null)
        {
            Utils.TriggerEvent(EvtID.kEnterBuilding,Data);
        }
    }
}
