using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NSTask
{
    public class RebuildHospital : BuildingTask
    {
        public RebuildHospital() : base()
        {
            _Type = TaskType.kRebuildHospital;
        }
    }
}
