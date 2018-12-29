using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NSBuilding
{
    //加油站
    public class GasStation : BuildingBasic
    {
        public GasStation() : base()
        {
            _Type = BuildingType.kGasStation;
        }
    }
}