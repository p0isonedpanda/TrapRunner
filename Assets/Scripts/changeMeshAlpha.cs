using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeMeshAlpha : MonoBehaviour
{ 
    public Material[] mats;

	// Update is called once per frame
	void Update ()
    {
        if (gameObject.transform.parent == Camera.main.transform)
        {
            GetComponent<Renderer>().material = mats[1];
        }
        else
        {
            GetComponent<Renderer>().material = mats[0];
        }
	}
}
