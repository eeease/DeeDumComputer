using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlideHideAway : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float slideAwayTime, slideAwayTimer, slideAwaySpeed, yPosUp, yPosDown;
    bool mouseOverPanel, slideUp, slideDown;
    Vector3 upPos, downPos;
    RectTransform myRect;
    // Use this for initialization
    void Start()
    {
        myRect = GetComponent<RectTransform>();
        slideAwayTime = slideAwayTimer;
        downPos = myRect.anchoredPosition;
        upPos = new Vector3(myRect.anchoredPosition.x, yPosUp);

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(myRect.anchoredPosition);
        if (!mouseOverPanel)
        {
            slideAwayTime -= Time.deltaTime;
            if (slideAwayTime <= 0)
            {
                slideUp = true;
            }
        }else
        {
            slideUp = false;
            slideAwayTime = slideAwayTimer;
            slideDown = true;
        }

        if (slideUp)
        {

            myRect.anchoredPosition = Vector3.Lerp(myRect.anchoredPosition, upPos, Time.deltaTime * slideAwaySpeed);
        }
        if (slideDown)
        {
            myRect.anchoredPosition = Vector3.Lerp(myRect.anchoredPosition, downPos, Time.deltaTime * slideAwaySpeed);
            if(Vector2.Distance(myRect.anchoredPosition, downPos) < .1f)
            {
                slideDown = false;
            }
        }

    }

    public void OnPointerEnter(PointerEventData data)
    {
        //Vector2 localPoint;
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.GetComponent<RectTransform>(), data.position, Camera.main, out localPoint);
        mouseOverPanel = true;

    }

    public void OnPointerExit(PointerEventData data)
    {
        mouseOverPanel = false;
    }
}
