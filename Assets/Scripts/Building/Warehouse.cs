using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NSBuilding
{
    //仓库
    public class Warehouse : BuildingBasic
    {
        public Warehouse() : base()
        {
            _Type = BuildingType.kWarehouse;
        }
    }
}