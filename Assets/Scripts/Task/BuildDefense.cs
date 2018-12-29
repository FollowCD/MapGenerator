using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NSTask
{
    public class BuildDefense : BuildingTask
    {
        public BuildDefense() : base()
        {
            _Type = TaskType.kBuildDefense;
        }
    }
}