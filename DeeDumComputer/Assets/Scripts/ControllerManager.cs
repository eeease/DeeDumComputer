using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControllerManager : MonoBehaviour {
    public static ControllerManager CM;
    public bool playstationController, xboxController, keyboard;
    public string[] currentControllers;

    public string xBoxX;  //this will be empty unless an xbox controller is connected.
    public float controllerCheckTimer = 2;
    public float controllerCheckTimerOG = 2;//after a few seconds of no input, this will countdown and check controllers again. (there's not really any cleaner way to detect disconnects in Unity atm...)
    private int theControllerWas, theControllerIs;//this is a joke (the missile knows where it is...) but also a practical way of telling the controller change panel to turn on.  if the controleris != controllerwas, the panel should turn on.
    private bool firstControllerCheck = true;
    AudioSource controllerNotificationSource;

    void Awake()
    {
        if (CM == null)
        {
            DontDestroyOnLoad(this);
            CM = this;
        }
        else if (CM != this)
        {
            Destroy(gameObject);
        }
    }
    // Use this for initialization
    void Start () {
        ControllerCheck();
        controllerNotificationSource = GetComponent<AudioSource>();
        	
	}
	
	// Update is called once per frame
	void Update () {

        //debug testing the effect of the notificaiton panel:
        if (Input.GetKeyDown(KeyCode.N))
        {
            ShowNotificationPanel();

        }


        //if no input after a couple of seconds, check for a controller:
        if (!Input.GetButtonDown(xBoxX+"Jump") && !Input.GetButtonDown(xBoxX+"Submit"))
        {
            controllerCheckTimer -= Time.deltaTime;
            if (controllerCheckTimer <= 0)
            {
                ControllerCheck();
                controllerCheckTimer = controllerCheckTimerOG;
            }
        }else
        {
            controllerCheckTimer = controllerCheckTimerOG;
        }
		
	}

    public void ControllerCheck()
    {
        System.Array.Clear(currentControllers, 0, currentControllers.Length);
        System.Array.Resize<string>(ref currentControllers, Input.GetJoystickNames().Length);
        int numberOfControllers = 0;

        if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
        {
            if (Input.GetJoystickNames().Length == 0)
            {
                xBoxX = string.Empty;
                keyboard = true;
                theControllerIs = 0;
                playstationController = false;
                xboxController = false;
                //this will set your player script(s) xBoxX to be empty:
                SetPlayersXBox();
            }
            else
            {
                //                Debug.Log (Input.GetJoystickNames () [0]);
                for (int i = 0; i < Input.GetJoystickNames().Length; i++)
                {
                    currentControllers[i] = Input.GetJoystickNames()[i].ToLower();
                    //                    Debug.Log ("name is: " + currentControllers [i] + "and i'm number " + i);
                    //                    Debug.Log ("Number of controllers = " + Input.GetJoystickNames ().Length);
                    //first check if it's blank (keyboard) for short circuit efficiency... yeah.

                    //                    if ((currentControllers [i] == "controller (xbox 360 for windows)" || currentControllers [i] == "controller (xbox 360 wireless receiver for windows)" || currentControllers [i] == "controller (xbox one for windows)") || currentControllers[i] == "microsoft xbox one wired controller") {
                    if (currentControllers[i].Contains("xbox"))
                    {
                        xboxController = true;
                        theControllerIs = 1;

                        xBoxX = "X"; //this is going to be added to all of the input.getbuttons.  if it's blank, it will work with playstation controller and keyboard.
                        SetPlayersXBox();
                        keyboard = false;
                        playstationController = false;
                    }
                    else if (currentControllers[i].Contains ("sony"))
                    {
                        playstationController = true;
                        theControllerIs = 2;
                        xBoxX = string.Empty; //this is going to be added to all of the input.getbuttons.  if it's blank, it will work with playstation controller and keyboard.
                        SetPlayersXBox();
                        keyboard = false;
                        xboxController = false;
                    }
                    else if (currentControllers[i] == "")
                    {
                        numberOfControllers++;

                    }
                }
            }
        }
        else
        {


            //Debug.Log(Input.GetJoystickNames().Length);
            //            Debug.Log (Input.GetJoystickNames ().Length);
            //            Debug.Log (Input.GetJoystickNames () [0].ToLower ());
            for (int i = 0; i < Input.GetJoystickNames().Length; i++)
            {
                currentControllers[i] = Input.GetJoystickNames()[i].ToLower();
                //                Debug.Log ("name is: " + currentControllers [i] + "and i'm number " + i);
                //                Debug.Log ("Number of controllers = " + Input.GetJoystickNames ().Length);
                //first check if it's blank (keyboard) for short circuit efficiency... yeah.

                //                if ((currentControllers [i] == "controller (xbox 360 for windows)" || currentControllers [i] == "controller (xbox 360 wireless receiver for windows)" || currentControllers [i] == "controller (xbox one for windows)") || currentControllers[i] == "microsoft xbox one wired controller") {
                if (currentControllers[i].Contains("xbox"))
                {
                    xboxController = true;
                    theControllerIs = 1;
                    xBoxX = "X";
                    SetPlayersXBox();
                    keyboard = false;
                    playstationController = false;
                }
                else if (currentControllers[i].Contains("wireless controller"))
                {
                    playstationController = true; //not sure if wireless controller is just super generic but that's what DS4 comes up as.
                    theControllerIs = 2;
                    xBoxX = string.Empty; //this is going to be added to all of the input.getbuttons.  if it's blank, it will work with playstation controller and keyboard.
                    SetPlayersXBox();

                    keyboard = false;
                    xboxController = false;
                }
                else if (currentControllers[i] == "")
                {
                    numberOfControllers++;

                }
            }
            if (numberOfControllers == Input.GetJoystickNames().Length)
            {
                keyboard = true;
                theControllerIs = 0;
                xboxController = false;

                playstationController = false;
                xBoxX = string.Empty; //this is going to be added to all of the input.getbuttons.  if it's blank, it will work with playstation controller and keyboard.
                SetPlayersXBox();

            }
        }
        //if the controllernumber is now NOT what the controller was (ex. it is now xbox(1) and was keyboard(0)), then show the panel.

        //don't show the panel on the first check
        if (!firstControllerCheck && (theControllerIs != theControllerWas))
        {
            ShowNotificationPanel();
        }
        theControllerWas = theControllerIs; //shift these so that it doesn't show notification when it sees the same controller twice
        if (firstControllerCheck)
        {
            firstControllerCheck = false;
        }
    }
    public void ShowNotificationPanel()
    {
        //show the notification panel
        GameObject notificationPanel = GameObject.Find("ControllerNotificationPanel");
        notificationPanel.GetComponent<PanelFadeAndDie>().stayTimer = 1;
        switch (theControllerIs)
        {
            case 0:
                notificationPanel.GetComponent<PanelFadeAndDie>().panelText.text = "Controller Disconnected\nUse Keyboard";
                SetEventSystem(false);

                break;

            case 1:
                notificationPanel.GetComponent<PanelFadeAndDie>().panelText.text = "Controller Connected:\nXBox";
                SetEventSystem(true);
                break;

            case 2:
                notificationPanel.GetComponent<PanelFadeAndDie>().panelText.text = "Controller Connected:\nPS4";
                SetEventSystem(false);
                break;
        }
        notificationPanel.GetComponent<PanelFadeAndDie>().thisPanel.canvasRenderer.SetColor(Color.white);//reset its color value (mostly alpha) so that it starts out opaque
        notificationPanel.GetComponent<PanelFadeAndDie>().panelText.canvasRenderer.SetColor(Color.white);//reset its color value (mostly alpha) so that it starts out opaque
        notificationPanel.GetComponent<PanelFadeAndDie>().thisPanel.color = GameManager.GM.colorTwo;//reset its color value (mostly alpha) so that it starts out opaque

        notificationPanel.GetComponent<PanelFadeAndDie>().thisPanel.enabled = true;
        notificationPanel.GetComponent<PanelFadeAndDie>().panelText.enabled = true;
        controllerNotificationSource.Play();
    }

    public void SetPlayersXBox()
    {
        GameManager.GM.xBoxX = xBoxX;
        
        if (GameManager.GM.players[0] != null)
        {
            foreach (GameObject player in GameManager.GM.players)
            {
                player.GetComponent<Player>().xBoxX = xBoxX;
            }
        }

    }

    /// <summary>
    /// if xbox, enter 1
    /// </summary>
    /// <param name="xBoxOrNot"></param>
    public void SetEventSystem(bool xBoxOrNot)
    {
        if (xBoxOrNot)
        {
            GameObject.Find("EventSystem").GetComponent<MyInputModule>().m_HorizontalAxis = "XHorizontal";
            GameObject.Find("EventSystem").GetComponent<MyInputModule>().m_VerticalAxis = "XVertical";
            GameObject.Find("EventSystem").GetComponent<MyInputModule>().m_SubmitButton = "XSubmit";
            GameObject.Find("EventSystem").GetComponent<MyInputModule>().m_CancelButton = "XCancel";
        }else
        {
            GameObject.Find("EventSystem").GetComponent<MyInputModule>().m_HorizontalAxis = "Horizontal";
            GameObject.Find("EventSystem").GetComponent<MyInputModule>().m_VerticalAxis = "Vertical";
            GameObject.Find("EventSystem").GetComponent<MyInputModule>().m_SubmitButton = "Submit";
            GameObject.Find("EventSystem").GetComponent<MyInputModule>().m_CancelButton = "Cancel";

        }

    }

}
