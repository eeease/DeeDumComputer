using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;


public class CameraTouchControls : MonoBehaviour
{
    public float speed = 0.1f;
    public float panSpeed;
    public float pinchZoomSpeed = 0.5f;
    public float easeSpeed, camResetTimer, camResetTimerOG;
    public float minX, maxX, minY, maxY, maxZoomOut, minZoomIn;
    public bool easeW, easeE, easeN, easeS, resetCam, easeToClosest;
    bool panRight, panLeft;
    public GameObject[] columns;
    public Vector3 camPos, camPosOG, camLerpBackToPos;
    Vector3 closestColumn; //I think I'm going to go with this - When you end the touchphase, the camera will find the closest column and lerp to that.
    //GameObject scrollRightButt, scrollLeftButt; //took these out after adding the camera lerping to swirl support
    float xboxZoom;
    // Use this for initialization
    void Start()
    {
        columns = GameObject.FindGameObjectsWithTag("Column");
        foreach(GameObject animal in GameObject.FindGameObjectsWithTag("Animal"))
        {
            animal.GetComponent<EnvironAnimalsBehavior>().ChangeAnchoredPos(); //call each animal's anchor find function
        }
        camPosOG = Camera.main.transform.position;
        camPos = Camera.main.transform.position;
        //scrollRightButt = GameObject.Find("ScrollRightButton");
        //scrollLeftButt = GameObject.Find("ScrollLeftButton");
        //scrollRightButt.transform.localScale = new Vector3(scrollRightButt.transform.localScale.x, Screen.height, scrollRightButt.transform.localScale.z);
        //scrollLeftButt.transform.localScale = new Vector3(scrollLeftButt.transform.localScale.x, Screen.height, scrollLeftButt.transform.localScale.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            camResetTimer -= Time.deltaTime;
            if (camResetTimer <= 0)
            {
                resetCam = true;
            }
            if (panRight)
            {
                Camera.main.transform.localPosition += new Vector3(panSpeed, 0, 0);
                camResetTimer = camResetTimerOG;
                //if you're panning right and you go farther than the maxX value, stop panning right.
                if (Camera.main.transform.localPosition.x > maxX)
                {
                    Camera.main.transform.localPosition = new Vector3(maxX, Camera.main.transform.localPosition.y, Camera.main.transform.localPosition.z);
                }

            }
            if (panLeft)
            {
                Camera.main.transform.localPosition -= new Vector3(panSpeed, 0, 0);
                camResetTimer = camResetTimerOG;
                //if you're panning left and you go farther than the minX value, stop panning left.
                if (Camera.main.transform.localPosition.x < minX)
                {
                    Camera.main.transform.localPosition = new Vector3(minX, Camera.main.transform.localPosition.y, Camera.main.transform.localPosition.z);
                }


            }
            //~~this works surprisingly well but should probably check for controller input first.  If GameManager.GM.controllerInput, then yes, it should follow the swirl.
            if (!GameManager.GM.GOPanel.activeSelf && !GameManager.GM.lvlCompletePAnel.activeSelf && !GameManager.GM.creditsPanel.activeSelf && !GameManager.GM.quitPanel.activeSelf && !GameManager.GM.storePanel.activeSelf && !panLeft && !panRight)
            {
                Camera.main.transform.position = Vector3.Lerp(camPos, Camera.main.GetComponent<CameraScript>().GetMidpoint(), easeSpeed * Time.deltaTime); //follow the swirl?
            }

        }
        xboxZoom = Input.GetAxis(GameManager.GM.xBoxX +"ZoomIn"); //this is going to read for LT and RT on an xbox controller.
       
        camPos = Camera.main.transform.position;

        if (!resetCam)
        {

            if (easeS)
            {
                Camera.main.transform.position = Vector3.Lerp(camPos, new Vector3(closestColumn.x, minY + 0.5f, -10), easeSpeed * Time.deltaTime);
                if (camPos.y > minY)
                {
                    easeS = false;
                }
            }
            if (easeN)
            {
                Camera.main.transform.position = Vector3.Lerp(camPos, new Vector3(closestColumn.x, maxY - 0.5f, -10), easeSpeed * Time.deltaTime);
                if (camPos.y < maxY)
                {
                    easeN = false;
                }
            }
        }
        else//reset the cam's position
        {
            //this looks like repetitive trash buti had some issues with changing camposOG between levels and when going back to map.
            if (SceneManager.GetActiveScene().buildIndex > 0)
            {
                Camera.main.transform.position = Vector3.Lerp(camPos, Camera.main.GetComponent<CameraScript>().GetMidpoint(), easeSpeed * Time.deltaTime);
                if (Vector3.Distance(camPos, Camera.main.GetComponent<CameraScript>().GetMidpoint()) < 1f)
                {
                    resetCam = false;
                    camResetTimer = camResetTimerOG;

                }
            }

        }

