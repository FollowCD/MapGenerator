using System;
using System.Collections;
using System.Collections.Generic;
using NSBuilding;
using Person;
using UnityEngine;

namespace NSData
{
    
    public class GameData:NSGame.IDate
    {
        private static GameData _ThisIntance = null;
        public static GameData Instance
        {
            get
            {
                if (_ThisIntance == null)
                {
                    _ThisIntance = new GameData();
                    _ThisIntance.Init();
                }

                return _ThisIntance;
            }
        }

        public static bool Alive
        {
            get { return _ThisIntance != null; }
        }
        public static void DestroySelf()
        {
            if (_ThisIntance != null)
            {
                _ThisIntance.CleanUp();
                _ThisIntance = null;
            }
        }

        //--------------------------------数据----------------------------
        private SurvivorManager _SurvivorMgr = new SurvivorManager();
        private BuildingManager _BuildingMgr = new BuildingManager();
        private Difficulty  _Difficulty = new Difficulty(Difficulty.kEasy);

        public Difficulty Difficult
        {
            get { return _Difficulty; }
        }
        public void Init()
        {
            _BuildingMgr.Init();
            _SurvivorMgr.Init();
        }

        public void CleanUp()
        {
            _BuildingMgr.CleanUp();
            _SurvivorMgr.CleanUp();
        }

        public SurvivorManager SurvivorMgr
        {
            get { return _SurvivorMgr; }
        }

        public BuildingManager BuildingMgr
        {
            get { return _BuildingMgr; }
        }

        //骚乱
        private bool rioting = false;
        //庆祝
        private bool riotingCelebrating = false;
        //幸福度
        private UInt16 morale = 0;
        
        public UInt16 Morale
        {
            get
            {
                if (morale < LeastMorale)
                {
                    morale = LeastMorale;
                }

                return morale;
            }
        }
        public UInt16 LeastMorale
        {
            get
            {
                return 0;
            }
        }
        public bool IsRioting
        {
            get { return rioting; }
        }

        public bool IsCelebrating
        {
            get { return riotingCelebrating; }
        }

        public void OnNightComing()
        {

        }

        public void OnDayPassed()
        {
            if (Morale < 20)
            {
                // 幸福指数低于20，发生暴动
                rioting = true;
            }
        }
    }
}
