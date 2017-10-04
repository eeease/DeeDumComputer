using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusSwirlScaleCheck : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(transform.localScale.x> 1)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        if (transform.localScale.x < 0.6)
        {
            transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        }
	}
}
