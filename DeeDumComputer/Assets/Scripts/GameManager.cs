using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using UnityEngine.Purchasing;
using System.Runtime.InteropServices;

/*Attributions:
 * 
 * Wrinkled paper texture: <a href="http://www.freepik.com/free-photos-vectors/background">Background vector created by Dotstudio - Freepik.com</a>
 * 
 * 
 * 
 * 
 * 
 * 
 * */
public class GameManager : MonoBehaviour {
    public static GameManager GM;
    [Header("Important Bools")]
    public bool clearPP;
    public bool feedbackBuild;
    public bool demoBuild;
    public bool computerMode;


    [Header("")]
    public int equippedCostume;
    public int currentScene;
    public float maxZoom, zoomSpeed;
    public bool greenGo, purpleGo, zoomOut, zoomedOut, zoomedIn, firstTime, winning, gotCoin; //both need to be true to load the next level
    public float orthoSizeOG; //the original size (for resetting's sake)
    public int heartsRemaining, totalHearts;
    public List<Image> hearts;
    public List<GameObject> players, goals;
    public List<Vector2> playersOGPos;
    public GameObject GOPanel, scrimPanel, lvlCompletePAnel, bottomPanel, quitPanel, oneTouchPanel, joystickControl, oneTouchButton, errorPanel, storePanel,outOfHearts, outOfHearts2, costumePanel;

    Text oneTouchText, activateText, deactivateText, errorText;

    public int numberOfStars;//stars are earned when completing levels.  used to unlock later levels.

    public float camSpeed, camWindowDist;


    public Sprite[] zoomButtons, heartIcons, audioIcons; //0 = enlarge, 1 = shrink //0 = empty, 1 = full


    public AudioSource greenWarp, purpleWarp, respawn, mainMusic; //these will be called from player OnCollision so they only happen once
    public AudioClip[] mainMusicThemes;
    public AudioClip[] SFX; //0 = respawn, 1 = heartRefill
    public bool muteSFX, muteMusic;  
    int mainMusicIndex;

    int colorPaletteIndex;
    public Color colorOne, colorTwo, colorThree; //one = p1; two = p2; three = background

    //Stats vars:
    int levelJumps, totalJumps, levelDeaths, totalDeaths, levelsCompleted, skips, restarts, wantsToBuy;
    float levelTime, totalTime;
    GameObject tapToRefillArrow;

    //endOfLevelStuff:
    public Sprite[] stars;
    Text jumpsText, lvlTimeText, bestJumpsText, accountStarsTextOMap;
    public int[] twoStarQuotas, threeStarQuotas;
    public int twoStarQuota, threeStarQuota;
    public string jumpsThisLevel, timeThisLevel, twoStarJumps, threeStarJumps;
    int endOfLevelTextIndex = 0;
    public string[] levelNames;
    bool inOverworldMap;
    string formerText, formerText2, formerText3;


    //check if user is seeing level for first time or has already completed it:
    public bool[] thisLevelCompleted, coinCollected;
    public int[] starsAwardedThisLevel;
    public int totalAccountStars, numberOfCoins;
    Text totalAccountStarsText, appQuitEnterText; //these are debug texts ~~ should probably be turned off before 'final' build

    //heart refill stuff:
    Text heartRefillText, heartRefillText2,heartRefillTimer;
    public float timeToRefill, timeToRefillOG;
    public DateTime exitAppTime, enterAppTime;
    public bool thirtyMinuteHearts;

    public bool oneHandControls;

    Vector3 greenSpawn, purpleSpawn, greenGoSpawn, purpleGoSpawn, coinSpawn;

    //Grab all lock buttons and check pp for if they're locked or not
    public List<GameObject> lockButtons;

    public int titleInt = 0; //this will be incremented by TitleSpriteSwap when the game opens.  it'll return to 0 when the game is closed.  only want the title screen to show up when you first open it.
    public GameObject creditsPanel;

    public float focusSpeed; //this will control how quickly the swirl moves to the selected button.
    public bool controllerInputDetected;
    public Button mostRecentlySelectedButton; //update this so if a player leaves a level, the focusSwirl (and camera) will be on the hub they last entered.
    public string mostRecentButtonName;
    float startOfLevelTimer = .8f;
    public string xBoxX;

    //finally prefabatizing players/spawn points so i don't have to change them in 75 scenes:
    [Header("Prefabs")]
    public GameObject greenPlayerPrefab;
    public GameObject purplePlayerPrefab;
    public GameObject greenGoPrefab;
    public GameObject purpleGoPrefab;
    public GameObject coinPrefab;

    void Awake()
    {
        if (GM == null)
        {
            DontDestroyOnLoad(this);
            GM = this;
        }
        else if (GM != this)
        {
            Destroy(gameObject);
        }
        //foreach (GameObject lockButt in GameObject.FindGameObjectsWithTag("LockButton"))
        //{
        //    lockButtons.Add(lockButt);
        //}
    }

