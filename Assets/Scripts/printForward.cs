using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class printForward : MonoBehaviour {

	// Use this for initialization
	void Start () {
        print(this.gameObject.transform.forward);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
