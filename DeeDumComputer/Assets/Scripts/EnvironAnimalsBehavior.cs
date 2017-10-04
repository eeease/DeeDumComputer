using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EnvironAnimalsBehavior : MonoBehaviour, ISubmitHandler {
    public CameraTouchControls camScript;
    RectTransform myRect;
	// Use this for initialization
	void Start () {
        camScript = GameObject.Find("GameManager").GetComponent<CameraTouchControls>();
        myRect = GetComponent<RectTransform>();

    }

    // Update is called once per frame
    void Update () {
		
	}

    public void ChangeAnchoredPos()
    {
        int randNum = Random.Range(0, camScript.columns.Length);
        GameObject randColumn = camScript.columns[randNum];
        transform.SetParent(randColumn.transform.parent);
        myRect.anchoredPosition = randColumn.transform.localPosition; //set its anchor to one of the columns at random.  This way it'll spawn in different spots?
        //Debug.Log("anchor should be " + camScript.columns[randNum].transform.position + "--" + randColumn.name);
    }

    public void OnSubmit(BaseEventData data)
    {
        GameManager.GM.ShowCreditsPanel();

    }
}
