using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonPressShrink : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, ISubmitHandler, IDeselectHandler, ISelectHandler {
    public Vector3 shrinkSize;
    private Vector3 originalSize;
    public AudioSource audSource;
    public AudioClip[] audClips;
    Image img;
    public Color pointerDownColor;
    Color originalColor;
    public float audioSourceVolume; //set this in editor.  some buttons should be lower.
    
	// Use this for initialization
	void Awake () {
        originalSize = transform.localScale;
        img = GetComponent<Image>();
        originalColor = img.color;
        if (GetComponent<AudioSource>() != null)
        {
            audSource = GetComponent<AudioSource>();
        }else
        {
            audSource = gameObject.AddComponent<AudioSource>() as AudioSource; //add an audiosource if there isn't one on there already
        }
        audSource.playOnAwake = false; //otherwise it'll play whatever sound is in there
        if (audioSourceVolume != 0) //if it isn't set in editor, then just keep it full volume.
        {
            audSource.volume = audioSourceVolume;
        }
	}
	
	// Update is called once per frame
	void Update () {
        //since i can't seem to find an opposite to OnSubmit...
        
        if (Input.GetButtonUp("Submit")) //~~this may be an issue once i start testing with sound lol.
        {
            //transform.localScale = originalSize;
            //audSource.clip = audClips[1];
            //img.color = originalColor;
            //if (!GameManager.GM.muteSFX)
            //{
            //    audSource.Play();
            //}

        }
    }
    //for controller support
    public void OnSubmit(BaseEventData data)
    {
        transform.localScale = shrinkSize;
        audSource.clip = audClips[0];//the click in sfx
        img.color = pointerDownColor;
        if (!GameManager.GM.muteSFX)
        {
            audSource.Play();
            if (!GameManager.GM.muteSFX)
            {
                audSource.Play();
            }

        }

    }
    public void OnSelect(BaseEventData data)
    {
        audSource.clip = audClips[1];
        if (!GameManager.GM.muteSFX)
        {
            audSource.Play();
        }
    }
    public void OnDeselect(BaseEventData data)
    {
        transform.localScale = originalSize;
        img.color = originalColor;
    }
    public void OnPointerDown(PointerEventData data)
    {
        transform.localScale = shrinkSize;
        audSource.clip = audClips[0];//the click in sfx
        img.color = pointerDownColor;
        if (!GameManager.GM.muteSFX)
        {
            audSource.Play();
        }

    }

    public void OnPointerUp(PointerEventData data)
    {
        transform.localScale = originalSize;
        audSource.clip = audClips[1];
        img.color = originalColor;
        if (!GameManager.GM.muteSFX)
        {
            audSource.Play();
        }

    }
    public void ShrinkMe()
    {
    }

    public void ResetMySize() //this will be called by GM when panels close.  there's no neat way that i could find to deselect a button via script, so going with this.
    {
        transform.localScale = originalSize;
        //Debug.Log(transform.name);
        img.color = originalColor;

    }
}
