using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NSTask
{
    public class RebuildInstitute : BuildingTask
    {
        public RebuildInstitute() : base()
        {
            _Type = TaskType.kRebuildInstitute;
        }
    }
}