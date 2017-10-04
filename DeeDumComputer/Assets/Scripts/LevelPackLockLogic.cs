using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelPackLockLogic : MonoBehaviour {
    public bool locked,unlockThendie;
    public Button[] hubButtonsToUnlock;
    public List<GameObject> surroundingLocks;
    public float unlockDeathTimer, unlockDeathTimerOG;
	// Use this for initialization
	void Start () {
    
        locked = PlayerPrefsX.GetBool(gameObject.name + "locked", true);
        foreach(Transform lockpc in GetComponentsInChildren<Transform>())
        {
            if (lockpc.gameObject.tag == "LockPiece")
            {
                surroundingLocks.Add(lockpc.gameObject);
            }
        }
        if (locked)
        {
            foreach(Button butt in hubButtonsToUnlock)
            {
                butt.gameObject.SetActive(false);
            }
        }else
        {
            gameObject.SetActive(false); //otherwise turn this off.
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (unlockThendie)
        {
            unlockDeathTimer -= Time.deltaTime;
            if (unlockDeathTimer <= 0)
            {
                gameObject.SetActive(false);

            }
        }
	}
    
    public void UnlockTheHubButtons()
    {
        PlayerPrefsX.SetBool(gameObject.name + "locked", false);
        foreach(Button butt in hubButtonsToUnlock)
        {
            butt.gameObject.SetActive(true);
        }
        foreach(GameObject lockpc in surroundingLocks)
        {
            lockpc.GetComponent<Animator>().SetBool("unlockSpin", true);
        }
        unlockThendie = true; //this will set the GO inactive after a few seconds.
    }
}
