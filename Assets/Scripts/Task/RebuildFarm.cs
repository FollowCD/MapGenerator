using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NSTask
{
    public class RebuildFarm : BuildingTask
    {
        public RebuildFarm() : base()
        {
            _Type = TaskType.kRebuildFarm;
        }
    }
}