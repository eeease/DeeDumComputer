using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetParentColor : MonoBehaviour {
    public Color parentColor;
    ParticleSystem particles;
    //~wound up not even needing this.  GM can directly change ps startColor instead of calling a script attached to the child of the platform or whatever...
	// Use this for initialization
	void Start () {
        //particles = GetComponent<ParticleSystem>();
        //parentColor = GetComponentInParent<SpriteRenderer>().color;
        //particles.startColor = parentColor;


    }


    public void UpdateParticleColor()
    {
        //parentColor = GetComponentInParent<SpriteRenderer>().color;
        //if (particles.startColor != parentColor)
        //{
        //    particles.startColor = parentColor;
        //}

    }
}