	// Use this for initialization
	void Start () {
        //set the levelCompletedBoolArray length:
        System.Array.Resize(ref thisLevelCompleted, SceneManager.sceneCountInBuildSettings);
        System.Array.Resize(ref coinCollected, SceneManager.sceneCountInBuildSettings);

        System.Array.Resize(ref starsAwardedThisLevel, SceneManager.sceneCountInBuildSettings);

        System.Array.Resize(ref twoStarQuotas, SceneManager.sceneCountInBuildSettings);
        System.Array.Resize(ref threeStarQuotas, SceneManager.sceneCountInBuildSettings);

        for (int i = 0; i < starsAwardedThisLevel.Length; i++)
        {
            starsAwardedThisLevel[i] = PlayerPrefs.GetInt("starsAwardedThisLevel" +i); //reset all the stars awarded.
        }
        for (int i = 0; i < thisLevelCompleted.Length; i++) //grab levels and coins collected bools from pp
        {
            thisLevelCompleted[i] = PlayerPrefsX.GetBool("thisLevelCompleted" + i);
            coinCollected[i] = PlayerPrefsX.GetBool("thisCoinCollected" + i);
        }
       
        if (clearPP)
        {
            ResetOverallStats();
        }
        if (!feedbackBuild)
        {
            GameObject.Find("FeedBackButton").SetActive(false);
        }
        //GRAB ALL THE OVERALL STATS FROM PP:
        totalTime = PlayerPrefs.GetFloat("totalTime", 0);
        totalDeaths = PlayerPrefs.GetInt("totalDeaths", 0);
        totalJumps = PlayerPrefs.GetInt("totalJumps", 0);
        //wantsToBuy = PlayerPrefs.GetInt("buyHearts", 0);
        levelsCompleted = PlayerPrefs.GetInt("levelsCompleted", 0);
        skips = PlayerPrefs.GetInt("skips", 0);
        restarts = PlayerPrefs.GetInt("restarts", 0);

        ResetLevelStats();
        currentScene = SceneManager.GetActiveScene().buildIndex;
        orthoSizeOG = Camera.main.orthographicSize;
        creditsPanel = GameObject.Find("CreditsPanel");
        GOPanel = GameObject.Find("PausePanel");
        lvlCompletePAnel = GameObject.Find("LevelCompletePanel");
        scrimPanel = GameObject.Find("ScrimPanel");
        //tapToRefillArrow = GameObject.Find("TapToRefillArrow");
        //bottomPanel = GameObject.Find("BottomPanel");
        quitPanel = GameObject.Find("AreYouSurePanel");
        //oneTouchPanel = GameObject.Find("OneTouchPanel");
        //errorPanel = GameObject.Find("PurchaseErrorPanel");
        storePanel = GameObject.Find("StorePanel");
        costumePanel = GameObject.Find("CostumePanel");
        outOfHearts = GameObject.Find("OutOfHeartsText");
        outOfHearts2 = GameObject.Find("OutOfHeartsText2");

        //oneTouchButton = GameObject.Find("OneTouchButton");
        //activateText = GameObject.Find("ActivateText").GetComponent<Text>();
        //deactivateText = GameObject.Find("DeactivateText").GetComponent<Text>();
        //oneTouchText = GameObject.Find("OneTouchText").GetComponent<Text>();
        //errorText = GameObject.Find("ErrorText").GetComponent<Text>();

        creditsPanel.SetActive(false);
        //oneTouchPanel.SetActive(false);
        quitPanel.SetActive(false);
        //tapToRefillArrow.SetActive(false);
        scrimPanel.SetActive(false);
        //grab music and color from pp and set them: (has to happen before turning GOPanel false)
        muteSFX = PlayerPrefsX.GetBool("muteSFX", false);
        muteMusic = PlayerPrefsX.GetBool("muteMusic", false);

        mainMusicIndex = PlayerPrefs.GetInt("PreferredSong", 0);
        mainMusic.clip = mainMusicThemes[mainMusicIndex];
        if (!muteMusic)
        {
            mainMusic.Play();
            GameObject.Find("MusicImage").GetComponent<Image>().sprite = audioIcons[1];

        }
        else
        {
            GameObject.Find("MusicImage").GetComponent<Image>().sprite = audioIcons[0];
        }
        if (muteSFX)
        {
            GameObject.Find("SFXImage").GetComponent<Image>().sprite = audioIcons[0];
        }

        GOPanel.SetActive(false);
        storePanel.SetActive(false);
        costumePanel.SetActive(false);
        //errorPanel.SetActive(false);
        jumpsText = GameObject.Find("CompleteJumps").GetComponent<Text>();
        bestJumpsText = GameObject.Find("BestJumps").GetComponent<Text>();
        lvlTimeText = GameObject.Find("CompleteTime").GetComponent<Text>();
        accountStarsTextOMap = GameObject.Find("AccountStarsTextOM").GetComponent<Text>();
        totalAccountStarsText = GameObject.Find("TotalAccountStarsText").GetComponent<Text>();
        //if (currentScene == 0)
        //{
        //    appQuitEnterText = GameObject.Find("AppQuitEnterText").GetComponent<Text>();
        //}
        //heartRefillText = GameObject.Find("HeartRefillText").GetComponent<Text>();
        //heartRefillText2 = GameObject.Find("HeartRefillText1").GetComponent<Text>();
        //heartRefillTimer = GameObject.Find("HeartRefillTimer").GetComponent<Text>();
        lvlCompletePAnel.SetActive(false);
        zoomedIn = true;
        //GetHearts();
        //sort the list 
        //hearts.Sort((x, y) => x.name.CompareTo(y.name)); //should make it easier to iterate through and take away hearts/add them
        //totalHearts = 5;
        //heartrefill datetime stuff:
        //enterAppTime = DateTime.Now;
        firstTime = PlayerPrefsX.GetBool("firstTime", true);

        if (!firstTime) //if it is the player's first time, don't need to get the exit app time from pp.
        {
            //exitAppTime = Convert.ToDateTime(PlayerPrefs.GetString("exitAppTime"));
        }
        //timeToRefill = PlayerPrefs.GetFloat("timeToRefill");

        if (firstTime) //first time will allow user to start with all hearts.
        {
            //exitAppTime = DateTime.Now;
            //heartsRemaining = totalHearts;
            PlayerPrefsX.SetBool("firstTime", false);
            firstTime = false;
        }
        else
        {
            //heartsRemaining = PlayerPrefs.GetInt("heartsRemaining", 5);
        }
        //joystickControl = GameObject.Find("JoystickBackground"); //actually finding the background image, but turning this on/off should be sufficient.
        //joystickControl.SetActive(false);
        //check if joystick should be on or off
        //oneHandControls = PlayerPrefsX.GetBool("oneHandControls", false);
        //if (!oneHandControls)
        //{
        //    joystickControl.SetActive(false);
        //}else
        //{
        //    joystickControl.SetActive(true);
        //}

        //LoseALife();
        //ResetLives();
        Camera.main.GetComponent<RippleEffect>().enabled = false; //turn off the ripples

        colorPaletteIndex = PlayerPrefs.GetInt("ColorPalette", 0);
        //myEvent = GameObject.Find("Player").GetComponent<Player>().MoveLeft();
        winning = true;
        totalAccountStars = PlayerPrefs.GetInt("totalAccountStars", 0); //~~i wonder if it should start at 1 so player can unlock a level as part of a tutorial or something.  tutorials?  pffffft.
        numberOfCoins = PlayerPrefs.GetInt("totalCoins", 0);
        //LoginHeartsCheck();
        if (currentScene == 0) //if on overworld map at startup (maybe there's a reason you won't be??)
        {
            OverworldMapObjectSetting();
        }
        else
        {
            SetTheColors(colorPaletteIndex);
            //get all the players and store their original positions.
            GetPlayersAndPos();

        }
        if (computerMode)
        {
            totalHearts = 5;
            heartsRemaining = 5;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (lvlCompletePAnel.activeSelf)
        {
            startOfLevelTimer = .8f; //the start of level timer needs to be hard set when lvlcomplete panel is up so that when you click the restart button, it's > 0 and won't register that click as a jump.

        }
        if (startOfLevelTimer >= 0)
        {
            startOfLevelTimer -= Time.deltaTime;
        }
        if (SceneManager.GetActiveScene().buildIndex!=0 && Input.GetButtonDown(xBoxX+"Jump") && !lvlCompletePAnel.activeSelf && !GOPanel.activeSelf)
        {
            if (startOfLevelTimer <= 0 && ((players[0].GetComponent<Player>().onTopOfPlatform || players[0].GetComponent<Player>().onTopOfPlayer) || (players[1].GetComponent<Player>().onTopOfPlatform||players[1].GetComponent<Player>().onTopOfPlayer)))
            {
                LogStats(true, "jumps");
                LogStats(false, "jumps");

            }
        }

        ////timeToRefill -= Time.deltaTime;
        ////if (timeToRefill <= 0) //grant a heart when refill time is up
        ////{
        ////    if (thirtyMinuteHearts)
        ////    {
        ////        PlayerPrefsX.SetBool("thirtyMinuteHearts", false); //turn off the thirty minute hearts switch.
        ////        thirtyMinuteHearts = PlayerPrefsX.GetBool("thirtyMinuteHearts");
        ////        ResetAllLives(); //this has to be after the switch so that the function correctly sprit swaps

        ////    }
        ////    else
        ////    {
        ////        AddAHeart(1);
        ////    }
        ////    timeToRefill = timeToRefillOG;
        ////}
        //Back button on android to pause game:
        if (Input.GetButtonDown(xBoxX + "Cancel"))
        {
            if (storePanel.activeSelf)
            {
                ShowStorePanel(1);
            }
            if (creditsPanel.activeSelf)
            {
                ShowCreditsPanel();
            }else if (!lvlCompletePAnel.activeSelf && !quitPanel.activeSelf)
            {
                ShowGOCanvas();
            }
            if (quitPanel.activeSelf)
            {
                ShowQuitCanvas(0);
            }
        }
        //Do the following while not on the overworld map:
        ////if ((bottomPanel.activeSelf || joystickControl.activeSelf) && inOverworldMap)
        ////{
        ////    bottomPanel.SetActive(false);
        ////    joystickControl.SetActive(false);
        ////}
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            levelTime += Time.deltaTime;//tick up the level time until you beat it, at which point it should be reset.
            totalTime += Time.deltaTime;
            ////if (!bottomPanel.activeSelf && !inOverworldMap)
            ////{
            ////    if (oneHandControls)
            ////    {
            ////        joystickControl.SetActive(true);
            ////    }
            ////    else
            ////    {
            ////        bottomPanel.SetActive(true);
            ////    }
            ////}
           
            //restart support (doesn't need an xbox check (triangle and Y are the same input):
            if (Input.GetButtonDown("QuickRestart"))
            {
                if (NoPanelOpen())
                {
                    RestartLevel();
                }
            }
            if (Input.GetButtonDown(xBoxX+"QuickHome"))
            {
                if (NoPanelOpen())
                {
                    ReturnToMap();
                }
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                //LoadNewScene(currentScene + 1);
            }

            //quick level switch (doesn't need an xbox check (r1 and rb are the same input number):
            if (Input.GetButtonDown("SkipLevel"))
            {
                currentScene = SceneManager.GetActiveScene().buildIndex;
                if (currentScene < SceneManager.sceneCountInBuildSettings - 1 && (currentScene % 5 != 0))
                {
                    ResetLevelStats();
                    LoadNewScene(currentScene + 1);
                }else
                {
                    ReturnToMap();
                }

            }

            
            //debug color set:
            //if (Input.GetKeyDown(KeyCode.C))
            //{
            //    SetTheColors(colorPaletteIndex);
            //}




            //Zooming logic:
            if (zoomOut)
            {
                ZoomOut(zoomSpeed, 30);
            }

            if (greenGo && purpleGo)
            {
                winning = true;
                if (winning)
                {
                    ZoomOut(2, maxZoom);
                    if (!GetComponent<AudioReverbFilter>().enabled)
                    {
                        GetComponent<AudioReverbFilter>().enabled = true;
                    }
                }
            }
            else
            {
                if (GetComponent<AudioReverbFilter>().enabled)
                {
                    GetComponent<AudioReverbFilter>().enabled = false;

                }
                if (Camera.main.GetComponent<RippleEffect>().enabled)
                {
                    Camera.main.GetComponent<RippleEffect>().enabled = false;

                }
                //~~~can't get this camreset section to work properly.  have to think about it more.
                //if (!winning && (purpleGo||greenGo))
                //{
                //    if (zoomedIn)
                //    {
                //        ZoomTrue();
                //    }else
                //    {
                //        ZoomSwap();
                //        ZoomTrue();
                //    }

                //    winning = true;

                //}
            }

            //just make sure you got the players (there's a delay issue going on here where it's trying to get them but they're not there):

            if (players[0] == null)
            {
                GetPlayersAndPos();
                SetTheColors(colorPaletteIndex);

            }
        }
        else
        {
            //OverworldMapObjectSetting();
        }
        ////if (!thirtyMinuteHearts)
        ////{
        ////    heartRefillText.enabled = true;
        ////    if(heartRefillText.GetComponent<Polyglot.LocalizedText>().Key != "MENU_LABEL_NEXT") //set it to read "next" if it isn't already set
        ////    {
        ////        heartRefillText.GetComponent<Polyglot.LocalizedText>().Key = "MENU_LABEL_NEXT";
        ////    }
        ////    if(heartRefillText2.GetComponent<Polyglot.LocalizedText>().Key != "GAME_CHARACTER_HEART")
        ////    {
        ////        heartRefillText2.GetComponent<Polyglot.LocalizedText>().Key = "GAME_CHARACTER_HEART";
        ////    }
        ////}
        ////else
        ////{
        ////    heartRefillText.enabled = false;
            
        ////    if (heartRefillText2.GetComponent<Polyglot.LocalizedText>().Key != "GAME_CHARACTER_UNLIMITED") //if 30 minutes are active, set the first word to say "free"
        ////    {
        ////        heartRefillText2.GetComponent<Polyglot.LocalizedText>().Key = "GAME_CHARACTER_UNLIMITED";
        ////    }
            
        ////    //heartRefillText.text = "Unlimited: " + FormatTime(timeToRefill);
        ////}
        //always update the refill timer
        ////heartRefillTimer.text = ": " + FormatTime(timeToRefill);

    }

