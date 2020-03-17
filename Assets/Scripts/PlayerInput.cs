using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	[SerializeField]
	private string fire1, fire2, fire3, horizontal, vertical, submit, cancel;

	public float hValue, vValue;
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetButton(submit))
		{
			//do submit
		}
		if (Input.GetButton(cancel))
		{
			//do cancel
		}
		if (Input.GetButtonDown(fire1))
		{
			Fire1();
		}
		if (Input.GetButton(fire2))
		{
			Fire2();
		}
		if (Input.GetButtonDown(fire3))
		{
			Fire3();
		}

		hValue = Input.GetAxis(horizontal);
		vValue = Input.GetAxis(vertical);
	}

	protected virtual void Fire1()
	{

	}
	protected virtual void Fire2()
	{

	}
	protected virtual void Fire3()
	{
		
	}

}