        //Pinch to zoom, from: https://unity3d.com/learn/tutorials/topics/mobile-touch/pinch-zoom
        // If there are two touches on the device...
        ////if (Input.touchCount == 2)
        ////{
        ////    // Store both touches.
        ////    Touch touchZero = Input.GetTouch(0);
        ////    Touch touchOne = Input.GetTouch(1);

        ////    // Find the position in the previous frame of each touch.
        ////    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        ////    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        ////    // Find the magnitude of the vector (the distance) between the touches in each frame.
        ////    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        ////    float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

        ////    // Find the difference in the distances between each frame.
        ////    float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

        ////    // If the camera is orthographic...
        ////    if (Camera.main.orthographic)
        ////    {
        ////        // ... change the orthographic size based on the change in distance between the touches.
        ////        //Don't pickup pinch input if the player is moving
        ////        if (SceneManager.GetActiveScene().buildIndex == 0 || (SceneManager.GetActiveScene().buildIndex > 0 && !GameManager.GM.players[0].GetComponent<Player>().buttonMoving && !GameManager.GM.players[0].GetComponent<Player>().jumping)) //if it's the map screen OR if it's another screen AND you're not moving
        ////        {
        ////            Camera.main.orthographicSize += deltaMagnitudeDiff * pinchZoomSpeed;
        ////        }
        ////        // Make sure the orthographic size never drops below zero.
        ////        Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize, minZoomIn); //make the camera's size the maxiumum between its size and most zoomed in.
        ////        Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize, maxZoomOut);
        ////    }
        ////}


        //mousescroll pinch to zoom:
        if (Camera.main.orthographic)
        {
            float deltaMagnitudeDiff = Input.GetAxis("Mouse ScrollWheel");
            //Debug.Log("scroll delta: " + deltaMagnitudeDiff);
            
            Camera.main.orthographicSize += -deltaMagnitudeDiff * pinchZoomSpeed;
            // Make sure the orthographic size never drops below zero.
            Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize, minZoomIn); //make the camera's size the maxiumum between its size and most zoomed in.
            Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize, maxZoomOut);
        }
        //button zoom:
        if (Input.GetButtonDown(GameManager.GM.xBoxX +"ZoomIn") || xboxZoom!=0)
        {
            GameManager.GM.ZoomTrue();
        }

    }

    public void ResetTimer()
    {
        camResetTimer = camResetTimerOG;

    }

    public void PanCam(string leftOrRight)
    {
        if (!GameManager.GM.GOPanel.activeSelf && !GameManager.GM.lvlCompletePAnel.activeSelf && !GameManager.GM.creditsPanel.activeSelf && !GameManager.GM.quitPanel.activeSelf)
        {
            switch (leftOrRight)
            {
                case "left":
                    panLeft = true;

                    break;

                case "right":
                    panRight = true;
                    break;
            }
        }
    }

    public void FindClosestColumn()
    {
        panRight = false;
        panLeft = false;
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            if (columns[0] == null) //if the script lost the columns (it does during scene transfer)
            {
                columns = GameObject.FindGameObjectsWithTag("Column");
            }
            closestColumn = columns[0].transform.position;

            for (int i = 0; i < columns.Length; i++)
            {
                if (Vector3.Distance(camPos, columns[i].transform.position) < Vector3.Distance(camPos, closestColumn))
                {
                    closestColumn = columns[i].transform.position;
                }
            }
            if (camPos.y < minY)
            {
                easeS = true;
            }
            if (camPos.y > maxY)
            {
                easeN = true;
            }
        }
        else
        {
            closestColumn = Camera.main.GetComponent<CameraScript>().GetMidpoint();
        }
        //Debug.Log(closestColumn);
        easeToClosest = true;
        Camera.main.GetComponent<CameraScript>().enabled = true; //turn regular camera script back on.


    }
}

