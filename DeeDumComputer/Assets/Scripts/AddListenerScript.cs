using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class AddListenerScript : MonoBehaviour, ISubmitHandler, IPointerDownHandler
{
	// Use this for initialization
	void Start () {
        
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (gameObject.name.Contains("Level"))
        {
            GameManager.GM.OverWorldMapFalse();
        }
    }
    //using onSubmit for computer version of game instead of pointerdown.
    public void OnSubmit(BaseEventData data)
    {
        if (gameObject.name.Contains("Level"))
        {
            GameManager.GM.OverWorldMapFalse();
        }
    }

}
