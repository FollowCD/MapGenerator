using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NSTask
{
    public class RebuildApartment : BuildingTask
    {
        public RebuildApartment() : base()
        {
            _Type = TaskType.kRebuildApartment;
        }
    }
}
