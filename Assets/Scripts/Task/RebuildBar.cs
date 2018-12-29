using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NSTask
{
    public class RebuildBar : BuildingTask
    {
        public RebuildBar() : base()
        {
            _Type = TaskType.kRebuildBar;
        }
    }
}