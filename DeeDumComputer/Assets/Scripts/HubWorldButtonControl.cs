using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HubWorldButtonControl : MonoBehaviour {
    public List<GameObject> subLevelButtons, starsEarnedList;
    public float slideSpeed;
    public bool locked, setInactive, checkStars;
    public int starsToUnlock;
    public Animation starLockAnim;
    public AudioClip[] hubButtonFX; //0=backtohub; 1=outofhub; 2=locked; 3=unlock
    AudioSource hubButtonAudio;
    public float starJiggleTimer, starJiggleTimerOG, setInactiveTimer;
    public int perfectCheck; //if this hits 5, change the hub button sprite to gold.
    public Sprite[] hubSprites;
    public float canClickTimer, canClickTimerOG; //this is to avoid Submit input spam, which was causing problems with controller/spacebar input.

	// Use this for initialization
	void Start () {
        //Debug.Log("Running star start");
        hubButtonAudio = GetComponent<AudioSource>();
        foreach (Button button in GetComponentsInChildren<Button>())
        {
            if (button.gameObject.name.Contains("LevelButton")) //grab all the sublevel buttons
            {
                subLevelButtons.Add(button.gameObject);
                button.gameObject.SetActive(false); //turn them off
            }
        }
        subLevelButtons.Sort((x, y) => x.name.CompareTo(y.name));

        for (int i=0; i<subLevelButtons.Count; i++)
        {
            foreach(Image img in subLevelButtons[i].GetComponentsInChildren<Image>())
            {
                if (img.name.Contains( "StarsEarned"))
                {
                    MapLogic.ML.subLevelStars.Add(img); //add all of the images to a list
                    starsEarnedList.Add(img.gameObject); //add the GOs to a list of starsEarned Images
                }
            }
        }
        //grab the star locked animation and also the number of stars needed to unlock the hub:
        foreach (Transform anim in GetComponentsInChildren<Transform>())
        {
            if(anim.name == "StarLockedButton")
            {
                starLockAnim = anim.GetComponent<Animation>();
            }
        }
        //!!This pp check needs to be in between getting the starlockanim and setting the starsToUnlock.
        if (GameManager.GM.clearPP)
        {
            PlayerPrefs.SetInt(gameObject.name + "starsToUnlock", Convert.ToInt32(starLockAnim.gameObject.GetComponentInChildren<Text>().text));

        }
        SetStarsToUnlock();

        if (starsToUnlock > 0)
        {
            locked = true;
        }else
        {
            locked = false;
            starLockAnim.gameObject.SetActive(false);
            locked = false;
            starsToUnlock = 0;

            //UnlockHub(); //this should destroy the star image on start if it's already unlocked? //yes, but the way things have changed, this would play the unlock particles and sound effect each start, which is unnecessary.
        }

        //~~~The following code is for setting the hub buttons gold for perfecting all the levels.  Not working. !!

        //~~the issue here is that each button is running through its entire Start(), meaning it's adding its sub levels then running CheckEarnedStars(), which should only be run once at the end.
        canClickTimer = canClickTimerOG;

    }

    // Update is called once per frame
    void Update()
    {
        if (locked)
        {
            GetComponent<Button>().interactable = false;
        }else
        {
            GetComponent<Button>().interactable = true;
        }
        starJiggleTimer -= Time.deltaTime;
        if (starJiggleTimer <= 0)
        {
            starLockAnim.Play();
            starJiggleTimer = starJiggleTimerOG;
        }
        if (setInactive)
        {
            setInactiveTimer -= Time.deltaTime;
            if (setInactiveTimer <= 0)
            {
                starLockAnim.gameObject.SetActive(false);
                locked = false;
                starsToUnlock = 0;
                PlayerPrefs.SetInt(gameObject.name + "starsToUnlock", starsToUnlock);
                setInactive = false;
            }
        }
        //if (checkStars)
        //{
        //    SetStarsToUnlock();
        //    checkStars = false;
        //}
        canClickTimer -= Time.deltaTime;
    }

    public void ClickToOpenLevels()
    {
        MapLogic.ML.CheckEarnedStars(); //update the number of earned stars when you open up the sub levels //moved this functionalityto ML so it can have all sublevel images in one list (instead of having several lists of 5
        if (canClickTimer <= 0)
        {
            foreach (GameObject button in subLevelButtons)
            {
                if (button.activeSelf)
                {
                    hubButtonAudio.clip = hubButtonFX[0];
                    if (!GameManager.GM.muteSFX)
                    {
                        hubButtonAudio.Play();
                    }
                    button.GetComponent<SubButtonMove>().ResetDelays();
                    button.GetComponent<SubButtonMove>().returnToHub = true;
                }
                else
                {

                    hubButtonAudio.clip = hubButtonFX[1];
                    if (!GameManager.GM.muteSFX)
                    {
                        hubButtonAudio.Play();
                    }
                    button.GetComponent<SubButtonMove>().ResetDelays();
                    button.GetComponent<SubButtonMove>().returnToHub = false;
                    button.SetActive(true);
                }
            }
            canClickTimer = canClickTimerOG;
        }
    }

    public void UnlockHub()
    {
        if (!GameManager.GM.demoBuild)
        {
            if (GameManager.GM.totalAccountStars >= starsToUnlock)
            {
                starLockAnim.gameObject.GetComponentInChildren<ParticleSystem>().Emit(30); //emit the particles from the childed system
                setInactiveTimer = starLockAnim.gameObject.GetComponentInChildren<ParticleSystem>().startLifetime;
                //Select the hub button instead of the star, which will auto-move the focusSwirl:
                GetComponent<Button>().Select();

                //make it inactive after a few seconds (allow particle effects to do their thing
                setInactive = true;

                if (gameObject.name != "HubButton0")
                {
                    hubButtonAudio.clip = hubButtonFX[3];
                    if (!GameManager.GM.muteSFX)
                    {
                        hubButtonAudio.Play();
                    }
                }
            }
            else
            {
                StarLockedJiggle();
            }
        }else
        {
            GameManager.GM.ShowStorePanel(5);
        }
    }
    public void StarLockedJiggle()
    {
        hubButtonAudio.clip = hubButtonFX[2];
        if (!GameManager.GM.muteSFX)
        {
            hubButtonAudio.Play();
        }
        starLockAnim.Play();
        GameObject.Find("AccountStarsTextOM").GetComponent<TimePulse>().EnlargeTextBy(1f);
    }
    public void SetStarsToUnlock() //this will be called by GM when you go back to overworld map to ensure that the hubs stay unlocked within play session
    {
        starsToUnlock = PlayerPrefs.GetInt(gameObject.name + "starsToUnlock", Convert.ToInt32(starLockAnim.gameObject.GetComponentInChildren<Text>().text)); //grab the starsToUnlock from PP.  if it's not there, grab it from the star's text.

    }

    public void PerfectCheck()
    {
        //MapLogic.ML.CheckEarnedStars(); //update earned stars before checking if they're 3-star sprites or not
        perfectCheck = 0; //start it at zero so it doesn't increment each time you open the game (only one level perfected * 5 = 5 levels perfected, = gold, = no good)
        foreach (GameObject img in starsEarnedList) //check each sub level to see if it has been 3-starred
        {
            if (img.GetComponent<Image>().sprite == GameManager.GM.stars[3])
            {
                //Debug.Log(img.transform.parent.name + " " + img.GetComponent<Image>().sprite);

                perfectCheck++;
            }
        }
        if (perfectCheck == 5) //if they were all 3-starred, go ahead and turn the hub button gold to signify that hey, user, you did it.  you perfected all the levels.
        {
            GetComponent<Image>().sprite = hubSprites[1];
        }
        else
        {
            //Debug.Log(perfectCheck + " are perfected");
            GetComponent<Image>().sprite = hubSprites[0];
        }

    }
}
