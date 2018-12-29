using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NSBuilding
{
    // 路边服务站
    public class ServiceStation : BuildingBasic
    {
        public ServiceStation() : base()
        {
            _Type = BuildingType.kServiceStation;
        }
    }
}