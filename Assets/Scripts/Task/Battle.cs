using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NSTask
{
    public class Battle : BuildingTask
    {
        public Battle() : base()
        {
            _Type = TaskType.kBattle;
        }
    }
}