using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NSTask
{
    public class RebuildChurch : BuildingTask
    {
        public RebuildChurch() : base()
        {
            _Type = TaskType.kRebuildChurch;
        }
    }
}
