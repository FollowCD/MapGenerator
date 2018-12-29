using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NSBuilding
{
    //公寓
    public class Apartment : BuildingBasic
    {
        public Apartment() : base()
        {
            _Type = BuildingType.kApartment;
        }
    }
}