    public void RestartLevel()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex; //refresh this number
        if (currentScene != 0)
        {
            greenGo = false;
            purpleGo = false;
            startOfLevelTimer = .8f; //reset the start of level timer (checked by restart jump stuff);
            for (int i = 0; i < players.Count; i++)
            {
                players[i].GetComponent<Player>().jumping = true; //added this for the PC version because the submit and jump button are the same.  was causing a 'restart' then immediate, midair jump.

                players[i].transform.position = playersOGPos[i];
            }
            //LoseALife();
            LogStats(true, "deaths");
            LogStats(false, "deaths");

            LogStats(false, "restarts");
            ResetLevelStats();
            zoomedOut = true;
            zoomedIn = false;
            zoomOut = true; //~~i'm realizing these bools are not well named.
            SpawnCoin();
        }
        //SceneManager.LoadScene(currentScene); //not reloading the scene cause that messes with particle effects and probably a lot of other stuff.  it's just choppy, too.
    }

    public void ZoomOut(float zoomRate, float maxZoom)
    {

        if (zoomedIn)
        {
            Camera.main.orthographicSize = Mathf.SmoothStep(Camera.main.orthographicSize, maxZoom, Time.deltaTime * zoomRate);
            if (Camera.main.orthographicSize >= (maxZoom - 0.5f)) //just to cover my ass.
            {

                zoomOut = false;
                ZoomSwap();

            }
        }
        else if (zoomedOut && (!purpleGo&&!greenGo))
        {
            Camera.main.orthographicSize = Mathf.SmoothStep(Camera.main.orthographicSize, orthoSizeOG, Time.deltaTime * zoomRate);
            if (Camera.main.orthographicSize <= (orthoSizeOG + 0.5f)) //just to cover my ass.
            {
                zoomOut = false;
                ZoomSwap();
            }
        }

        //do this stuff regardless
        currentScene = SceneManager.GetActiveScene().buildIndex; //refresh this number
        if ((purpleGo && greenGo) && !lvlCompletePAnel.activeSelf && !inOverworldMap) //if cubes are on their spots and there's another scene AND the lvlComplete panel is not onscreen
        {
            Camera.main.orthographicSize = Mathf.SmoothStep(Camera.main.orthographicSize, maxZoom, Time.deltaTime * zoomRate);
            if (!Camera.main.GetComponent<RippleEffect>().enabled)
            {
                Camera.main.GetComponent<RippleEffect>().enabled = true;
            }
            
            if (Camera.main.orthographicSize >= (maxZoom - 10f)) //just to cover my ass.
            {
                WriteStatsToFile();
                AwardStars(currentScene); //award stars first, then set the level to having been completed

                PlayerPrefsX.SetBool("thisLevelCompleted" + currentScene, true); //set that the player has completed this level.
                ShowLvLCompleteCanvas();

                ResetLevelStats();
                //LoadNewScene(currentScene + 1);
                //GetHearts();
                //Camera.main.orthographicSize = orthoSizeOG; //reset cam size
                if (zoomedOut)
                {
                    ZoomSwap(); //reset the button
                }
            }

        }
    }

    public void LoadNewScene(int sceneNum)
    {
        SceneManager.LoadScene(sceneNum);

    }
    public void LoadNextScene()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex; //refresh this number
        if (currentScene < SceneManager.sceneCountInBuildSettings -1 && currentScene%5 !=0)
        {
            SceneManager.LoadScene(currentScene + 1);
            greenGo = false;
            purpleGo = false;
            ResetSounds();
            GetPlayersAndPos();



        }
        else if(currentScene%5 == 0)
        {
            ReturnToMap();
        }else
        {
            ShowGOCanvas();
        }
        if (!PlayerPrefsX.GetBool("thisLevelCompleted" + currentScene)) //if you didn't beat this level, that means you skipped this level.
        {
            LogStats(false, "skips");
        }
    }

    public void ResetSounds()
    {
        greenWarp.Stop();
        purpleWarp.Stop();
        //Debug.Log("AudioFilterIsCurrently: " + GetComponent<AudioReverbFilter>().enabled);
    }

    public void ZoomSwap()
    {
        //GameObject zoomIMG = GameObject.Find("ZoomIMG");

        if (zoomedIn)
        {

            zoomedIn = false;
            zoomedOut = true;
            //zoomIMG.GetComponent<Image>().sprite = zoomButtons[1];
        }else if (zoomedOut)
        {
            //zoomIMG.GetComponent<Animation>().Play();

            zoomedOut = false;
            zoomedIn = true;
            //zoomIMG.GetComponent<Image>().sprite = zoomButtons[0];

        }
    }
    public void ZoomTrue()
    {
        //GameObject zoomIMG = GameObject.Find("ZoomIMG");

        if (!purpleGo || !greenGo)
        {
            zoomOut = true;
            //zoomIMG.GetComponent<Animation>().Play();

        }
    }

    //Life losing functionality being commented out for computer version since it's a mobile-only 'feature'
    //public void LoseALife()
    //{
    //    if (heartsRemaining > 0)
    //    {
    //        if (!thirtyMinuteHearts)
    //        {
    //            heartsRemaining--;
    //        }
    //        respawn.clip = SFX[0];
    //        if (!muteSFX)
    //        {
    //            respawn.Play();
    //        }
    //    }else
    //    {
    //        ShowStorePanel(1);
    //        //tapToRefillArrow.SetActive(true);
    //    }
    //    //go through the hearts list and if i is less than the number of remaining hearts, set that sprite to have a full heart.  else set it empty
    //    for(int i=0; i<hearts.Count; i++)
    //    {
    //        if (i < heartsRemaining)
    //        {
    //            if (!thirtyMinuteHearts)
    //            {
    //                hearts[i].sprite = heartIcons[1];
    //            }else
    //            {
    //                hearts[i].sprite = heartIcons[2];
    //            }
    //        }else
    //        {
    //            hearts[i].sprite = heartIcons[0];
    //        }
    //    }
    //    if (heartsRemaining < totalHearts)
    //    {
    //        hearts[heartsRemaining].GetComponent<Animation>().Play("heartBurst");
    //        hearts[heartsRemaining].GetComponentInChildren<ParticleSystem>().Emit(30);
    //    }

    //}

    //public void ResetAllLives()
    //{
    //    heartsRemaining = totalHearts;
    //    for(int i=0; i<hearts.Count; i++)
    //    {
    //        if (!thirtyMinuteHearts)
    //        {
    //            hearts[i].sprite = heartIcons[1]; //put all the hearts back in there
    //        }else
    //        {
    //            hearts[i].sprite = heartIcons[2]; //make 'em gold if thirtyMinutes is true
    //        }
    //        hearts[i].GetComponent<Animation>().Play("heartSwell");
    //    }
    //    respawn.clip = SFX[1];
    //    if (!muteSFX)
    //    {
    //        respawn.Play();
    //    }
    //    if (tapToRefillArrow.activeSelf)
    //    {
    //        tapToRefillArrow.SetActive(false);
    //    }
    //    timeToRefill = timeToRefillOG; //reset this to its limit because the user has all lives.  hooray.
    //    LogStats(false, "buyHearts");
    //}
    //public void Fullfill(string productId)
    //{
        
    //    SIS.IAPManager.PurchaseProduct(productId);

    //    //switch (productId)
    //    //{
    //    //    case "com.dashingcape.deedum.heart30minutes":
    //    //        Debug.Log("unlimited hearts for 30 minutes.  hooray");
    //    //        //ThirtyMinuteHearts();
    //    //            break;

    //    //    case "com.dashingcape.deedum.heartrefill":
    //    //        if (!thirtyMinuteHearts)
    //    //        {
    //    //            ResetAllLives();
    //    //        }
    //    //        else
    //    //        {
    //    //            ShowErrorPanel(1);
    //    //        }
    //    //        break;

    //    //    case "com.dashingcape.deedum.levelpack1":
    //    //        GameObject.Find("LockButton1").GetComponent<LevelPackLockLogic>().UnlockTheHubButtons();

    //    //        break;

    //    //    case "com.dashingcape.deedum.levelpack2":
    //    //        GameObject.Find("LockButton2").GetComponent<LevelPackLockLogic>().UnlockTheHubButtons();

    //    //        break;
    //    //    default:
    //    //        Debug.Log(
    //    //            string.Format(
    //    //                "Unrecognized productId \"{0}\"",
    //    //                productId
    //    //            )
    //    //        );
    //    //        break;
    //    //}
    //}

    //public void ThirtyMinuteHearts()
    //{
        
    //    //PlayerPrefs.SetString("thirtyMinuteHearts", DateTime.Now.ToString());
    //    PlayerPrefsX.SetBool("thirtyMinuteHearts", true);
    //    thirtyMinuteHearts = PlayerPrefsX.GetBool("thirtyMinuteHearts");
    //    ResetAllLives();

    //    timeToRefill = 1800;
        
    //}

    //~~this should maybe be a coroutine so i can delay for a second?  or not a function at all but a timer?
    //this is called at start to check number of hearts remaining and fill them up accordingly.
    //public void ResetLives()
    //{
    //    for (int i = 0; i < totalHearts; i++)
    //    {
    //        if (i < heartsRemaining)
    //        {
    //            if (!thirtyMinuteHearts)
    //            {
    //                hearts[i].sprite = heartIcons[1]; //put all the hearts back in there
    //            }else
    //            {
    //                hearts[i].sprite = heartIcons[2]; //keep 'em gold if you've still got time left.
    //            }
    //            hearts[i].GetComponent<Animation>().Play("heartSwell");
    //        }else
    //        {
    //            hearts[i].sprite = heartIcons[0]; //set the empties
    //            hearts[i].GetComponent<Animation>().Play("heartBurst");

    //        }
    //    }
    //    if (Time.deltaTime > .5f) //this stops it from playing at start.  potentially a bit hacky but... it works.
    //    {
    //        respawn.clip = SFX[1];
    //        if (!muteSFX)
    //        {
    //            respawn.Play();
    //        }
    //    }
    //    ////if (tapToRefillArrow.activeSelf)
    //    ////{
    //    ////    tapToRefillArrow.SetActive(false);
    //    ////}
    //}

    /// <summary>
    /// This will be used in conjunction with the heart refill timer.  add hearts = multiple of 20 minutes between app exit and app enter time.  (ex. exited at 4:00, entered at 4:40, should add 2 hearts)
    /// </summary>
    /// <param name="howMany"></param>
    //public void AddAHeart(int howMany)
    //{
    //    heartsRemaining += howMany; //add the hearts
    //    if(heartsRemaining > totalHearts) //if you've added too many, bring it down to the limit (the limiiiiiiit)
    //    {
    //        heartsRemaining = totalHearts;
    //    }
    //    ResetLives(); //then reset accordingly.
    //    Debug.Log("Added " + howMany + "heart(s).");
    //    if (SceneManager.GetActiveScene().buildIndex == 0)
    //    {
    //        //appQuitEnterText = GameObject.Find("AppQuitEnterText").GetComponent<Text>();
    //        //appQuitEnterText.text += "\nAdded " + howMany + " heart(s).";
    //    }
    //    respawn.clip = SFX[1];
    //    if (!muteSFX)
    //    {
    //        respawn.Play();
    //    }
    //}

    public void GetPlayersAndPos()
    {
        players.Clear();
        playersOGPos.Clear();
        //first spawn them:
        greenSpawn = GameObject.Find("GreenSpawn").transform.position;
        purpleSpawn = GameObject.Find("PurpleSpawn").transform.position;
        Instantiate(greenPlayerPrefab, greenSpawn, Quaternion.identity);
        Instantiate(purplePlayerPrefab, purpleSpawn, Quaternion.identity);
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            players.Add(player);
        }
        players.Sort((x, y) => x.name.CompareTo(y.name)); //have to sort the players first before getting their positions (otherwise they would respawn in opposite positions)
        foreach (GameObject player in players)
        {
            playersOGPos.Add(player.transform.position);
        }
        Camera.main.GetComponent<CameraScript>().player = players[0];
        Camera.main.GetComponent<CameraScript>().player2 = players[1];
        GetComponent<CameraTouchControls>().resetCam = true;
        SpawnGoals();
        SpawnCoin();
        thisLevelCompleted[SceneManager.GetActiveScene().buildIndex] = PlayerPrefsX.GetBool("thisLevelCompleted" + SceneManager.GetActiveScene().buildIndex, false); //update this at the beginning of a level.  If the player has never completed the level, this should come back false
    }

    public void SpawnGoals()
    {
        goals.Clear();
        greenGoSpawn = GameObject.Find("GreenGoSpawn").transform.position;
        purpleGoSpawn = GameObject.Find("PurpleGoSpawn").transform.position;
        Instantiate(greenGoPrefab, greenGoSpawn, Quaternion.identity);
        Instantiate(purpleGoPrefab, purpleGoSpawn, Quaternion.identity);
        foreach (GameObject goal in GameObject.FindGameObjectsWithTag("Goal"))
        {
            goals.Add(goal);
        }
        goals.Sort((x, y) => x.name.CompareTo(y.name)); //have to sort the players first before getting their positions (otherwise they would respawn in opposite positions)
        goals[0].transform.Rotate(Vector3.forward, 90); //make the green one upright

    }

    public void SpawnCoin()
    {
        if (!PlayerPrefsX.GetBool("thisCoinCollected" + GameManager.GM.currentScene)) //if this comes back false, that means the player has never gotten this coin and it should spawn!
        {
            gotCoin = false;
            coinSpawn = GameObject.Find("CoinSpawn").transform.position;
            Instantiate(coinPrefab, coinSpawn, Quaternion.identity);
        }

    }

    //public void GetHearts()
    //{
    //    foreach (GameObject heartGO in GameObject.FindGameObjectsWithTag("HeartIMG"))
    //    {
    //        hearts.Add(heartGO.GetComponent<Image>());
    //    }
    //}

    public void ShowGOCanvas()
    {
        if (!GOPanel.activeSelf && !lvlCompletePAnel.activeSelf)
        {
            GOPanel.SetActive(true);

            scrimPanel.SetActive(true);
            ResetPanelButtons(GOPanel);

            GameObject.Find("LevelText").GetComponent<Text>().text += " - " + SceneManager.GetActiveScene().buildIndex; //just add the number after the word 'level'
            //eventually the following line will need to go in the controllerinput If statement
            GameObject.Find("ResetButtonPause").GetComponent<Button>().Select(); //select the reset button so the swirl defaults to it when menu is opened.
            if (controllerInputDetected)
            {

            }
            //~~the following was to support level names, which are a much bigger pain to do with localization
            //if (levelNames[SceneManager.GetActiveScene().buildIndex] != string.Empty)
            //{
            //    GameObject.Find("LevelText").GetComponent<Text>().text = "Level " + SceneManager.GetActiveScene().buildIndex + " - " + levelNames[SceneManager.GetActiveScene().buildIndex]; //~~later on, this should pull from a list of level names.
            //}else
            //{
            //    GameObject.Find("LevelText").GetComponent<Text>().text = "Level " + SceneManager.GetActiveScene().buildIndex + " - Placeholder Name"; //cause i don't have names for all of the levels yet.
            //}
            //if(SceneManager.GetActiveScene().buildIndex != 0)
            //{
            //    oneTouchButton.SetActive(true);
            //}else
            //{
            //    oneTouchButton.SetActive(false); //if you're on the map screen, disable the one touch option button (don't need to toggle that here)
            //}
        }
        else
        {
            //if the GO panel is active and you're not on the map screen and you have lives, allow to close.  else you can't close it:
            RetreiveFocusSwirl();
            GOPanel.SetActive(false);
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                GameObject.Find(mostRecentButtonName).GetComponent<Button>().Select();
            }

            if (!lvlCompletePAnel.activeSelf)
            {
                scrimPanel.SetActive(false);
              
            }
        }
    }

    /// <summary>
    /// input 0 to show gocanvas when pressing x button
    /// input 1 (or anything, really) to not show gocanvas (when pressing home button)
    /// </summary>
    /// <param name="showOrNo"></param>
    public void ShowQuitCanvas(int showOrNo)
    {
        if (!quitPanel.activeSelf && !lvlCompletePAnel.activeSelf)
        {
            GOPanel.SetActive(false);

            quitPanel.SetActive(true);
            ResetPanelButtons(quitPanel);

            GameObject.Find("QuitButton").GetComponent<Button>().Select();

        }
        else
        {
            RetreiveFocusSwirl();

            quitPanel.SetActive(false);
            if(showOrNo == 0)
            {

                GOPanel.SetActive(true);
                ResetPanelButtons(GOPanel);

                GameObject.Find("QuitButton").GetComponent<Button>().Select();


            }
            else
            {
                scrimPanel.SetActive(false);
                GameObject.Find(mostRecentButtonName).GetComponent<Button>().Select();

            }
        }
    }
    /* //Mobile purchase error function
    //public void ShowErrorPanel(int showOrNo)
    //{
    //    if (!errorPanel.activeSelf)
    //    {
    //        storePanel.SetActive(false);
    //        errorPanel.SetActive(true);
    //        if(showOrNo ==1)
    //        {
    //            errorText.text = "Your 30 minute powerup is still active";
    //            heartRefillTimer.GetComponent<TimePulse>().EnlargeTextBy(1f);
    //        }
    //        else
    //        {
    //            errorText.text = "There was an error processing your purchase.\nPlease try again later.";
    //        }
    //    }
    //    else
    //    {
    //        errorPanel.SetActive(false);
    //        if (showOrNo == 0)
    //        {
    //            storePanel.SetActive(true);

    //        }
    //        else
    //        {
    //            scrimPanel.SetActive(false);

    //        }
    //    }
    //}
    */

    /// <summary>
    /// if int arg != 0, then show the "Out of hearts" text.
    /// </summary>
    /// <param name="showOrNo"></param>
    public void ShowStorePanel(int showOrNo)
    {
        if (!storePanel.activeSelf)
        {
            storePanel.SetActive(true);
            if (showOrNo == 0) //it will return null if 
            {
                outOfHearts.SetActive(false);
                outOfHearts2.SetActive(false);

            }
            else
            {
                outOfHearts.SetActive(true);
                outOfHearts2.SetActive(true);
                GameObject.Find("HeartRefillButton").GetComponent<Button>().Select();

            }

        }
        else
        {
            RetreiveFocusSwirl();
            storePanel.SetActive(false);
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                GameObject.Find(mostRecentButtonName).GetComponent<Button>().Select();
            }

        }
    }

    public void ShowCreditsPanel()
    {
        if (creditsPanel.activeSelf)
        {

            creditsPanel.SetActive(false);
            if (SceneManager.GetActiveScene().buildIndex == 0)//I can't think of a reason why it wouldn't be...
            {
                GameObject.Find(mostRecentButtonName).GetComponent<Button>().Select();

            }
        }
        else
        {

            creditsPanel.SetActive(true);
            ResetPanelButtons(creditsPanel);

            GameObject.Find("CloseCreditsButton").GetComponent<Button>().Select();

        }
    }

    public void ShowCostumePanel()
    {
        if (costumePanel.activeSelf)
        {
            costumePanel.SetActive(false);
            GameObject.Find(mostRecentButtonName).GetComponent<Button>().Select();

        }
        else
        {
            costumePanel.SetActive(true);
        }
    }
    //For mobile only:
    /*public void ShowOneTouchCanvas(int showOrNo)
    //{
    //    if(!oneTouchPanel.activeSelf && !lvlCompletePAnel.activeSelf)
    //    {
    //        GOPanel.SetActive(false);
    //        oneTouchPanel.SetActive(true); //turn on the prompt
    //        if (!oneHandControls)
    //        {
    //            activateText.gameObject.SetActive(true);
    //            deactivateText.gameObject.SetActive(false);
    //            //oneTouchText.text = "Turn on joystick?"; //!!~~this is hardcoded, which is not great for localization.
    //        }else
    //        {
    //            activateText.gameObject.SetActive(false);
    //            deactivateText.gameObject.SetActive(true);

    //            //oneTouchText.text = "Turn off joystick?"; //set the panel to read the correct prompt.  (if the joystick is off, ask to turn it on.  if it's on, vice versa)
    //        }

    //    }
    //    else
    //    {
    //        oneTouchPanel.SetActive(false);  //if it's already open, turn it off
    //        if (showOrNo == 0) //if you click the x cancel button
    //        {
    //            GOPanel.SetActive(true);
    //        }else
    //        {
    //            scrimPanel.SetActive(false);
    //        }
    //    }
    //}

    //public void ToggleOneTouch()
    //{
    //    if (!oneHandControls) //if one hand controls are off
    //    {
    //        bottomPanel.SetActive(false); //turn off the bottom panel and pause menu
    //        joystickControl.SetActive(true); //turn joystick ON
    //        oneHandControls = true;
    //        PlayerPrefsX.SetBool("oneHandControls", true); //also set it in pp so when player restarts app, its default is onehand controls.
    //    }else
    //    {
    //        bottomPanel.SetActive(true);
    //        joystickControl.SetActive(false);
    //        oneHandControls = false;
    //        PlayerPrefsX.SetBool("oneHandControls", false);
    //    }
    //}
    */
    public void ShowLvLCompleteCanvas()
    {
        //grab the star button image
        int sceneNum = SceneManager.GetActiveScene().buildIndex;
        GetStarQuotas(sceneNum); //update the jump quotas
        jumpsThisLevel = levelJumps.ToString();
        timeThisLevel = FormatTime(levelTime);
        thisLevelCompleted[sceneNum] = PlayerPrefsX.GetBool("thisLevelCompleted" + sceneNum); //update the fact that the level was completed

        if (!lvlCompletePAnel.activeSelf && !GOPanel.activeSelf && !inOverworldMap)
        {

            lvlCompletePAnel.SetActive(true);
            scrimPanel.SetActive(true);
            ResetPanelButtons(lvlCompletePAnel); //reset buttons once you go in (cause they might be highlighted/shrunk from when you last clicked them.

            GameObject.Find("NextLevelButton").GetComponent<Button>().Select(); //this is the default button that'll be selected by focusSwirl.
            Image starImage = GameObject.Find("StarsIMG").GetComponent<Image>();
            if(levelJumps< PlayerPrefs.GetInt("bestJumps" + sceneNum)) //if your jumps this level are better than what they were, update the pp with that info.
            {
                PlayerPrefs.SetInt("bestJumps" + sceneNum, levelJumps);
            }
            jumpsText.text += ": " + jumpsThisLevel;
            bestJumpsText.text += ": " + PlayerPrefs.GetInt("bestJumps" + sceneNum); //pull the stored best jumps int from playerprefs
            lvlTimeText.text += ": " + timeThisLevel;

            //also update the star image if you completed the level under the alloted jumps:
            if (PlayerPrefsX.GetBool("thisLevelCompleted" + sceneNum)) //if player has completed this level:
            {
                //~~Also need to set the fact that they've gotten three/two stars into pp here.  Otherwise the player can get 3 stars, retry a level, then get 2 stars on it.
                if ((levelJumps <= threeStarQuota) || starsAwardedThisLevel[sceneNum] == 3)
                {
                    starImage.sprite = stars[3];

                }
                else if ((levelJumps <= twoStarQuota && levelJumps > threeStarQuota) || starsAwardedThisLevel[sceneNum] == 2)
                {
                    starImage.sprite = stars[2];


                }
                else if ((levelJumps > twoStarQuota) || starsAwardedThisLevel[sceneNum] == 1)
                {
                    starImage.sprite = stars[1];


                }
            }
            else
            {
                starImage.sprite = stars[0]; //if the level hasn't yet been completed, set the star image to be empty.

            }
            totalAccountStarsText.text = totalAccountStars.ToString();
            //GameObject.Find("LevelText").GetComponent<Text>().text = "Level " + SceneManager.GetActiveScene().buildIndex + " - Placeholder name"; //~~later on, this should pull from a list of level names.
            accountStarsTextOMap.text = totalAccountStars.ToString();

        }
        else
        {
            //first, reset the text (this was causing localization issues with my hacky temp strings):
            if(endOfLevelTextIndex == 1)
            {
                SwapEndLevelText();
            }
            RetreiveFocusSwirl();

            lvlCompletePAnel.SetActive(false);

            if (!GOPanel.activeSelf)
            {
                scrimPanel.SetActive(false);
            }
        }
    }

    public void AudioMusicOffOn()
    {
        if (mainMusic.isPlaying)
        {
            mainMusic.Pause();
            PlayerPrefsX.SetBool("muteMusic", true);
            muteMusic = true;

            GameObject.Find("MusicImage").GetComponent<Image>().sprite = audioIcons[0];
        }else
        {
            mainMusic.Play();
            PlayerPrefsX.SetBool("muteMusic", false);
            muteMusic = false;
            GameObject.Find("MusicImage").GetComponent<Image>().sprite = audioIcons[1];

        }
    }
    public void AudioSFXOffOn()
    {
        if (!muteSFX)
        {
            PlayerPrefsX.SetBool("muteSFX", true); //set the preferences to mute sfx/music so next login will save that.
            muteSFX = true;

            purpleWarp.enabled = false;
            greenWarp.enabled = false;
            //~moving this to the player.cs. if in menu, ex, GM doesn't have any players in the list.
            //foreach (GameObject player in players)
            //{
            //    player.GetComponent<AudioSource>().enabled = false;
            //}
            GameObject.Find("SFXImage").GetComponent<Image>().sprite = audioIcons[0];

        }
        else
        {
            PlayerPrefsX.SetBool("muteSFX", false); //set the preferences to mute sfx/music so next login will save that.
            muteSFX = false;

            purpleWarp.enabled = true;
            greenWarp.enabled = true;
            //foreach (GameObject player in players)
            //{
            //    player.GetComponent<AudioSource>().enabled = true;
            //}
            GameObject.Find("SFXImage").GetComponent<Image>().sprite = audioIcons[1];


        }


    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ChangeSong(string leftOrRight)
    {
        switch (leftOrRight)
        {
            case "left":
                if (mainMusicIndex > 0)
                {
                    mainMusicIndex--;
                }else
                {
                    mainMusicIndex = mainMusicThemes.Length-1;
                }
                break;

            case "right":
                if (mainMusicIndex < mainMusicThemes.Length-1)
                {
                    mainMusicIndex++;
                }
                else
                {
                    mainMusicIndex = 0;
                }
                break;
        }
        mainMusic.clip = mainMusicThemes[mainMusicIndex];
        mainMusic.Play();
        PlayerPrefs.SetInt("PreferredSong", mainMusicIndex);//set the user's preferred tune so the game starts up with it in the future (cool effect that takes minimal work).
    }

    public void ChangePalette(string leftOrRight)
    {
        //first, set the color Palette index:
        switch (leftOrRight)
        {
            case "left":
                if (colorPaletteIndex > 0)
                {
                    colorPaletteIndex--;
                }else
                {
                    colorPaletteIndex = 3; //~~this needs to be dynamically set.
                }
                break;

            case "right":
                if (colorPaletteIndex < 3)
                {
                    colorPaletteIndex++;
                }else
                {
                    colorPaletteIndex = 0;
                }
                break;
        }
        //store it in pp:
        PlayerPrefs.SetInt("ColorPalette", colorPaletteIndex);

        

        //finally, set all the stuff:

        SetTheColors(colorPaletteIndex);
    }

    public void SetTheColors(int colorPaletteIndex)
    {
        //then, set all of the colors based on the index:
        switch (colorPaletteIndex)
        {
            case 0:
                ColorUtility.TryParseHtmlString("#56B949", out colorOne);
                ColorUtility.TryParseHtmlString("#844D9E", out colorTwo);
                ColorUtility.TryParseHtmlString("#F3A530", out colorThree);

                break;

            case 1:
                ColorUtility.TryParseHtmlString("#367ABD", out colorTwo);
                ColorUtility.TryParseHtmlString("#F9ED3A", out colorOne); //i'm swapping the order here because i screwed it up in the google store images and it looks pretty good
                ColorUtility.TryParseHtmlString("#EE4035", out colorThree);

                break;

            case 2:
                ColorUtility.TryParseHtmlString("#F3A530", out colorOne);
                ColorUtility.TryParseHtmlString("#EC4A94", out colorTwo);
                ColorUtility.TryParseHtmlString("#4CB2D4", out colorThree);

                break;

            case 3:
                ColorUtility.TryParseHtmlString("#30499B", out colorOne);
                ColorUtility.TryParseHtmlString("#88C542", out colorTwo);
                ColorUtility.TryParseHtmlString("#EB7B2D", out colorThree);

                break;
        }
        if (SceneManager.GetActiveScene().buildIndex > 0)
        {
            players[0].GetComponent<SpriteRenderer>().color = colorOne;
            players[1].GetComponent<SpriteRenderer>().color = colorTwo;
            //set the goal to be transparent but the selected color:
            goals[0].GetComponent<SpriteRenderer>().color = new Color(colorOne.r, colorOne.g, colorOne.b, .5f);
            goals[0].GetComponentInChildren<ParticleSystem>().startColor = colorOne;
            //GameObject.Find("greenPlat").GetComponentInChildren<GetParentColor>().UpdateParticleColor(); //update the particles' colors from here since GM is always alive and active.
            goals[1].GetComponent<SpriteRenderer>().color = new Color(colorTwo.r, colorTwo.g, colorTwo.b, .5f);
            goals[1].GetComponentInChildren<ParticleSystem>().startColor = colorTwo;

            //GameObject.Find("purpPlat").GetComponentInChildren<GetParentColor>().UpdateParticleColor(); 
        }
        Camera.main.backgroundColor = colorThree;
        //Debug.Log("Color set: " + colorPaletteIndex);

    }
    public void OpenURL(string whichURL)
    {
//#if !UNITY_EDITOR
        switch (whichURL)
        {

            case "paypal":
              openWindow("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=WARRJF5F2H9KC");

                break;

            case "itch":
                openWindow("https://eeease.itch.io");

                break;

            case "kenney":
                openWindow("https://kenney.nl/assets");
                break;

            case "playtesters":
                openWindow("https://groups.google.com/forum/m/#!forum/dashing-playtesters/join");
                break;

            case "game-mobile-play":
                openWindow("https://tiny.cc/deedumplay");
                break;

            case "game-mobile-ios":
                openWindow("https://tiny.cc/deedumios");

                break;


            case "game-computer":
                openWindow("https://eeease.itch.io/dee-dum/purchase");

                break;
        }
//#endif
    }
    /// <summary>
    /// jumps, deaths, levelsCompleted, skips, restarts, buyHearts
    /// </summary>
    /// <param name="true if level, false if not"></param>
    /// <param name="whichStat"></param>
    public void LogStats(bool levelOrNot, string whichStat)
    {
        if (levelOrNot)
        {
            switch (whichStat)
            {
                case "jumps":
                    //update var, then write to file:
                    levelJumps++;
                    break;

                case "deaths":
                    levelDeaths++;
                    break;

            }
        }
        else
        {
            switch (whichStat)
            {
                case "jumps":
                    totalJumps++;
                    break;

                case "deaths":
                    totalDeaths++;
                    break;

                case "levelsCompleted":
                    levelsCompleted++;
                    break;

                case "skips":
                    skips++;
                    break;

                case "restarts":
                    restarts++;
                    break;

                case "buyHearts":
                    wantsToBuy++;
                    break;
            }
        }
    }

    public void WriteStatsToFile()
    {
        string path = Application.persistentDataPath + "/DeeDumLog" +Application.version+".txt";
        if (!System.IO.File.Exists(path))
        {
            string createText = "Dee Dum Log File - " + Application.version + " " + DateTime.Now.ToShortDateString() + ", " + DateTime.Now.ToShortTimeString() + "\n\n";
            System.IO.File.WriteAllText(path, createText);

        }

        string appendText = "Level " + SceneManager.GetActiveScene().buildIndex + ": " +"\n" + "Jumps: " + levelJumps + "\n" + "Deaths: " + levelDeaths + "\n" + "Level Time: " + levelTime +"\n";
        string totalsAppend = "Totals: " + "Jumps: " + totalJumps + "||" + "Deaths: " + totalDeaths + "|| " + "Levels Completed: " + levelsCompleted + "||" + "Skip: " + skips + "Restarts: " + restarts + "Total Time: " + totalTime + "||" + "Life Refills: " + wantsToBuy + "\n\n";
        System.IO.File.AppendAllText(path, appendText);
        System.IO.File.AppendAllText(path, totalsAppend);

    }
    public void SendEmail()
    {
        string email = "ericmg123@gmail.com";
        string subject = MyEscapeURL("Dee Dum Feedback");
        //string body = MyEscapeURL("Here's feedback");
        string body = MyEscapeURL(System.IO.File.ReadAllText(Application.persistentDataPath + "/DeeDumLog" + Application.version + ".txt"));

        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }

    string MyEscapeURL(string url)
    {
        return WWW.EscapeURL(url).Replace("+", "%20");
    }


    public void ResetLevelStats()
    {
        levelDeaths = 0;
        levelJumps = 0;
        levelTime = 0;
    }

    public void ResetOverallStats()
    {
        totalDeaths = 0;
        totalJumps = 0;
        totalTime = 0;
        wantsToBuy = 0;
        totalAccountStars = 0;
        numberOfCoins = 0;
        mainMusicIndex = 0;
        colorPaletteIndex = 0;
        PlayerPrefs.SetInt("PreferredSong", mainMusicIndex);
        PlayerPrefs.SetInt("ColorPalette", colorPaletteIndex);
        //stats to reset into pp:
        PlayerPrefs.SetInt("totalAccountStars", totalAccountStars);
        PlayerPrefs.SetInt("totalCoins", numberOfCoins);

        PlayerPrefsX.SetBool("firstTime", true);
        PlayerPrefs.SetInt("totalDeaths", totalDeaths);
        PlayerPrefs.SetInt("totalJumps", totalJumps);
        PlayerPrefs.SetFloat("totalTime", totalTime);
        PlayerPrefs.SetInt("buyHearts", wantsToBuy);
        PlayerPrefsX.SetBool("oneHandControls", false);

        //~~also have to reset time difference pps:
        PlayerPrefs.SetFloat("timeToRefill", 1200);
        PlayerPrefs.SetString("exitAppTime", DateTime.Now.ToString());
        PlayerPrefs.SetFloat("timeToRefill", timeToRefillOG);
        for(int i=0; i<starsAwardedThisLevel.Length; i++)
        {
            PlayerPrefs.SetInt("starsAwardedThisLevel" + i, 0); //reset all the stars awarded.
            PlayerPrefsX.SetBool("thisLevelCompleted" + i, false);//reset all the levels being completed to false
            PlayerPrefs.SetInt("bestJumps" + i, 999); //set all the jump vals to 999 (so most user results will be less than that)
            PlayerPrefsX.SetBool("thisCoinCollected" + i, false); //reset all the coins
        }
        PlayerPrefsX.SetBool("thirtyMinuteHearts", false);
        //reset all the lockbuttons to being locked:
        //foreach(GameObject lockButt in lockButtons)
        //{
        //    //Debug.Log(lockButt.name);
        //    PlayerPrefsX.SetBool(lockButt.name + "locked", true);
        //}
        PlayerPrefsX.SetBool("muteMusic", false);
        PlayerPrefsX.SetBool("muteSFX", false);
        
    }

    public void GetStarQuotas(int sceneNumber)
    {
        //check the arrays for the quotas at the fed in scene number"
        twoStarQuota = twoStarQuotas[sceneNumber-1]; //-1 because i added the overworld map as scene 0.  this is the most efficient way to deal with that issue.
        threeStarQuota = threeStarQuotas[sceneNumber-1];
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("totalTime", totalTime); //store the total time played so it can pull that number on startup and not start at 0 every time.
        //store overall stats into pp:
        PlayerPrefs.SetInt("totalDeaths", totalDeaths);
        PlayerPrefs.SetInt("totalJumps", totalJumps);
        //PlayerPrefs.SetInt("buyHearts", wantsToBuy);
        PlayerPrefs.SetInt("levelsCompleted", levelsCompleted);
        PlayerPrefs.SetInt("skips", skips);
        PlayerPrefs.SetInt("restarts", restarts);
        //PlayerPrefs.SetInt("heartsRemaining", heartsRemaining); //store hearts remaining so we know how much the player left the game with.
        exitAppTime = DateTime.Now;
        //Notification notification = new Notification("hud_heartFull", "Full hearts!", "Help us home!");
        //if ((totalHearts-heartsRemaining)>0) //don't notify if you have full hearts on quit
        //{
        //    NotificationManager.ShowNotification(2, notification, SetNotification(totalHearts-heartsRemaining));
        //}
        PlayerPrefs.SetString("exitAppTime", exitAppTime.ToString());
        PlayerPrefs.SetFloat("timeToRefill", timeToRefill);
        PlayerPrefsX.SetBool("firstTime", false);

    }

    //~~!!Adding this for Android support.  should probably use the hashes if android player, but android doesn't always call OnApplicationQuit, even if you... quit the application.  great.
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            PlayerPrefs.SetFloat("totalTime", totalTime); //store the total time played so it can pull that number on startup and not start at 0 every time.
            //store overall stats into pp:
            PlayerPrefs.SetInt("totalDeaths", totalDeaths);
            PlayerPrefs.SetInt("totalJumps", totalJumps);
            PlayerPrefs.SetInt("buyHearts", wantsToBuy);
            PlayerPrefs.SetInt("levelsCompleted", levelsCompleted);
            PlayerPrefs.SetInt("skips", skips);
            PlayerPrefs.SetInt("restarts", restarts);
            //PlayerPrefs.SetInt("heartsRemaining", heartsRemaining); //store hearts remaining so we know how much the player left the game with.
            exitAppTime = DateTime.Now;
            Notification notification = new Notification("hud_heartFull", "Full hearts!", "Help us home!");
            //if ((totalHearts - heartsRemaining) > 0) //don't notify if you have full hearts on quit
            //{
            //    NotificationManager.ShowNotification(2, notification, SetNotification(totalHearts - heartsRemaining));
            //}
            PlayerPrefs.SetString("exitAppTime", exitAppTime.ToString());
            PlayerPrefs.SetFloat("timeToRefill", timeToRefill);
            PlayerPrefsX.SetBool("firstTime", false);
        }
        //~~!!may have to add if (!pauseStatus){LoginHeartCheck()} to check the time difference since being paused.
    }


    public void ReturnToMap() //transport the user back to the overworld map:
    {
        // Debug.Log("Level " + SceneManager.GetActiveScene().buildIndex + " is " +PlayerPrefsX.GetBool("thisLevelCompleted" + SceneManager.GetActiveScene().buildIndex));
        //if ((SceneManager.GetActiveScene().buildIndex !=0) && (!PlayerPrefsX.GetBool("thisLevelCompleted" + SceneManager.GetActiveScene().buildIndex))) //if you haven't beaten the level but you're going back to map, you lose a life.  (no need for notification (see: Cookie Cats Pop)).
        //{
        //    LoseALife();
        //}
        RetreiveFocusSwirl();
        SceneManager.LoadScene(0);
        OverworldMapObjectSetting();
        ResetLevelStats();
    }


    public void SwapEndLevelText()
    {
        if (lvlCompletePAnel.activeSelf)
        {

            if (endOfLevelTextIndex==0)
            {
                string localizedStars = GameObject.Find("TotalAccountStarsTextJUSTSTARS").GetComponent<Text>().text;
                string[] localizedJumps = jumpsText.text.Split(":"[0]); //split the string at the colon in order to just take the localized jumps word
                //Debug.Log("full: " + jumpsText.text);
                //Debug.Log("split: " + localizedJumps[0]);
                twoStarJumps = "2 " + localizedStars + ": " + twoStarQuota + " " + localizedJumps[0];
                threeStarJumps = "3" + localizedStars + ": " + threeStarQuota + " " + localizedJumps[0] ;

                formerText = jumpsText.text; //store this before switching so we can switch back to it (this whole function got messy with localization)
                jumpsText.text = twoStarJumps;
                formerText3 = bestJumpsText.text;

                bestJumpsText.text = string.Empty;
                formerText2 = lvlTimeText.text;
                lvlTimeText.text = threeStarJumps;
                endOfLevelTextIndex++;
            }else if (endOfLevelTextIndex ==1)
            {
                jumpsText.text = formerText;
                bestJumpsText.text += ": " + PlayerPrefs.GetInt("bestJumps" + SceneManager.GetActiveScene().buildIndex);
                lvlTimeText.text = formerText2;
                bestJumpsText.text = formerText3;
                endOfLevelTextIndex--;
            }
        }
    }

    public void AwardStars(int whichScene)
    {
        int tempStarsToAward = 0;
        GetStarQuotas(whichScene);
        starsAwardedThisLevel[whichScene] = PlayerPrefs.GetInt("starsAwardedThisLevel" + whichScene); //grab the stars awarded from pp (set in ShowLVLCompleteCanvas)
        totalAccountStars = PlayerPrefs.GetInt("totalAccountStars");

        //how many stars are we going to award?
        if (levelJumps <= threeStarQuota)
        {
            tempStarsToAward = 3;
        }
        else if (levelJumps <= twoStarQuota && levelJumps > threeStarQuota)
        {
            tempStarsToAward = 2;
        }
        else if (levelJumps > twoStarQuota)
        {
            tempStarsToAward = 1;
        }

        if (tempStarsToAward > starsAwardedThisLevel[whichScene]) //only set the pp if you got more stars than what was previously set in there (ex. if you had 2 stars, retried, then only got one, do NOT set the pp to 1).
        {
            switch (tempStarsToAward)
            {
                case 3:
                    PlayerPrefs.SetInt("starsAwardedThisLevel" + whichScene, 3);

                    break;

                case 2:
                    PlayerPrefs.SetInt("starsAwardedThisLevel" + whichScene, 2);

                    break;

                case 1:
                    PlayerPrefs.SetInt("starsAwardedThisLevel" + whichScene, 1);

                    break;
            }
        }

        //has the level been completed before this?  if so, only award the difference.
        if (thisLevelCompleted[whichScene])
        {

            if (starsAwardedThisLevel[whichScene] < tempStarsToAward) //if the stars you have already gotten in this level > the stars you're about to get, then you get the difference.  (ex. you have gotten 2 stars, then you got 3, so you really overall only get 1 more)
            {
                tempStarsToAward -= starsAwardedThisLevel[whichScene];
            }
            else
            {
                tempStarsToAward = 0; //otherwise (it's equal or you've gotten more before than you just did), you get 0.
            }
        }

        totalAccountStars += tempStarsToAward;

        if (gotCoin)
        {
            PlayerPrefsX.SetBool("thisCoinCollected" + GameManager.GM.currentScene, true); //set that the player has collected this coin.
            numberOfCoins++;
            PlayerPrefs.SetInt("totalCoins", numberOfCoins); //also set the number of coins when completing a level.

            gotCoin = false;
        }
        PlayerPrefs.SetInt("totalAccountStars", totalAccountStars);
    }

    /// <summary>
    /// pass in the number of hearts missing.  this will multiply 20mins by number of hearts missing and notify the user accordingly.
    /// ex (missing 3 hearts, notify in 60 minutes of full hearts)
    /// </summary>
    /// <param name="numOfHeartsGone"></param>
    //public DateTime SetNotification(int numOfHeartsGone) //~!this is set for android only atm.  need to include iOS support
    //{
    //    DateTime notificationDelay = DateTime.Now.AddSeconds(1200 * numOfHeartsGone);
    //    return notificationDelay;
    //}

    public string FormatTime(float timeToFormat) //doing this enough that i should make it a function.  call it to format a time.
    {
        int fminutes = Mathf.FloorToInt(timeToFormat / 60f);
        int fseconds = Mathf.FloorToInt(timeToFormat - fminutes * 60);
        string formattedTime = string.Format("{0:0}:{1:00}", fminutes, fseconds);

        return formattedTime;
    }

    //all of this seems to work and i am flabbergasted that i was able to do this.  it's for mobile only, so commented out in computer version
    //public void LoginHeartsCheck()
    //{
    //    //check if thirty minutes are still active:
    //    thirtyMinuteHearts = PlayerPrefsX.GetBool("thirtyMinuteHearts");
    //    TimeSpan difference = enterAppTime.Subtract(exitAppTime);
    //    //Debug.Log("Difference in time (seconds): " + enterAppTime + " - " + exitAppTime + " = " + difference.TotalSeconds);
    //    float subtractFromRefill = (float)(difference.TotalSeconds);
    //    if (SceneManager.GetActiveScene().buildIndex == 0) //only update this debug text if it's first scene.
    //    {
    //        //appQuitEnterText.text = "App enter: " + enterAppTime + "\n" + "App exit: " + exitAppTime + "\n" + "Difference(secs): " + difference.TotalSeconds;
    //    }
    //    //Debug.Log("thirtyMinuteHearts from PP = " + thirtyMinuteHearts);

    //    if (thirtyMinuteHearts)  //if it is still active, tick down the time since it was purchased.
    //    {
    //        timeToRefill -= subtractFromRefill;

    //    }else
    //    //there's probably a neater way of checking this, but...
    //    if (difference.TotalSeconds >= (80 * 60) + timeToRefill)
    //    {
    //        ResetAllLives();
    //    }
    //    else if (difference.TotalSeconds >= (60 * 60) + timeToRefill)
    //    {
    //        AddAHeart(4);
    //        timeToRefill = timeToRefillOG - subtractFromRefill;

    //    }
    //    else if (difference.TotalSeconds >= (40 * 60) + timeToRefill)
    //    {
    //        AddAHeart(3);
    //        timeToRefill = timeToRefillOG - subtractFromRefill;

    //    }
    //    else if (difference.TotalSeconds >= (20 * 60) + timeToRefill)
    //    {
    //        AddAHeart(2);
    //        timeToRefill = timeToRefillOG - subtractFromRefill;

    //    }
    //    else if (difference.TotalSeconds >= timeToRefill) //this should check if the difference is greater than the time to refill when exiting app (ex. you left app with 5 minutes left to refill.  if you come back 6 minutes later, this should fire)
    //    {
    //        AddAHeart(1);
    //        timeToRefill = timeToRefillOG - subtractFromRefill;
    //    }
    //    else //if it doesn't fit into any of those checks, still reduce the wait time accordingly:
    //    {
    //        timeToRefill -= subtractFromRefill;
    //    }
    //    //should also adjust timeToRefill:

    //}

    public void OverworldMapObjectSetting()
    {
        inOverworldMap = true;
        
        greenGo = false;
        purpleGo = false;
        greenWarp.Stop();
        purpleWarp.Stop();
        accountStarsTextOMap.text = totalAccountStars.ToString();

        if (GOPanel.activeSelf)
        {
            GOPanel.SetActive(false);
        }
        if (lvlCompletePAnel.activeSelf)
        {
            lvlCompletePAnel.SetActive(false);
        }
        if (scrimPanel.activeSelf)
        {
            scrimPanel.SetActive(false);
        }
        if (quitPanel.activeSelf)
        {
            quitPanel.SetActive(false);
        }
        Camera.main.GetComponent<RippleEffect>().enabled = false;
        GetComponent<CameraTouchControls>().resetCam = true; //also reset the cam's position (should be swirlPos) when returning to map
    }

    public void OverWorldMapFalse()
    {
        inOverworldMap = false;
    }

    public void RetreiveFocusSwirl()
    {
        //this is inconsistent with the other ways i'm finding the focus swirl (usually i do it by name).  that's because i have a bad feeling about doing it by name and may have to prefabitize the focus swirl.  we'll see.
        GameObject focusSwirl = GameObject.FindGameObjectWithTag("FocusSwirl");
        focusSwirl.transform.SetParent(gameObject.transform);

    }
    public bool NoPanelOpen()
    {
        if(!GOPanel.activeSelf && !lvlCompletePAnel.activeSelf && !quitPanel.activeSelf)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ResetPanelButtons(GameObject whatPanel)
    {
        foreach (Button canvasButton in whatPanel.GetComponentsInChildren<Button>())
        {
            if (canvasButton.GetComponent<ButtonPressShrink>() != null)
            {
                canvasButton.GetComponent<ButtonPressShrink>().ResetMySize();
            }
        }

    }

    public void ChangeEquippedCostume()
    {
        equippedCostume = GameObject.Find("CostumeDropdown").GetComponent<Dropdown>().value;
    }

  

    //adding a delegate or something to check for when scene was loaded.
    //from http://answers.unity3d.com/questions/1174255/since-onlevelwasloaded-is-deprecated-in-540b15-wha.html
    //void OnEnable()
    //{
    //    SceneManager.sceneLoaded += OnLevelFinishedLoading;
    //}
    //void OnDisable()
    //{
    //    SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    //}

    //void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    //{
    //    Debug.Log("Level Loaded");
    //    if (mostRecentButtonName != String.Empty)
    //    {
    //        Debug.Log(mostRecentButtonName);
    //        GameObject mrsButton = GameObject.Find(mostRecentButtonName);
    //        Debug.Log("mrs button = " + mrsButton.name);
    //        mostRecentlySelectedButton = mrsButton.GetComponent<Button>();
    //        mostRecentlySelectedButton.Select(); //select the most recently selected button so that the swirl moves to it.
    //    }

    //}
    [DllImport("__Internal")]
    private static extern void openWindow(string url);
}
