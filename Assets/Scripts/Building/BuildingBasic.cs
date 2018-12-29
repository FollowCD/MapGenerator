using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using NSData;
using UnityEngine;
using NSGame;
using NSGameConfig;
using NSTask;
using Person;

namespace NSBuilding
{
    public class BuildingType
    {
        public const int kUnknow = 0;
        public const int kFarmLit = 1;              //农场
        public const int kFarmLarge = 2;            //大农场
        public const int kVilla = 3;                //别墅
        public const int kPark = 4;                 //公园
        public const int kWasteland = 5;            //荒地
        public const int kParkingLot = 6;           //停车场
        public const int kOvernightParkingLot = 7;  //夜宿停车场
        public const int kServiceStation = 8;       //路边服务站
        public const int kRuins = 9;                //废墟
        public const int kSchool = 10;              //学校
        public const int kLaboratory = 11;          //研究所
        public const int kHispital = 12;            //医院
        public const int kApartment = 13;           //住宅
        public const int kPub = 14;                 //酒吧
        public const int kChurch = 15;              //教会
        public const int kGasStation = 16;          //加油站
        public const int kWarehouse = 17;           //仓库
        public const int kFleaMarket = 18;          //跳蚤市场
        public const int kMotel = 19;               //汽车旅馆
        public const int kConvenienceStore = 20;    //便利店
        public const int kMcDonald = 21;            //麦当当
        public const int kOffice = 22;              //办公室
        public const int kFoodStore = 23;           //食物卖场
        public const int kOlma = 24;                //奥尔马
        public const int kShoppingMall = 25;        //购物中心
        public const int kCemetery = 26;            //墓地
        public const int kCemeteryLite = 27;        //大墓地
        public const int kPoliceOffice = 28;        //警察局
        public const int kHelicopterApron = 29;     //直升机停机坪
        public const int kCityHall = 30;            //市政厅
        public const int kMetro = 31;               //地铁
    }
    

    public class EquipGenList : IJsRead
    {
        private UInt16 _Ratio = 0;
        private int[] _EquipList = null;

        public UInt16 Ratio
        {
            get { return _Ratio; }
        }

        public int[] EquipList
        {
            get { return _EquipList; }
        }

        public bool ContainsEquip(int id)
        {
            return _EquipList.Contains(id);
        }

        public bool ReadJs(Dictionary<string, object> dic)
        {
            if (dic != null)
            {
                _Ratio = Convert.ToUInt16(dic["ratio"]);
                var lst = dic["lst"] as List<object>;
                if (lst != null && lst.Count > 0)
                {
                    _EquipList = new int[lst.Count];
                    for (int i = 0; i < lst.Count; ++i)
                    {
                        _EquipList[i] = Convert.ToInt32(lst[i]);
                    }
                }
                return true;
            }

            _EquipList = null;
            _Ratio = 0;
            return false;
        }
    }

    public class BuildingBasic:IDate
    {
        protected int _Type = BuildingType.kUnknow;
        protected List<int> _OptionList = new List<int>();
        protected List<int> _OpenedOptions = new List<int>();
        protected BuildingTask _CurrentTask;
        
        //是否已经侦查过
        protected bool _Investigated = false;

        private UInt16 _Foods = 0;           //苹果数量
        private UInt16 _Survivors = 0;        //幸存者数量
        private UInt16 _Equipts = 0;          //装备数量
        private UInt16 _ZombiesCount = 0;     //僵尸数量
        private string _Name = "";            //建筑名字

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public void UpdateZombiesNum(Int16 delta)
        {
            if (delta < 0)
            {
                if (Math.Abs(delta) > _Foods)
                {
                    _ZombiesCount = 0;
                }
            }
            else
            {
                _ZombiesCount += (UInt16)delta;
            }
        }
        public void UpdateFoodNum(Int16 delta)
        {
            if (delta < 0 )
            {
                if (Math.Abs(delta) > _Foods)
                {
                    _Foods = 0;
                }
            }
            else
            {
                _Foods += (UInt16)delta;
            }
        }

        public void UpdateSurvivorsNum(Int16 delta)
        {
            if (delta < 0)
            {
                if (Math.Abs(delta) > _Survivors)
                {
                    _Survivors = 0;
                }
            }
            else
            {
                _Survivors += (UInt16)delta;
            }
        }

        public void UpdateEquipNum(Int16 delta)
        {
            if (delta < 0)
            {
                if (Math.Abs(delta) > _Foods)
                {
                    _Equipts = 0;
                }
            }
            else
            {
                _Equipts += (UInt16)delta;
            }
        }
        public bool Investigated
        {
            set { _Investigated = value; }
        }
        public BuildingTask CurrenTask
        {
            get{ return _CurrentTask;}
        }

        public void AddZombies(UInt16 count)
        {
            _ZombiesCount += count;
        }
        public void OnDayPassed()
        {
            if (_CurrentTask != null)
            {
                _CurrentTask.OnDayPassed();
                if (_CurrentTask.Completed)
                {
                    _CurrentTask = null;
                }
            }
        }

        public void OnNightComing()
        {
            if (_CurrentTask!=null)
            {
                _CurrentTask.OnNightComing();
            }
        }

        /// <summary>
        /// 收复后调用
        /// </summary>
        public virtual void OnRecovered()
        {
        }

        /// <summary>
        /// 被强占后调用,会打断正在执行的任务
        /// </summary>
        public virtual void OnDiscovered()
        {

        }

        public int[] CurrentOptions
        {
            get
            {
                if (_Investigated)
                {
                    return _OpenedOptions.ToArray();
                }
                else
                {
                    return _OptionList.ToArray();
                }
            }
        }
        public void PushOption(int opid)
        {
            _OpenedOptions.Add(opid);
        }

        public void PushOptions(int[] options)
        {
            _OpenedOptions.AddRange(options);
        }
        public BuildingBasic()
        {
            _OptionList.Add(OptionType.kInvestigation);
            _OptionList.Add(OptionType.kCleanBuilding);
            _OptionList.Add(OptionType.kBattle);
            _OptionList.Add(OptionType.kRecoverArea);
        }
    }
}
