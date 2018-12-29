using System;
using System.Collections;
using System.Collections.Generic;
using NSGameConfig;
using UnityEngine;

namespace Person
{
    public class SkillType
    {
        public const Int16 kUnknow = 0;   
        public const Int16 kLeader = 1;   //领导力
        public const Int16 kBuilder = 2;  //建造力
        public const Int16 kAttack = 3;   //战斗力
        public const Int16 kScientist = 4;//科研力
        public const Int16 kScavenger = 5;//搜查力

        public static Int16 RandomSkill()
        {
            Int16[] skills = new Int16[5] {kLeader, kBuilder, kAttack, kScientist, kScavenger};
            return Utils.PickRandom(skills);
        }
    }

    public class EquipType
    {
        public const Int16 kUnknow = 0;
        public const Int16 kHand = 1;   //手持穿戴式
        public const Int16 kDress = 2;  //环身穿戴式
        public const Int16 kWaist = 3;  //腰部穿戴式
        public const Int16 kDog = 4;    //狗狗
        public const Int16 kCat = 5;    //喵喵
        public const Int16 kHead = 6;   //头戴式
    }

    public class Skill
    {
        private Int16 _Type = SkillType.kUnknow;
        private float _Level = 0.0f;

        public float Level
        {
            get { return _Level; }
        }
        public void UpdateLevel(float delta)
        {
            _Level += delta;
        }
        public Int16 Type
        {
            get { return _Type; }
        }

        public void Reset(Int16 type, float level)
        {
            _Type = type;
            _Level = level;
        }
    }

    public class Equip: EquipTemplate
    {
        private int _Udid = 0;
        private int _SurvivorId = 0;

        public Equip(int udid) : base()
        {
            _Udid = udid;
        }
        public int UDID
        {
            get { return _Udid; }
        }
        public bool IsUsed
        {
            get { return _SurvivorId != 0; }
        }

        public void Used(int id)
        {
            _SurvivorId = id;
        }
        public void UnUsed()
        {
            _SurvivorId = 0;
        }

    }
    public class Survivor
    {
        protected string _Name;                 //幸存者名字
        protected bool _IsMale;                 //幸存者性别
        protected int _Id;                      //幸存者ID
        Skill[] _Skills = new Skill[3];         //幸存者技能
        private Equip _Equip;                   //幸存者装备
        private NSTask.BuildingTask _CurrentTask; // 当前执行的任务

        public Survivor(int id)
        {
            _Id = id;
        }
        public bool HasSkill(Int16 type,int lv = 1)
        {
            foreach (var a in _Skills)
            {
                if (a.Type == type && a.Level >= lv)
                {
                    return true;
                }
            }

            return false;
        }
        public void CancelTask()
        {
            _CurrentTask = null;
        }

        /// <summary>
        /// 是否为男性: true->是  false->否
        /// </summary>
        public bool IsMale
        {
            get { return _IsMale; }
        }
        public bool Idle
        {
            get { return _CurrentTask == null; }
        }

        public string NickName
        {
            get { return _Name; }
            set { _Name = value; }
        }
        /// <summary>
        /// 获取技能信息
        /// </summary>
        public Skill[] GetSkillProperty
        {
            get { return _Skills; }
        }

        /// <summary>
        /// 技能更新
        /// </summary>
        /// <param name="type">技能类型</param>
        /// <param name="level">技能等级</param>
        public void UpdateSkill(Int16 type,float level)
        {
            for (int i = 0; i < 3; i++)
            {
                if (type == _Skills[i].Type)
                {
                    _Skills[i].UpdateLevel(level);
                    break;
                }
                else if (_Skills[i].Type == SkillType.kUnknow)
                {
                    _Skills[i].Reset(type,level);
                    break;
                }
            }
        }

        /// <summary>
        /// 附加装备
        /// </summary>
        /// <param name="equip">附加的装备对象</param>
        public void AttachEquip(Equip equip)
        {
            if (equip != null)
            {
                DettachEquip();

                var skills = _Equip.Skills;
                if (skills != null)
                {
                    foreach (var s in skills)
                    {
                        foreach (var os in _Skills)
                        {
                            if (s.Type == os.Type)
                            {
                                os.UpdateLevel(+s.Level);
                                break;
                            }
                        }
                    }
                }
                equip.Used(_Id);
            }
        }

        /// <summary>
        /// 解除装备
        /// </summary>
        public void DettachEquip()
        {
            if (_Equip!=null)
            {
                var skills = _Equip.Skills;
                if (skills!=null)
                {
                    foreach (var s in skills)
                    {
                        foreach (var os in _Skills)
                        {
                            if (s.Type == os.Type)
                            {
                                os.UpdateLevel(-s.Level);
                                break;
                            }
                        }
                    }
                }
                _Equip.UnUsed();
            }
        }
    }
}
