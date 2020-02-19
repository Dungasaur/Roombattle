using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoombaController : MonoBehaviour
{
	public float speed, rotationSpeed;
	private bool rotating;
	private Rigidbody rb;
	public Collider bumperCollider;
	
	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		foreach(Collider c in GetComponentsInChildren<Collider>())
		{
			if (c.gameObject.name == "Bumper")
			{
				bumperCollider = c;
			}
		}
	}
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void FixedUpdate()
	{
		if(!rotating)
		{
			transform.Translate(Vector3.forward * speed*Time.deltaTime);
		}
		else
		{
			transform.Rotate(Vector3.up * rotationSpeed);
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		if(collision.GetContact(0).thisCollider==bumperCollider)
		{// Checking if hit collider is the front bumper. if it is, start rotating.
			rotating = true;
			StartCoroutine("RotateFixed");
		}
	}

	IEnumerator RotateFixed()
	{
		yield return new WaitForSeconds(1);
		rotating = false;
	}

}
