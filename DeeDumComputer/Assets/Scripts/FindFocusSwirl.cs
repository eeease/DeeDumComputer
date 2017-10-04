using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class FindFocusSwirl : MonoBehaviour, ISelectHandler
{
    GameObject focusSwirl;
    // Use this for initialization
    void Start()
    {
        focusSwirl = GameObject.Find("ButtonFocusObj");
        //check if this button was the last hub button to be selected before entering a level.
        //if so, bring the focusSwirl to this.
        if(name == GameManager.GM.mostRecentButtonName)
        {
            //focusSwirl.transform.SetParent(transform);
            GetComponent<Button>().Select();

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (focusSwirl.transform.parent == gameObject.transform) //only run this update lerping if this object is the current parent of the swirl.  (i think this was causing many issues?)
        {
            if (Vector3.Distance(focusSwirl.transform.localPosition, Vector3.zero) > .1f) //if the swirl is farther than .1f away from its target, then keep on lerpin, baby.
            {
                focusSwirl.transform.localPosition = Vector3.Lerp(focusSwirl.transform.localPosition, Vector3.zero, Time.deltaTime * GameManager.GM.focusSpeed);
            }
        }
    }

    public void OnSelect(BaseEventData eventdata)
    {
        focusSwirl.transform.SetParent(transform, true);
        if (name.Contains("Hub"))
        {
            GameManager.GM.mostRecentButtonName = name; //the reason i'm sending the name instead of the button component is because the button component gets lost in scene changing.  the string can stay stored, and when user goes back to map, that object can be searched for and found.
        }
        //Vector2 localPoint;
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.GetComponent<RectTransform>(), data.position, Camera.main, out localPoint);

    }

}