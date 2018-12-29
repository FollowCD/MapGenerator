using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NSData
{
    public class DataBehaviour : MonoBehaviour
    {
        void Awake()
        {
            Utils.Init();
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Utils.UpdateFrame();
        }

        void OnApplicationQuit()
        {
            Utils.CleanUp();
        }
    }
    
    
}