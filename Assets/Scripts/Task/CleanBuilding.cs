using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NSTask
{
    public class CleanBuilding : BuildingTask
    {
        public CleanBuilding() : base()
        {
            _Type = TaskType.kCleanBuilding;
        }
    }
}