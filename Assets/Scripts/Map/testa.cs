using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testa : MonoBehaviour {

	// Use this for initialization
	void Start () {
        BuildingRect a = new BuildingRect(1, 1, 1, 1);
        BuildingRect b = new BuildingRect(1, 1, 1, 1);

        Debug.Log("a==b：" + a.Equals(b));
    }
	
	void TestValueRef(ref int i)
    {
        i += 2;
    }
}
