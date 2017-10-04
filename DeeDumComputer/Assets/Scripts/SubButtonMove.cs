using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubButtonMove : MonoBehaviour {
    public LineRenderer lr;
    public float speed;
    public float lrPosIndex;//input 0-5 for this depending on what location you want the button to go to
    public float delay, delayOG;
    public bool returnToHub;

    // Use this for initialization
	void Start () {
        lr = GetComponentInParent<LineRenderer>();
        speed = GetComponentInParent<HubWorldButtonControl>().slideSpeed;
        transform.localPosition = Vector3.zero; //start at the center of parent button
        delayOG = lrPosIndex/20; //so if it's going to the left, there's no delay; it will go right away.
        delay = delayOG;
	}
	
	// Update is called once per frame
	void Update () {
        if (isActiveAndEnabled)
        {
            if (!returnToHub)
            {
                if (delay > 0)
                {
                    delay -= Time.deltaTime;
                    GetComponent<Button>().interactable = false;

                }
                if (delay <= 0) //5 seconds is a long time to wait.  5/5 is not.
                {
                    gameObject.transform.localPosition = Vector3.Lerp(gameObject.transform.localPosition, new Vector3 (lr.GetPosition((int)lrPosIndex).x, lr.GetPosition((int)lrPosIndex).y, -3), Time.deltaTime * speed); //the new location has a z of 3 so it draws above line renderer
                    if (Vector3.Distance(gameObject.transform.localPosition, Vector3.zero) > (Mathf.Abs(lr.GetPosition((int)lrPosIndex).x)-10f)) //ex. x = 50.  once distance > 40, become interactable
                    {
                        GetComponent<Button>().interactable = true;
                    }
                }
            }
            else
            {
                if (delay > 0)
                {
                    GetComponent<Button>().interactable = false;

                    delay -= Time.deltaTime;
                }
                if (delay <= 0) //5 seconds is a long time to wait.  5/5 is not.
                {
                    gameObject.transform.localPosition = Vector3.Lerp(gameObject.transform.localPosition, Vector3.zero, Time.deltaTime * speed);
                    if(Vector3.Distance(gameObject.transform.localPosition, Vector3.zero) <1f) //when it gets close enough to the hub button, turn off.  this is an easier check than using the object's transform.position (Lerping takes a while)
                    {
                        gameObject.SetActive(false);
                    }
                }

            }
        }
	}

    public void ResetDelays()
    {
        delay = delayOG;
    }
}
