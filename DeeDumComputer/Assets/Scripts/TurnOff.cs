using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOff : MonoBehaviour {

    //this is a dinky script i'm using to turn off the credits panel when the button is pressed to reload scene 0
	// Use this for initialization
	void Start () {
        GameManager.GM.creditsPanel = gameObject;
		if (GameManager.GM.titleInt > 1)
        {
            gameObject.SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
