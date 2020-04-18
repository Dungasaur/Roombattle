using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateCamera : MonoBehaviour
{
	public GameObject camera2;
    void Start()
    {
        if(Display.displays.Length>1 && Display.displays[1].active)
		{
			camera2.SetActive(true);
		}
    }

   
}
