using System;
using System.Collections;
using System.Collections.Generic;
using Person;
using UnityEngine;

namespace NSData
{
    public class Difficulty
    {
        public const int kEasy = 1;
        public const int kNormal = 2;
        public const int kHard = 3;
        public const int kUnbeliveable = 4;

        private readonly int _Difficult = kEasy;

        public Difficulty(int difficult)
        {
            _Difficult = difficult;
        }

        public int Type
        {
            get { return _Difficult; }
        }

        public Int16 PickFoundSkill()
        {
            if (_Difficult == kEasy || _Difficult == kNormal)
            {
                if (Utils.GetRandChance(1,2))
                {
                    return SkillType.kAttack;
                }
            }

            return SkillType.RandomSkill();
        }
    }
}
