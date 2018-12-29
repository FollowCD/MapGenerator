using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NSTask
{
    public class TrainScavenger : BuildingTask
    {
        public TrainScavenger() : base()
        {
            _Type = TaskType.kTrainScavenger;
        }
    }
}