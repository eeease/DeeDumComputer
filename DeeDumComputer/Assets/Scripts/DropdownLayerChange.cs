using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownLayerChange : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeDropDownLayer()
    {
        GameObject.Find("Dropdown List").GetComponent<Canvas>().overrideSorting = false;
    }
}
