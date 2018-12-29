using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NSTask
{
    public class TrainSoldier : BuildingTask
    {
        public TrainSoldier() : base()
        {
            _Type = TaskType.kTrainSoldier;
        }
    }
}