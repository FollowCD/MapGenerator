using System;
using System.Collections;
using System.Collections.Generic;
using NSBuilding;
using NSData;
using UnityEngine;

namespace NSGameConfig
{
    /// <summary>
    /// 操作类型
    /// </summary>
    public class OptionType
    {
        public const int kUnknow = 0;
        public const int kCleanBuilding = 10001;        //清理杂物
        public const int kInvestigation = 10002;        //侦查未知区域
        public const int kBattle = 10003;               //消灭僵尸
        public const int kRecoverArea = 10004;          //收复区域
        public const int kConscribe = 10005;            //招募幸存者
        public const int kAttackDoomsdayTrial = 10006;  //进攻末日审判
        public const int kSecurityTask = 10007;         //警卫任务
        public const int kRebuildApartment = 10008;     //改建公寓
        public const int kRebuildBar = 10009;           //改建酒吧
        public const int kRebuildHospital = 10010;      //改建医院
        public const int kRebuildInstitute = 10011;     //改建研究院
        public const int kStudyElectricPower = 10012;   //电力研究
        public const int kRebuildHouse = 10013;         //改建为住宅
        public const int kRebuildFarm = 10014;        //改建为农场
        public const int kRebuildChurch = 10015;        //改建为教堂
        public const int kTrainLeader = 10016;          //leader培养
        public const int kTrainBuilder = 10017;         //builder培养
        public const int kTrainSoldier = 10018;         //soldier培养
        public const int kTrainscientist = 10019;       //scientist培养
        public const int kTrainScavenger = 10020;       //scavenger培养
        public const int kSpeech = 10021;               //演讲
        public const int kCareBar = 10022;              //指派酒吧侍者
        public const int kCareApple = 10023;            //照料作物
        public const int kRebuildSchol = 10024;         //改建成一所学校
        public const int kBuildDefense = 10025;         //修建防御工事
        public const int kConstitution = 10026;         //撰写宪法
    }

    /// <summary>
    /// 建筑物操作选项
    /// </summary>
    public class OptionTemplate : IJsRead
    {
        public bool CheckExcute()
        {
            return true;
        }
        public OptionTemplate()
        {
            _Id = OptionType.kUnknow;
        }

        public bool ReadJs(Dictionary<string, object> dic)
        {
            if (dic != null)
            {
                _Id = Convert.ToInt32(dic["id"]);
                _Title = dic["title"].ToString();
                _Desc1 = dic["desc1"].ToString();
                _Desc2 = dic["desc2"].ToString();
                _Conditions = Convert.ToInt32(dic["conditions"]);
                return true;
            }

            return false;
        }

        public int _Id;
        public string _Title;
        public string _Desc1;
        public string _Desc2;
        public int _Conditions;

        /// <summary>
        /// 检查是否满足选项需求
        /// </summary>
        /// <returns></returns>
        public virtual bool Check(ref BuildingBasic building)
        {
            return false;
        }
        /// <summary>
        /// 请复写该函数，不同的选项进入后是不同的
        /// </summary>
        public virtual void OnEnter()
        {
            Debug.Log("Enter the Option:" + ToString());
        }

        public override string ToString()
        {
            return string.Format("Option Id:{0},Title:{1},Desc:{2}", _Id, _Title, _Desc1);
        }

        public bool CanExecute(ref string reason)
        {
            if (GameData.Alive)
            {
                if (GameData.Instance.IsRioting)
                {
                    reason = "发生暴乱,不可进行新的任务";
                    return false;
                }

                if (GameData.Instance.IsCelebrating)
                {
                    reason = "全员庆祝,无心工作，不可自拔";
                    return false;
                }

                if (!GameData.Instance.SurvivorMgr.CheckCondition(this._Conditions, ref reason))
                {
                    return false;
                }
            }
            return false;
        }
    }
}