using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NSTask
{
    public class TaskManager
    {
        private static TaskManager _ThisInstance = null;

        public static TaskManager Instance
        {
            get
            {
                if (_ThisInstance == null)
                {
                    _ThisInstance = new TaskManager();
                    _ThisInstance.Init();
                }

                return _ThisInstance;
            }
        }

        public static void DestroySelf()
        {
            if (_ThisInstance == null)
            {
                _ThisInstance.CleanUp();
                _ThisInstance = null;
            }
        }

        public void Init()
        {

        }
        public void CleanUp()
        {

        }
    }
}