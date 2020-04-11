using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayActivate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(Display.displays.Length == 1|| Display.displays[1].active)
		{
			gameObject.SetActive(false);
		}
    }

	public void ActivateDisplays()
	{
		if (Display.displays.Length >= 2)
		{
			Display.displays[1].Activate();
			gameObject.SetActive(false);
		}
	}
}
