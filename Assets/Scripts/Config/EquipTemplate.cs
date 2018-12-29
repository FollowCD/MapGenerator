using System;
using System.Collections;
using System.Collections.Generic;
using Person;
using UnityEngine;

namespace NSGameConfig
{
    public class EquipTemplate : IJsRead
    {
        private string _Name;           //名字
        private Skill[] _Skills;        //技能加成
        private int _TypeId;            //类型id
        protected int _Type;            //装备类型
        protected UInt16 _Weight = 0;   //权重
        protected UInt16 _FoodNum = 0;  //可兑换的食物个数

        public UInt16 FoodNum
        {
            get { return _FoodNum; }
        }
        public UInt16 Weight
        {
            get { return _Weight; }
        }
        public int Type
        {
            get { return _Type; }
        }
        public int TypeId
        {
            get { return _TypeId; }
        }

        public Skill[] Skills
        {
            get { return _Skills; }
        }

        public bool ReadJs(Dictionary<string, object> dic)
        {
            _TypeId = Convert.ToInt32(dic["id"]);
            _Name = dic["name"].ToString();
            _Type = Convert.ToInt32(dic["type"]);
            _Weight = Convert.ToUInt16(dic["weight"]);
            _FoodNum = Convert.ToUInt16(dic["food"]);

            var lst = dic["pro"] as List<object>;

            if (lst.Count > 0)
            {
                _Skills = new Skill[lst.Count];
                for (int i = 0; i < lst.Count; ++i)
                {
                    var obj = lst[i] as Dictionary<string, object>;
                    if (obj != null)
                    {
                        _Skills[i] = new Skill();
                        _Skills[i].Reset(Convert.ToInt16(obj["type"]), Convert.ToSingle(obj["level"]));
                    }
                }
            }

            return true;
        }
    }
}