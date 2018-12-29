using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NSGame
{
    public interface IDate
    {
        void OnNightComing();
        void OnDayPassed();
    }
}