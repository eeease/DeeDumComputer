using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepMeAlive : MonoBehaviour {
    //public static KeepMeAlive KMA;

    void Awake()
    {
        if (GameObject.Find(this.name) != null)
        {
            DontDestroyOnLoad(this);
            //KMA = this;
        }
        //else Destroy(this);
    }
	// Use this for initialization
	void Start () {
        for(int i=1; i< GameObject.FindGameObjectsWithTag("Respawn").Length; i++)
        {
            Destroy(GameObject.FindGameObjectsWithTag("Respawn")[i]);
        }
       
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
