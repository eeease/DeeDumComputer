using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TitleSpriteSwap : MonoBehaviour {
    public Sprite[] landscapeSprites, portraitSprites;
    Image img;
    public float imgSwapTime, imgSwapTimeOG, shrinkSpeed;
    public int index = 0;
    bool shrink;
	// Use this for initialization
	void Start () {
        img = GetComponent<Image>();
        GameManager.GM.titleInt++;
        if (GameManager.GM.titleInt > 1)
        {
            gameObject.SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () {
        
        imgSwapTime -= Time.deltaTime;
        if (imgSwapTime <= 0)
        {
            if (index < landscapeSprites.Length-1)
            {
                index++;
            }else
            {
                index = 0;
            }
            imgSwapTime = imgSwapTimeOG;

        }
        if (!Application.isMobilePlatform) //if you're on a comp/console and playing, just scroll through the landscaped sprites.
        {
            img.sprite = landscapeSprites[index];

        }

        //outside of the timer if so that when device is turned, it automatically switches.
        if (Input.deviceOrientation == DeviceOrientation.Portrait || Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)
        {
            img.sprite = portraitSprites[index];
        }
        else if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft || Input.deviceOrientation == DeviceOrientation.LandscapeRight)
        {
            img.sprite = landscapeSprites[index];
        }

        if(Input.GetMouseButtonDown(0) || Input.GetButtonDown(GameManager.GM.xBoxX+"Jump") || Input.GetButtonDown(GameManager.GM.xBoxX + "Submit") || Input.GetButtonDown(GameManager.GM.xBoxX + "Cancel"))
        {
            shrink = true;
            //highlight the first hub button
            GameObject.Find("HubButton0").GetComponent<Button>().Select();
            //MapLogic.ML.CheckHubPerfects();
        }


        //shrink down to an insignificant, imperceptible grain of unimportance
        if (shrink)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * shrinkSpeed);

        }
        if (transform.localScale.y < 0.05f)
        {
            gameObject.SetActive(false);
        }

    }
}
