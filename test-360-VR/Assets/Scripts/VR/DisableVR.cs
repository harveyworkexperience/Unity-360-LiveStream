using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableVR : MonoBehaviour {

	// Use this for initialization
	void Start () {
        UnityEngine.XR.XRSettings.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
