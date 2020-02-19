using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoombaController : MonoBehaviour
{
	public enum State
	{
		Rotating,
		Moving,
		Following
	};
	public State state;

	public float speed, rotationSpeed, dirtCheckRadius;
	private Collider bumperCollider;
	private bool coroutineIsRunning;
	private Rigidbody rb;
	private LayerMask dirtLayer;
	private GameObject currentTarget;

	void Awake()
	{
		coroutineIsRunning = false;
		dirtLayer = LayerMask.GetMask("Dirt");
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

		switch (state)
		{
			case State.Rotating:
				if(!coroutineIsRunning)
				{
					transform.Rotate(Vector3.up * rotationSpeed);

				}
				break;
			case State.Moving:
				Collider[] nearbyDirt = Physics.OverlapSphere(transform.position, dirtCheckRadius, dirtLayer, QueryTriggerInteraction.Collide);
				if (nearbyDirt.Length > 1)
				{
					GameObject target = nearbyDirt[0].gameObject;
					float distance = Vector3.Distance(transform.position, target.transform.position);
					for (int i = 1; i < nearbyDirt.Length; i++)
					{
						float tempDistance = Vector3.Distance(transform.position, nearbyDirt[i].transform.position);
						if (tempDistance < distance)
						{
							distance = tempDistance;
							target = nearbyDirt[i].gameObject;
						}
					}
					if(currentTarget != target)
					{
						currentTarget = target;
						StartCoroutine("RotateDynamic", currentTarget.transform.position);
					}
				}
				else if (nearbyDirt.Length == 1)
				{
					if (currentTarget != nearbyDirt[0].gameObject)
					{
						currentTarget = nearbyDirt[0].gameObject;
						StartCoroutine("RotateDynamic", currentTarget.transform.position);
					}
				}
				else
				{
					currentTarget = null;
				}

				transform.Translate(Vector3.forward * speed * Time.deltaTime);

				break;
			case State.Following:
				break;
			default:
				break;
		}
		
	}

	void OnCollisionEnter(Collision collision)
	{
		if(collision.GetContact(0).thisCollider==bumperCollider)
		{// Checking if hit collider is the front bumper. if it is, start rotating.
			state = State.Rotating;
			StartCoroutine("RotateFixed");
		}
	}

	IEnumerator RotateFixed()
	{
		currentTarget = null;
		yield return new WaitForSeconds(1);
		
		state = State.Moving;
	}

	IEnumerator RotateDynamic(Vector3 position)
	{
		coroutineIsRunning = true;
		yield return null;
		//Vector3.RotateTowards()
		coroutineIsRunning = false;

	}

}
