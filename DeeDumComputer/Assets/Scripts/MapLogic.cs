using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapLogic : MonoBehaviour {
    public List<Image> subLevelStars;
    public List<GameObject> HubLevelButtons; //tbh i'm not sure if i already have a list of these somewhere... oh well, it kind of fits here.

    public static MapLogic ML;
    void Awake()
    {
        if (ML == null)
        {

            ML = this;
        }
        else if (ML != this)
        {
            Destroy(gameObject);
        }
    }


    // Use this for initialization
    void Start () {
		foreach(GameObject hb in GameObject.FindGameObjectsWithTag("HubButton"))
        {
            HubLevelButtons.Add(hb);
        }
        CheckHubPerfects();
	}
	
	// Update is called once per frame
	void Update () {
        //checking this here because checking from the GM was finicky
        if (Camera.main.GetComponent<RippleEffect>().enabled)
        {
            Camera.main.GetComponent<RippleEffect>().enabled = false;
        }
	}
    public void LoadNewScene(int sceneNum)
    {
        ////if (GameManager.GM.storePanel.activeSelf)//if, for some reason, the store is open when you try to load a level (it can be)
        ////{
        ////    GameManager.GM.ShowStorePanel(0);

        ////}
        //first retreive the focusSwirl so it doesn't get lost in transition:
        GameManager.GM.RetreiveFocusSwirl();
        //~~should this check if the level is unlocked?

            SceneManager.LoadScene(sceneNum);
    }

    public void CheckEarnedStars()
    {
        //Debug.Log("there are " + subLevelStars.Count + " in subLevevlStars");
        subLevelStars.Sort((x, y) => x.transform.parent.name.CompareTo(y.transform.parent.name)); //sort them all first
        for (int i = 0; i < subLevelStars.Count; i++)
        {
            subLevelStars[i].sprite = GameManager.GM.stars[PlayerPrefs.GetInt("starsAwardedThisLevel" + (i + 1))]; //sublevelbutton 0 = stars[level 1 (0+1)]
        }
    }

    public void CheckIfUnlockable()
    {

    }

    public void CheckHubPerfects()
    {
        CheckEarnedStars();
        if (HubLevelButtons[0] == null)
        {
            HubLevelButtons.Clear();
            foreach (GameObject hb in GameObject.FindGameObjectsWithTag("HubButton"))
            {
                HubLevelButtons.Add(hb);
            }

        }
        foreach (GameObject hwbc in HubLevelButtons)
        {
            hwbc.GetComponent<HubWorldButtonControl>().PerfectCheck();
        }
    }


}
