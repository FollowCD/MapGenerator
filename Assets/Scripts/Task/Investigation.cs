using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NSTask
{
    public class Investigation : BuildingTask
    {
        public Investigation() : base()
        {
            _Type = TaskType.kInvestigation;
        }
    }
}