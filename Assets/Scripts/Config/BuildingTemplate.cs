using System;
using System.Collections;
using System.Collections.Generic;
using NSBuilding;
using UnityEngine;

namespace NSGameConfig
{
    public class BuildingTemplate : IJsRead
    {
        private int _Type = BuildingType.kUnknow;
        private UInt16 _Live = 0;
        private UInt16 _Product = 0;
        private UInt16[] _Survivor = null;
        private UInt16[] _Food = null;
        private UInt16[] _Equip = null;
        private EquipGenList _GenEquip = null;
        private string _Name = "";
        private int[] _Options;
        public UInt16[] Foods
        {
            get { return _Food; }
        }
        public UInt16[] Survivor
        {
            get { return _Survivor; }
        }
        public UInt16[] Equip
        {
            get { return _Equip; }
        }
        public bool ReadJs(Dictionary<string, object> dic)
        {
            if (dic != null)
            {
                _Type = Convert.ToInt32(dic["type"]);
                _Live = Convert.ToUInt16(dic["live"]);
                _Product = Convert.ToUInt16(dic["product"]);

                _Name = dic["name"].ToString();

                ReadListI32(dic["options"], ref _Options);
                ReadListU16(dic["survivor"], ref _Survivor);
                ReadListU16(dic["food"], ref _Food);
                ReadListU16(dic["equip"], ref _Equip);

                if (dic.ContainsKey("equiplst"))
                {
                    _GenEquip = new EquipGenList();
                    _GenEquip.ReadJs((Dictionary<string, object>)dic["equiplst"]);
                }
                else
                {
                    _GenEquip = null;
                }
                return true;
            }

            return false;
        }

        public bool GenEquip
        {
            get { return _GenEquip != null; }
        }

        public EquipGenList GenEquiList
        {
            get { return _GenEquip; }
        }

        private bool ReadListU16(object obj, ref UInt16[] array)
        {
            var lst = obj as List<object>;
            if (lst != null && lst.Count > 0)
            {
                array = new UInt16[lst.Count];
                int idx = 0;
                foreach (var i in lst)
                {
                    array[idx] = Convert.ToUInt16(i);
                }

                return true;
            }

            return false;
        }

        private bool ReadListI32(object obj, ref int[] array)
        {
            var lst = obj as List<object>;
            if (lst.Count > 0)
            {
                array = new int[lst.Count];
                int idx = 0;
                foreach (var i in lst)
                {
                    array[idx] = Convert.ToInt32(i);
                    idx += 1;
                }

                return true;
            }

            return false;
        }

        public int[] Options
        {
            get { return _Options; }
        }
        public string Name
        {
            get { return _Name; }
        }

        public int Type
        {
            get { return _Type; }
        }
        public UInt16 Live
        {
            get { return _Live; }
        }
        public UInt16 Product
        {
            get { return _Product; }
        }
    }

}