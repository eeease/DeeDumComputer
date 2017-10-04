using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityStandardAssets.CrossPlatformInput
{
	public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
	{
		public enum AxisOption
		{
			// Options for which axes to use
			Both, // Use both
			OnlyHorizontal, // Only horizontal
			OnlyVertical // Only vertical
		}

		public int MovementRange = 100;
		public AxisOption axesToUse = AxisOption.Both; // The options for the axes that the still will use
		public string horizontalAxisName = "Horizontal"; // The name given to the horizontal axis for the cross platform input
		public string verticalAxisName = "Vertical"; // The name given to the vertical axis for the cross platform input

		public Vector3 m_StartPos;
		bool m_UseX; // Toggle for using the x axis
		bool m_UseY; // Toggle for using the Y axis
		CrossPlatformInputManager.VirtualAxis m_HorizontalVirtualAxis; // Reference to the joystick in the cross platform input
		CrossPlatformInputManager.VirtualAxis m_VerticalVirtualAxis; // Reference to the joystick in the cross platform input
        public bool holdingUp, beingTouched;
        public int holdingUpCount; //this is incremented by Player.cs.  when it's 2 (both characters jumping) holdingUp is set to true.

		void OnEnable()
		{
		}

        void Start()
        {
            CreateVirtualAxes();

            m_StartPos = transform.localPosition;
            holdingUpCount = 0;
        }

        void Update()
        {
            ////when user touches screen, store that position so that the joystick can go back there:
            //if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            //{
            //    gameObject.SetActive(true);
            //    Vector3 pos = Input.GetTouch(0).position;
            //    m_StartPos = Camera.main.ScreenToWorldPoint(pos);
            //}
            if (holdingUpCount >= 2 && !holdingUp)
            {
                holdingUp = true;
            }
        }

        void UpdateVirtualAxes(Vector3 value)
		{
			var delta = m_StartPos - value;
			delta.y = -delta.y;
			delta /= MovementRange;
			if (m_UseX)
			{
				m_HorizontalVirtualAxis.Update(-delta.x);
			}

			if (m_UseY)
			{
				m_VerticalVirtualAxis.Update(delta.y);
			}
            //Debug.Log("My pos: " + transform.localPosition);
		}

		void CreateVirtualAxes()
		{
			// set axes to use
			m_UseX = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyHorizontal);
			m_UseY = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyVertical);

			// create new axes based on axes to use
			if (m_UseX)
			{
				m_HorizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
				CrossPlatformInputManager.RegisterVirtualAxis(m_HorizontalVirtualAxis);
			}
			if (m_UseY)
			{
				m_VerticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(verticalAxisName);
				CrossPlatformInputManager.RegisterVirtualAxis(m_VerticalVirtualAxis);
			}
		}


		public void OnDrag(PointerEventData data)
		{
            
			Vector3 newPos = Vector3.zero;
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), data.position, Camera.main, out localPoint);


            if (m_UseX)
			{
				int delta = (int)(localPoint.x - m_StartPos.x);
				//delta = Mathf.Clamp(delta, - MovementRange, MovementRange);
				newPos.x = delta;
			}

			if (m_UseY)
			{
				int delta = (int)(localPoint.y - m_StartPos.y);
				//delta = Mathf.Clamp(delta, -MovementRange, MovementRange);
				newPos.y = delta;
			}
			transform.localPosition = Vector3.ClampMagnitude(new Vector3(newPos.x, newPos.y,newPos.z), MovementRange)+m_StartPos;
			UpdateVirtualAxes(transform.localPosition);
          
        }


		public void OnPointerUp(PointerEventData data)
		{
			transform.localPosition = m_StartPos;
			UpdateVirtualAxes(m_StartPos);
            holdingUp = false;
            holdingUpCount = 0;
            beingTouched = false;
		}


		public void OnPointerDown(PointerEventData data) { //~~this needs to tell the gm or players that they're button moving as soon as it is touched (not once it starts moving!)
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), data.position, Camera.main, out localPoint);
            beingTouched = true;
            //Debug.Log("Parent RectT = " + transform.parent.GetComponent<RectTransform>().name);
            //Debug.Log("Click Pos = " + localPoint);
        }
        void DebugPoint(PointerEventData ped)
        {
            Vector2 localCursor;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.GetComponent<RectTransform>(), ped.position, null, out localCursor))
                return;

           // Debug.Log("LocalCursor:" + localCursor);
        }
        void OnDisable()
		{
			// remove the joysticks from the cross platform input
			if (m_UseX)
			{
				//m_HorizontalVirtualAxis.Remove();
			}
			if (m_UseY)
			{
				//m_VerticalVirtualAxis.Remove();
			}
		}
	}
}