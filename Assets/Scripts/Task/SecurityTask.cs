using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NSTask
{
    public class SecurityTask : BuildingTask
    {
        public SecurityTask() : base()
        {
            _Type = TaskType.kSecurityTask;
        }
    }
}