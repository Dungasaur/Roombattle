﻿using System.Collections;
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

	public int dirtCounter, playerNumber;

	private float followingThreshhold;
	private Collider bumperCollider;
	private bool keepMoving;
	private Rigidbody rb;
	private LayerMask dirtLayer;
	private GameObject currentTarget;

	void Awake()
	{
		//Physics.IgnoreCollision(bumperCollider, bodycollider, true) ;
		keepMoving = false;
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
		dirtCounter = 5;
		followingThreshhold = 1;
		GameManager.instance.SetScore(dirtCounter, playerNumber);
    }



	private void FixedUpdate()
	{

		switch (state)
		{
			case State.Rotating:
				
					transform.Rotate(Vector3.up * rotationSpeed);

				break;
			case State.Moving:
				if (!keepMoving)
				{
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
						if (currentTarget != target)
						{
							currentTarget = target;
							state = State.Following;
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
				}
				transform.Translate(Vector3.forward * speed * Time.deltaTime);

				break;
			case State.Following:
				
					//Check if front of roomba is within '5' degrees of facing the object dead on. If it is, move forward.
					// if it isn't, Rotate toward target.
					if (currentTarget == null || Vector3.Distance(transform.position, currentTarget.transform.position) < followingThreshhold || currentTarget.activeInHierarchy == false)
					{
						currentTarget = null;
						state = State.Moving;
					}
					else
					{
						Vector3 heading = (new Vector3(currentTarget.transform.position.x, transform.position.y, currentTarget.transform.position.z) - transform.position).normalized;

						//if(transform.rotation != Quaternion.Euler(heading))
						if (Vector3.Distance(transform.forward, heading) > .05f)
						{
							transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, (heading), rotationSpeed * Time.deltaTime, 0), Vector3.up);
						}
						else
						{
							transform.Translate(Vector3.forward * speed * Time.deltaTime);
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
			//transform.Translate(transform.forward * (-speed) * Time.deltaTime);
			StartCoroutine(RotateFixed());
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.layer == 8)
		{
			dirtCounter++;
			GameManager.instance.ChangeScore(dirtCounter,playerNumber);
			other.transform.root.gameObject.SetActive(false);
		}
	}

	IEnumerator RotateFixed()
	{
		State tempState = state;
		state = State.Rotating;
		currentTarget = null;
		yield return new WaitForSeconds(1);

		if (tempState != State.Rotating)
			state = tempState;
		else
			state = State.Moving;
	}

	IEnumerator KeepMoving()
	{
		keepMoving = true;
		yield return new WaitForSeconds(0.5f);
		keepMoving = false;
	}

	//IEnumerator RotateDynamic(Vector3 position)
	//{
	//	coroutineIsRunning = true;
	//	float angle = Vector3.SignedAngle(transform.forward, (position - transform.position), Vector3.up);
	//	//Vector3.RotateTowards()
	//	coroutineIsRunning = false;

	//}

}
