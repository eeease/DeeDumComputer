using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraScript : MonoBehaviour {
    public GameObject player, player2;
    Vector3 midPoint, camPos2D;
    public float camSpeed, windowDist;

	// Use this for initialization
	void Start () {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            //player = GameManager.GM.players[0] ;
            //player2 = GameManager.GM.players[1];
            //transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
            //I know this is hacky but i wound up making a bunch of levels and don't feel like changing the cam's variables in all of them so i'm just going to change them from the one GM:
            camSpeed = GameManager.GM.camSpeed;
            windowDist = GameManager.GM.camWindowDist;
            //GetComponent<Camera>().orthographicSize = GameManager.GM.maxZoom; ~~this is a cool idea but is a lot of motion for the first thing a player will see.
        }
        if(GameManager.GM.orthoSizeOG!= gameObject.GetComponent<Camera>().orthographicSize)
        {
            GameManager.GM.orthoSizeOG = gameObject.GetComponent<Camera>().orthographicSize; //update the original size of the cam when changing scenes.
        }
	}
	
	// Update is called once per frame
	void Update () {
        
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            camPos2D = new Vector3(transform.position.x, transform.position.y, -10);
            //playerPos2D = new Vector2(player.transform.position.x, player.transform.position.y);


            //transform.position = new Vector3(GetMidpoint().x, GetMidpoint().y, transform.position.z);
            ////lerp the camera to the player's pos
            if (Vector3.Distance(camPos2D, GetMidpoint()) > windowDist)
            {
                camPos2D = Vector3.Lerp(camPos2D, GetMidpoint(), camSpeed * Time.deltaTime);
                transform.position = new Vector3(camPos2D.x, camPos2D.y, -10);
            }
        }

        //GM sets zoomedout to be true based on a button press.  This should always check in order to set it vis pinch to zoom:
        if(GetComponent<Camera>().orthographicSize >= GameManager.GM.maxZoom)
        {
            GameManager.GM.zoomedIn = false;
            GameManager.GM.zoomedOut = true;
        }
        if(GetComponent<Camera>().orthographicSize <= GameManager.GM.orthoSizeOG)
        {
            GameManager.GM.zoomedOut = false;
            GameManager.GM.zoomedIn = true;
        }
    }

    public Vector3 GetMidpoint()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            GameObject focusSwirl = GameObject.Find("ButtonFocusObj");
            Vector3 midLoc = new Vector3(focusSwirl.transform.position.x, focusSwirl.transform.position.y, -10);
            return midLoc;
        }
        else
        {
            if (player != null)
            {
                float xDif = player.transform.position.x + (player2.transform.position.x - player.transform.position.x) / 2;
                float yDif = player.transform.position.y + (player2.transform.position.y - player.transform.position.y) / 2;
                float dist = Vector3.Distance(player.transform.position, player2.transform.position);
                Vector3 midLoc = new Vector3(xDif, yDif, -10); //keep the -10 zPos otherwise it'll move to 0, which is no good.

                //Debug.Log(midLoc);
                return midLoc;
            }
            else
            {
                return new Vector3(0, 0, -10);
            }
        }
    }
}
