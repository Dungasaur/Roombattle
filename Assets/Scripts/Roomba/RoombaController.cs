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
		state = State.Moving;
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
						state = State.Following;
					}
				}
				else
				{
					currentTarget = null;
				}

				transform.Translate(Vector3.forward * speed * Time.deltaTime);

				break;
			case State.Following:

				if(!coroutineIsRunning)
				{
					//Check if front of roomba is within '5' degrees of facing the object dead on. If it is, move forward.
					// if it isn't, Rotate toward target.
					if (currentTarget == null)
					{
						state = State.Moving;
					}
					else
					{
						Vector3 heading = (new Vector3(currentTarget.transform.position.x, transform.position.y, currentTarget.transform.position.z) - transform.position).normalized;

						//Debug.Log(currentAngle);at currentAngle = Vector3.SignedAngle(transform.forward,(transform.position - currentTarget.transform.position), transform.up);
						//Debug.Log(currentAngle);

						//if(transform.rotation != Quaternion.Euler(heading))
						if (Vector3.Distance(transform.forward, heading) > .05f)
						{
							transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, (heading), rotationSpeed * Time.deltaTime, 0), Vector3.up);
						}
						else
						{
							transform.Translate(Vector3.forward * speed * Time.deltaTime);
						}
						//if(currentAngle> 20f)
						//{
						//	//Rotate Right
						//	transform.Rotate(Vector3.up * -rotationSpeed);
						//}
						//else if(currentAngle<-20f)
						//{
						//	//Roate Left
						//	transform.Rotate(Vector3.up * rotationSpeed);
						//}
						//else
						//{
						//	
						//}
					}
				}
				

				break;
			default:
				break;
		}
		
	}

	void OnCollisionEnter(Collision collision)
	{
		if(collision.GetContact(0).thisCollider==bumperCollider)
		{// Checking if hit collider is the front bumper. if it is, start rotating.
			
			StartCoroutine("RotateFixed");
		}
	}

	IEnumerator RotateFixed()
	{
		State tempState = state;
		state = State.Rotating;
		currentTarget = null;
		yield return new WaitForSeconds(1);
		
		state =tempState;
	}

	//IEnumerator RotateDynamic(Vector3 position)
	//{
	//	coroutineIsRunning = true;
	//	float angle = Vector3.SignedAngle(transform.forward, (position - transform.position), Vector3.up);
	//	//Vector3.RotateTowards()
	//	coroutineIsRunning = false;

	//}

}
