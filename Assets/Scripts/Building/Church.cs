using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NSBuilding
{
    //教会
    public class Church : BuildingBasic
    {
        public Church() : base()
        {
            _Type = BuildingType.kChurch;
        }
    }
}