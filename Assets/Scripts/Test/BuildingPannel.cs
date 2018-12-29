using System.Collections;
using System.Collections.Generic;
using NSBuilding;
using NSGameConfig;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPannel : MonoBehaviour
{
    [SerializeField]
    private Transform _OptionsContainer;
    [SerializeField]
    private GameObject _OptionTemplate;
    [SerializeField]
    private Button _BtnClose;
    [SerializeField]
    private Text _Name;

    private Dictionary<int,Option> _Options = new Dictionary<int, Option>();
    private NSBuilding.BuildingBasic _Building = null;
    
    public NSBuilding.BuildingBasic Building
    {
        set
        {
            if (_Building != value)
            {
                _Building = value;
                Refresh();
            }
        }
        get { return _Building; }
    }

    void RemoveAllOptions()
    {
        foreach (var o in _Options)
        {
            o.Value.Visable(false);
        }
    }

    Option GetOption(int id)
    {
        if (_Options.ContainsKey(id))
        {
            return _Options[id];
        }

        var opt = Instantiate(_OptionTemplate);
        if (opt!=null)
        {
            opt.transform.SetParent(_OptionsContainer);
            opt.transform.localPosition = Vector3.zero;
            opt.transform.localScale = Vector3.one;

            var ops = opt.GetComponent<Option>();
            ops.OptionItem = ConfigManager.Instance.GetBuildingOptionById(id);
            _Options.Add(id,ops);
            return ops;
        }

        return null;
    }

    void Refresh()
    {
        if (_Building != null)
        {
            int[] ops = _Building.CurrentOptions;
            if (ops != null)
            {
                foreach (var i in ops)
                {
                    var op = GetOption(i);
                    op.Visable(true);
                    op.Refresh();
                }
            }

            _Name.text = _Building.Name;
        }
    }

    // Use this for initialization
    void Start()
    {
        _BtnClose.onClick.AddListener(OnCloseClick);
    }

    void OnCloseClick()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
