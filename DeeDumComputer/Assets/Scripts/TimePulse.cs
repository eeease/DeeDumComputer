using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimePulse : MonoBehaviour {
    Text thisText;
    Vector3 originalScale;
    public float speed;
    bool enlarge;
    public float enlargeCD, enlargeCDOG;
    AudioSource thisAudio;
    public AudioClip clip;
    public bool playSound;
	// Use this for initialization
	void Start () {
        thisText = GetComponent<Text>();
        originalScale = transform.localScale;
        enlargeCD = enlargeCDOG;
        if (playSound)
        {
            if (GetComponent<AudioSource>() != null)
            {
                thisAudio = GetComponent<AudioSource>();
            }
            else
            {
                thisAudio = gameObject.AddComponent<AudioSource>() as AudioSource; //add an audiosource if there isn't one on there already
            }
            thisAudio.playOnAwake = false; //otherwise it'll play whatever sound is in there
        }
    }

    // Update is called once per frame
    void Update () {
        if(transform.localScale.x > 1)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, speed * Time.deltaTime);
        }
        if(transform.localScale.x < 1.02)
        {
            transform.localScale = originalScale;
        }
        if (enlarge) 
        {
            enlargeCD -= Time.deltaTime;
            if (enlargeCD <= 0)
            {
                enlargeCD = enlargeCDOG;
                enlarge = false;
            }


        }
        //switch (thisText.text)
        //{
        //    case "9":
        //        enlargeCD -= Time.deltaTime;
        //        if (enlargeCD > .5)
        //        {
        //            EnlargeTextBy(.6f);
        //        }
        //        break;

        //    case "8":
        //        enlargeCD -= Time.deltaTime;
        //        if (enlargeCD > .5)
        //        {
        //            EnlargeTextBy(.6f);
        //        }
        //        break;

        //    case "7":
        //        enlargeCD -= Time.deltaTime;
        //        if (enlargeCD > .5)
        //        {
        //            EnlargeTextBy(.6f);
        //        }
        //        break;

        //}
    }

    public void EnlargeTextBy(float size)
    {
        if (transform.localScale == originalScale)
        {
            enlarge = true;
            transform.localScale = new Vector3(originalScale.x + size, originalScale.y + size, originalScale.z + size);
            if (playSound)
            {
                thisAudio.clip = clip;
                thisAudio.Play();
            }
        }
        
    }
}
