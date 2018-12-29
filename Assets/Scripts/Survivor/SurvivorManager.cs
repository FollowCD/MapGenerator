using System;
using System.Collections;
using System.Collections.Generic;
using NSData;
using UnityEngine;

namespace Person
{
    public class SurvivorManager
    {
        private static int _StartID = 1024000;
        private Dictionary<int,Survivor> _Survivors = new Dictionary<int, Survivor>();

        private string[] _Male_soldier_first_names = null;
        private string[] _Female_soldier_first_names = null;
        private string[] _Male_leader_first_names = null;
        private string[] _Female_leader_first_names = null;
        private string[] _Male_scientist_first_names = null;
        private string[] _Female_scientist_first_names = null;
        private string[] _Male_first_names = null;
        private string[] _Female_first_names = null;
        private string[] _Nick_names = null;
        private string[] _Eyeless_nick_names = null;
        private string[] _Soldier_nick_names = null;
        private string[] _Scientist_nick_names = null;
        private string[] _Last_names = null;

        private void ReadArray(out string[] _array,object jsobject)
        {
            var lst = jsobject as List<object>;
            if (lst!=null && lst.Count > 0)
            {
                _array = new string[lst.Count];
                for (int i = 0; i < lst.Count; ++i)
                {
                    _array[i] = lst[i].ToString();
                }
            }
            else
            {
                _array = null;
            }
        }
        public void SetNameConfig(Dictionary<string,object> dic)
        {
            if (dic != null)
            {
                ReadArray(out _Male_soldier_first_names, dic["MALE_SOLDIER_FIRST_NAMES"]);
                ReadArray(out _Female_soldier_first_names, dic["FEMALE_SOLDIER_FIRST_NAMES"]);
                ReadArray(out _Male_leader_first_names, dic["MALE_LEADER_FIRST_NAMES"]);
                ReadArray(out _Female_leader_first_names, dic["FEMALE_LEADER_FIRST_NAMES"]);
                ReadArray(out _Male_scientist_first_names, dic["MALE_SCIENTIST_FIRST_NAMES"]);
                ReadArray(out _Female_scientist_first_names, dic["FEMALE_SCIENTIST_FIRST_NAMES"]);
                ReadArray(out _Male_first_names, dic["MALE_FIRST_NAMES"]);
                ReadArray(out _Female_first_names, dic["FEMALE_FIRST_NAMES"]);
                ReadArray(out _Nick_names, dic["NICK_NAMES"]);
                ReadArray(out _Eyeless_nick_names, dic["EYELESS_NICK_NAMES"]);
                ReadArray(out _Soldier_nick_names, dic["SOLDIER_NICK_NAMES"]);
                ReadArray(out _Scientist_nick_names, dic["SCIENTIST_NICK_NAMES"]);
                ReadArray(out _Last_names, dic["LAST_NAMES"]);
            }
        }

        public void Init()
        {

        }

        public void CleanUp()
        {

        }

        public Survivor NewSurvivor(Int16 type = SkillType.kUnknow, float lv=1.0f,bool male = true,bool add=true)
        {
            var person = new Survivor(_StartID);

            if (type == SkillType.kUnknow)
            {
                type = GameData.Instance.Difficult.PickFoundSkill();
            }
            string name = RandomName(type, male, 1);
            person.NickName = name;
            if (add)
            {
                _Survivors.Add(_StartID, person);
            }
            ++_StartID;
            return person;
        }

