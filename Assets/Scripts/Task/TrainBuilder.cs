using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NSTask
{
    public class TrainBuilder : BuildingTask
    {
        public TrainBuilder() : base()
        {
            _Type = TaskType.kTrainBuilder;
        }
    }
}