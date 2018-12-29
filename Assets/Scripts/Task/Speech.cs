using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NSTask
{
    public class Speech : BuildingTask
    {
        public Speech() : base()
        {
            _Type = TaskType.kSpeech;
        }
    }
}