        /// <summary>
        /// 随机生成一个名字
        /// </summary>
        /// <param name="type"></param>
        /// <param name="male"></param>
        /// <param name="dupeAttempt"></param>
        /// <returns></returns>
        public string RandomName(Int16 type,bool male,int dupeAttempt=1)
        {
            bool hasNickName = Utils.GetRandChance(1,5);
            bool hasFirstName = Utils.GetRandChance(4, 5);
            bool hasLastName = Utils.GetRandChance(4, 5);
            if (type == SkillType.kAttack)
            {
                hasNickName = Utils.GetRandChance(1, 3);
                hasFirstName = Utils.GetRandChance(3, 5);
            }
            if (type == SkillType.kScientist || type == SkillType.kLeader)
            {
                hasNickName = Utils.GetRandChance(1, 7);
                hasFirstName = Utils.GetRandChance(9, 10);
                hasLastName = Utils.GetRandChance(9, 10);
            }

            if (male && Utils.GetRandChance(4, 5)
                || type == SkillType.kAttack && Utils.GetRandChance(3, 4)
                || type == SkillType.kAttack && Utils.GetRandChance(1, 2))
            {
                hasFirstName = true;
                hasNickName = false;
                hasLastName = true;
            }

            string nickName = "";
            string firstName = "";
            string lastName = "";
            if (hasNickName)
            {
                if (type == SkillType.kAttack && Utils.GetRandChance(1,3))
                {
                    nickName = Utils.PickRandom(_Soldier_nick_names);
                }
                else if (type == SkillType.kScientist && Utils.GetRandChance(1, 3))
                {
                    nickName = Utils.PickRandom(_Scientist_nick_names);
                }
                else
                {
                    nickName = Utils.PickRandom(_Nick_names);
                }
            }

            if (hasFirstName)
            {
                if (type == SkillType.kLeader && Utils.GetRandChance(1,3))
                {
                    hasLastName = true;
                    if (male) 
                    {
                        firstName = Utils.PickRandom(_Male_leader_first_names);
                    }
                    else
                    {
                        firstName = Utils.PickRandom(_Female_leader_first_names);
                    }
                }
                else if (type == SkillType.kAttack && Utils.GetRandChance(1, 3))
                {
                    hasLastName = true;
                    if (male)
                    {
                        firstName = Utils.PickRandom(_Male_soldier_first_names);
                    }
                    else
                    {
                        firstName = Utils.PickRandom(_Female_soldier_first_names);
                    }
                }
                else if (type == SkillType.kScientist && Utils.GetRandChance(1, 3))
                {
                    hasLastName = true;
                    if (male)
                    {
                        firstName = Utils.PickRandom(_Male_scientist_first_names);
                    }
                    else
                    {
                        firstName = Utils.PickRandom(_Female_scientist_first_names);
                    }
                }
                else if (male)
                {
                    firstName = Utils.PickRandom(_Male_first_names);
                }
                else
                {
                    firstName = Utils.PickRandom(_Female_first_names);
                }
            }

            if (hasLastName)
            {
                lastName = Utils.PickRandom(_Last_names);
            }

            if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(nickName) && string.IsNullOrEmpty(lastName))
            {
                return RandomName(type, male, dupeAttempt);
            }

            if (!string.IsNullOrEmpty(nickName) && !string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
            {
                lastName = firstName;
                firstName = "";
            }

            if (!string.IsNullOrEmpty(nickName) && string.IsNullOrEmpty(firstName)&&!string.IsNullOrEmpty(lastName))
            {
                firstName = lastName;
                lastName = "";
            }

            string target = Utils.MakeName(firstName,nickName,lastName);

            if (!ContainName(target))
            {
                return target;
            }
            else
            {
                if (dupeAttempt < 5)
                {
                    dupeAttempt += 1;
                    return RandomName(type, male, dupeAttempt);
                }
                else
                {
                    target += "1";
                }
            }
            return target ;
        }

        /// <summary>
        /// 检查是否已经有该名字的幸存者
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool ContainName(string name)
        {
            foreach (var sur in _Survivors)
            {
                if (sur.Value.NickName == name)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 检查任务的条件需求
        /// 1234:技能1的需要等级为2的至少34个人
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public bool CheckCondition(int condition,ref string reason)
        {
            if (condition <= 10)
            {
                if (HasEnoughIdle(condition))
                {
                    return true;
                }
                reason = "至少需要" + condition + "闲置人员才可进行此项任务";
            }
            else
            {
                int count = condition % 100;
                Int16 skill = (Int16)(condition / 1000 % 10);
                int lv = condition / 100 % 10;
                if (HasEnoughIdle(count, skill, lv))
                {
                    return true;
                }

                string[] jobs = { "","Leader","Builder", "Soldier", "Scientist", "Scavenger" };
                reason = string.Format("至少需要{0}个{1}级或以上的{2}", count, lv, skill);
            }

            return false;
        }

        /// <summary>
        /// 获取闲置幸存者数量
        /// </summary>
        /// <returns></returns>
        public int GetIdleCount()
        {
            int i = 0;
            foreach (var s in _Survivors)
            {
                if (s.Value.Idle)
                {
                    ++i;
                }
            }
            return i;
        }

        public bool HasEnoughIdle(int count, Int16 skill,int lv)
        {
            if (count <= 0)
            {
                return true;
            }
            int i = 0;
            foreach (var s in _Survivors)
            {
                if (s.Value.Idle && s.Value.HasSkill(skill,lv))
                {
                    i += 1;
                    if (count >= i)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool HasEnoughIdle(int count)
        {
            if (count <= 0)
            {
                return true;
            }
            int i = 0;
            foreach (var s in _Survivors)
            {
                if (s.Value.Idle)
                {
                    i += 1;
                    if (count >= i)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
