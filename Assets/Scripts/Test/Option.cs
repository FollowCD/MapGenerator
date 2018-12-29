using System.Collections;
using System.Collections.Generic;
using NSBuilding;
using NSGameConfig;
using UnityEngine;
using UnityEngine.UI;

public class Option : MonoBehaviour
{
    private OptionTemplate _BuildingOption;

    private Button _btnClick = null;

    void Awake()
    {
        _btnClick = GetComponent<Button>();
        if (_btnClick != null)
        {
            _btnClick.onClick.AddListener(OnOptionClick);
        }
    }


    public void Visable(bool bVisable)
    {
        gameObject.SetActive(bVisable);
    }
    public void OnOptionClick()
    {
        if (OptionItem!=null)
        {
            Debug.Log("进入选项");
        }
    }
    public OptionTemplate OptionItem
    {
        set
        {
            _BuildingOption = value;
        }
        get { return _BuildingOption; }
    }

    public void Refresh()
    {
        if (_BuildingOption != null)
        {
            var txt = GetComponent<Text>();
            if (txt != null)
            {
                txt.text = _BuildingOption._Title;
                string reason = "";
                if (_BuildingOption.CanExecute(ref reason))
                {
                    txt.color = Color.white;
                    _btnClick.interactable = true;
                }
                else
                {
                    txt.color = Color.red;
                    _btnClick.interactable = false;
                }
            }
        }
    }


}
