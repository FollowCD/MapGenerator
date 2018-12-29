using System;
using System.Collections;
using System.Collections.Generic;
using NSBuilding;
using NSData;
using Person;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NSGameConfig
{
    public interface IJsRead
    {
        bool ReadJs(Dictionary<string, object> dic);
    }
    public class ConfigManager
    {
        private static ConfigManager _ThisInstance = null;

        private Dictionary<int, OptionTemplate> _OptionTemplates = new Dictionary<int, OptionTemplate>();
        private Dictionary<int, BuildingTemplate> _BuildingTemplates = new Dictionary<int, BuildingTemplate>();
        private Dictionary<int, EquipTemplate> _EquipTemplates = new Dictionary<int, EquipTemplate>();
        private string _NobodyDesc = "";

        private int _EquipUdid = 100000;
        public void CleanUp()
        {
            _BuildingTemplates.Clear();
            _OptionTemplates.Clear();
            _EquipTemplates.Clear();
        }
        public static ConfigManager Instance
        {
            get
            {
                if (_ThisInstance == null)
                {
                    _ThisInstance = new ConfigManager();
                }

                return _ThisInstance;
            }
        }
        public OptionTemplate GetBuildingOptionById(int id)
        {
            if (_OptionTemplates.ContainsKey(id))
            {
                return _OptionTemplates[id];
            }

            return null;
        }

        public BuildingTemplate GetBuildTemplateByType(int buildingtype)
        {
            if (_BuildingTemplates.ContainsKey(buildingtype))
            {
                return _BuildingTemplates[buildingtype];
            }

            return null;
        }
        public void DestroySelf()
        {
            if (_ThisInstance != null)
            {
                _ThisInstance.CleanUp();
                _ThisInstance = null;
            }
        }
        public bool Init()
        {
            try
            {
                TextAsset txt = Resources.Load<TextAsset>("Config/Building/desc");
                if (txt != null)
                {
                    CleanUp();
                    Dictionary<string, object> o = MiniJSON.Json.Deserialize(txt.text) as Dictionary<string, object>;

                    _NobodyDesc = o["nobody"].ToString();
                    object a = o["building_options"];
                    var lst = (List<object>)a;
                    foreach (var i in lst)
                    {
                        OptionTemplate op = new OptionTemplate();
                        op.ReadJs((Dictionary<string, object>)i);
                        _OptionTemplates.Add(op._Id, op);
                    }

                    lst = o["building"] as List<object>;
                    foreach (var i in lst)
                    {
                        BuildingTemplate tmp = new BuildingTemplate();
                        tmp.ReadJs((Dictionary<string, object>)i);
                        _BuildingTemplates.Add(tmp.Type, tmp);
                    }

                    lst = o["equips"] as List<object>;
                    foreach (var i in lst)
                    {
                        EquipTemplate tmp = new EquipTemplate();
                        tmp.ReadJs((Dictionary<string, object>)i);
                        _EquipTemplates.Add(tmp.TypeId, tmp);
                    }

                    var names = o["names"] as Dictionary<string, object>;
                    GameData.Instance.SurvivorMgr.SetNameConfig(names);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 根据装备生成列表，计算生成权重，随机出生成的装备id
        /// </summary>
        /// <param name="lst">装备生成列表，在建筑配置中</param>
        /// <returns></returns>
        private int CalcEquipWeight(ref EquipGenList lst)
        {
            if (lst != null)
            {
                int w = 0;
                foreach (var t in _EquipTemplates)
                {
                    if (lst.ContainsEquip(Convert.ToUInt16(t.Key)))
                    {
                        w += lst.Ratio * t.Value.Weight;
                    }
                    else
                    {
                        w += t.Value.Weight;
                    }
                }

                int r = Random.Range(1, w);

                foreach (var t in _EquipTemplates)
                {
                    if (lst.ContainsEquip(Convert.ToUInt16(t.Key)))
                    {
                        w += lst.Ratio * t.Value.Weight;
                    }
                    else
                    {
                        w += t.Value.Weight;
                    }

                    if (w >= r)
                    {
                        return t.Key;
                    }
                }
                return lst.EquipList[Random.Range(0,lst.EquipList.Length)];
            }

            return 0;
        }

        /// <summary>
        /// 实例化一个装备
        /// </summary>
        /// <param name="buildingTypeId">建筑类型</param>
        /// <returns></returns>
        public Equip InstantiateEquip(int buildingTypeId)
        {
            if (!_BuildingTemplates.ContainsKey(buildingTypeId))
            {
                return null;
            }

            BuildingTemplate temp = _BuildingTemplates[buildingTypeId];
            if (!temp.GenEquip)
            {
                return null;
            }

            var lst = temp.GenEquiList;

            int id = CalcEquipWeight(ref lst);
            if (id == 0)
            {
                return null;
            }

            EquipTemplate tmp = _EquipTemplates[id];
            if (tmp == null)
            {
                return null;
            }

            var equip = new Equip(_EquipUdid++);

            return equip;
        }
    }
}