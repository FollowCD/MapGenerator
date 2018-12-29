using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NSTask
{
    public class RebuildHouse : BuildingTask
    {
        public RebuildHouse() : base()
        {
            _Type = TaskType.kRebuildHouse;
        }
    }
}