using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NSTask
{
    public class Constitution : BuildingTask
    {
        public Constitution() : base()
        {
            _Type = TaskType.kConstitution;
        }
    }
}