using System;
using System.Collections;
using System.Collections.Generic;
using NSGame;
using UnityEngine;

namespace NSTask
{
    public class TaskType
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
    public class BuildingTask : IDate
    {
        protected UInt16 _LeftDays = 0;
        protected int _Type = TaskType.kUnknow;

        public int Type
        {
            get { return _Type; }
        }
        /// <summary>
        /// 一天结束
        /// </summary>
        public void OnDayPassed()
        {
            if (_LeftDays > 0)
            {
                BalanceAccount();
                --_LeftDays;

                if (DaysLeft == 0)
                {
                    OnTaskComplate();
                }
            }
        }

        public void CheckComplate()
        {

        }

        protected void BalanceAccount()
        {

        }
        /// <summary>
        /// 夜幕降临
        /// </summary>
        public void OnNightComing()
        {

        }

        /// <summary>
        /// 任务被打断
        /// </summary>
        public virtual void OnTaskBreak()
        {

        }

        /// <summary>
        /// 任务完成时被调用
        /// </summary>
        public virtual void OnTaskComplate()
        {

        }

        /// <summary>
        /// 任务剩余天数
        /// </summary>
        public UInt16 DaysLeft
        {
            get { return _LeftDays; }
        }

        /// <summary>
        /// 任务是否已经完成
        /// </summary>
        public bool Completed
        {
            get { return DaysLeft <= 0; }
        }
    }
}