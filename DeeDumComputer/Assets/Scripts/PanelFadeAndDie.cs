using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelFadeAndDie : MonoBehaviour {
    public Image thisPanel;
    public Text panelText;
    public float duration; //how long will it take the panel to become transparent?
    public float stayTimer = 1; //stay on screen for 1 second before fading.
    public float stayTimerOG;
    // Use this for initialization
    void Start () {
        thisPanel = GetComponent<Image>();
        panelText = GetComponentInChildren<Text>();
        thisPanel.canvasRenderer.SetColor(Color.white);
	}
	
	// Update is called once per frame
	void Update () {
        if (thisPanel.enabled)
        {
            stayTimer -= Time.deltaTime;
        }
        if (stayTimer <= 0)
        {
            if (thisPanel.IsActive())
            {
                thisPanel.CrossFadeAlpha(0, duration, false);
                panelText.CrossFadeAlpha(0, duration, false);
            }
            if (thisPanel.canvasRenderer.GetColor().a <= .1f)
            {
                thisPanel.enabled = false;
                panelText.enabled = false; //better than disabling the obj, cause then it can't be found.
                stayTimer = stayTimerOG;
            }
        }
	}
}
