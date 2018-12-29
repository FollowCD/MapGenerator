using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NSBuilding
{
    //食品卖场
    public class FoodStore : BuildingBasic
    {
        public FoodStore() : base()
        {
            _Type = BuildingType.kFoodStore;
        }
    }
}