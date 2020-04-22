using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideTitleScreen : MonoBehaviour
{
	public GameObject menu;
    void Awake()
    {
        if(Statics.numberOfPlayers!=0)
		{
			menu.SetActive(true);
			gameObject.SetActive(false);
		}
    }

    
}
