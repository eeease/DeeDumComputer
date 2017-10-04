using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeTextGlide : MonoBehaviour {
    public bool floatingUp, floatingHome;
    public float upSpeed, homeSpeed, upDist, startTimer;
    TextMesh myText;
    GameObject myParent, myHome;
    Vector3 upPos, homePos;
    public Vector3 largestScale;
    public AudioClip[] textNoises;
    AudioSource mySound;
    int audioSeparator = 0;

	// Use this for initialization
	void Start () {
        myText = GetComponent<TextMesh>();
        mySound = GetComponent<AudioSource>();
        myParent = transform.parent.gameObject;
        Color parentCol = myParent.GetComponent<SpriteRenderer>().color;
        myText.color = parentCol; //set the text's color to the parent block's color.
        floatingUp = true;
        floatingHome = false;
        upPos = new Vector3(transform.localPosition.x, upDist);
        if (myParent.name.Contains("z")) //player2 has a z in its name (easier than writing out the full name) ~~(possibly not as performant?)
        {
            myHome = GameObject.Find("PurpleGoSpawn");
        }else
        {
            myHome = GameObject.Find("GreenGoSpawn");
        }
        GetComponent<TrailRenderer>().startColor = parentCol;

        GetComponent<TrailRenderer>().endColor = new Color(parentCol.r, parentCol.g, parentCol.b, .2f);
        GetComponent<TrailRenderer>().enabled = false;
        transform.localScale = Vector3.zero; //start it zero until it starts floating up (some languages were popping up through the cubes)
    }
	
	// Update is called once per frame
	void Update () {
        startTimer -= Time.deltaTime;
        if (startTimer <= 0)
        {
            if (!GameManager.GM.muteSFX && !mySound.isPlaying && audioSeparator==0)
            {
                audioSeparator++;
                mySound.clip = textNoises[0]; //play the 'home' sfx
                mySound.Play();
            }
            if (floatingUp)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, largestScale, upSpeed);
                transform.localPosition = Vector3.Lerp(transform.localPosition, upPos, upSpeed);
                if (Vector3.Distance(transform.localPosition, upPos) < .2f)  //once it basically gets to its upPosition, bool switch.
                {
                    floatingUp = false;
                    floatingHome = true;
                    GetComponent<TrailRenderer>().enabled = true;
                    GetComponent<TrailRenderer>().Clear();
                }
            }
            if (floatingHome)
            {
                if (!GameManager.GM.muteSFX && !mySound.isPlaying && audioSeparator ==1)
                {
                    audioSeparator++;
                    mySound.volume = 1;
                    mySound.clip = textNoises[1];
                    mySound.Play();
                }
                transform.SetParent(myHome.transform);
                transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, homeSpeed);

                if (Vector3.Distance(transform.localPosition, Vector3.zero) < .2f)
                {
                    //right now i'm going to have trailrenderer autodestruct instead of turning it off in code. ~~may change.
                   // gameObject.SetActive(false); //dont' know if I should destroy here or not.  setting inactive for now in case i want to make it turn on from tapping player
                }
            }
        }
		
	}
}
