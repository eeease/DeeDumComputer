using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnPointerDown(PointerEventData eventData)
    {
        switch (gameObject.name)
        {
            case "LeftButton":
                GameManager.GM.players[0].GetComponent<Player>().MoveLeft();
                GameManager.GM.players[1].GetComponent<Player>().MoveLeft();

                break;

            case "RightButton":
                GameManager.GM.players[0].GetComponent<Player>().MoveRight();
                GameManager.GM.players[1].GetComponent<Player>().MoveRight();

                break;

            case "JumpButton":
                GameManager.GM.players[0].GetComponent<Player>().StartJump();
                GameManager.GM.players[1].GetComponent<Player>().StartJump();
                //update number of jumps:
                GameManager.GM.LogStats(true, "jumps");
                GameManager.GM.LogStats(false, "jumps");
                break;

            case "LevelSkipButton":
                GameManager.GM.LogStats(false, "skips");

                break;

            case "ResetButton":
                GameManager.GM.LogStats(false, "restarts");
                break;

            case "HeartsButton":
                GameManager.GM.LogStats(false, "buyHearts");
                break;

        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(gameObject.name == "LeftButton" || gameObject.name == "RightButton")
        {
            GameManager.GM.players[0].GetComponent<Player>().VelReset();
            GameManager.GM.players[1].GetComponent<Player>().VelReset();

        }
        if(gameObject.name == "JumpButton")
        {
            GameManager.GM.players[0].GetComponent<Player>().JumpVelReset();
            GameManager.GM.players[1].GetComponent<Player>().JumpVelReset();

        }
    }
    
}
