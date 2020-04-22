using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class RoombaController : MonoBehaviour
{
	public enum State
	{
		Rotating,
		Moving,
		Dead,
		Following
	};
	public State state;
	public Color col;
	public float speed, rotationSpeed, dirtCheckRadius;

	public int playerNumber, balloonCount;
	public AudioPlayer audioPlayer;

	private float followingThreshhold;
	private Collider bumperCollider, bodyCollider;
	private bool keepMoving;
	private Rigidbody rb;
	private LayerMask dirtLayer;
	private GameObject currentTarget;

	private Vector3 startPosition;
	private Quaternion startRotation;
	private Balloon[] balloons;

	void Awake()
	{
		startPosition = transform.position;
		startRotation = transform.rotation;
		col = GetComponentInChildren<Renderer>().material.color;
		balloonCount = 3;
		keepMoving = false;
		dirtLayer = LayerMask.GetMask("Dirt");
		rb = GetComponent<Rigidbody>();
		balloons = GetComponentsInChildren<Balloon>();
		foreach(Collider c in GetComponentsInChildren<Collider>())
		{
			if (c.gameObject.name == "Bumper")
			{
				bumperCollider = c;
			}
			if (c.gameObject.name == "Body")
			{
				bodyCollider = c;
			}
		}
		Physics.IgnoreCollision(bumperCollider, bodyCollider, true);
		foreach (Balloon b in balloons)
		{
			b.papaRoomba = this;
		}
		audioPlayer = GetComponent<AudioPlayer>();
	}
    void Start()
    {
		state = State.Moving;
		startPosition = transform.position;
		startRotation = transform.rotation;
		followingThreshhold = 1;
    }

	private void FixedUpdate()
	{
		//Lock the Roombas to the ground, don't let them rotate so they'll fly away or go through the floor.
		if(transform.localPosition.y !=1)
		{
			transform.localPosition = new Vector3(transform.localPosition.x,1,transform.localPosition.z);
		}

		if (transform.localRotation.x+transform.localRotation.z != 0)
		{
			transform.localRotation = new Quaternion(0, transform.localRotation.y, 0, transform.localRotation.w);
		}


		switch (state)
		{
			case State.Rotating:

				transform.Rotate(Vector3.up * rotationSpeed);

				break;
			case State.Moving:
				if (!keepMoving)
				{
					Collider[] nearbyDirt = Physics.OverlapSphere(transform.position, dirtCheckRadius, dirtLayer, QueryTriggerInteraction.Collide);
					if (nearbyDirt.Length > 0)
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
					Vector3 heading = (new Vector3(currentTarget.transform.position.x, currentTarget.transform.position.y, transform.position.z) - transform.position).normalized;
					//if(transform.rotation != Quaternion.Euler(heading))
					if (Vector3.Distance(transform.forward, heading) > .05f)
					{
						transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, (heading), rotationSpeed * Time.deltaTime, .1f), transform.up);
					}
					else
					{
						transform.Translate(Vector3.forward * speed * Time.deltaTime);
						
					}
				}
				break;
			case State.Dead:
				
				break;
			default:
				break;
		}
	}

	public void Reset()
	{
		transform.position = startPosition;
		transform.rotation = startRotation;
		foreach(Balloon b in balloons)
		{
			b.Reset();
		}
		balloonCount = 3;
		state = State.Moving;
	}

	void OnCollisionEnter(Collision collision)
	{
		if(state != State.Dead && collision.gameObject.tag != "Ground")
		{
			if(collision.gameObject.tag == "Wall")
			{
				transform.Translate(Vector3.back * 0.02f);
			}
			if (collision.GetContact(0).thisCollider == bumperCollider)
			{// Checking if hit collider is the front bumper. if it is, start rotating.
			 //transform.Translate(transform.forward * (-speed) * Time.deltaTime);
				audioPlayer.PlayClip(0, Random.Range(0.7f, 1.2f));
				StartCoroutine(RotateFixed());
			}
		}
		
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.layer == 8)
		{
			GameManager.instance.ChangeScore(1,playerNumber);
			other.transform.root.gameObject.SetActive(false);
		}
	}

	public void BalloonPopped()
	{
		audioPlayer.PlayRandomClip();
		balloonCount--;
		if(balloonCount <=0)
		{
			StopAllCoroutines();
			GameManager.instance.IDied(playerNumber);
			state = State.Dead;
		}
	}

	public void GetScore(int score)
	{
		GameManager.instance.ChangeScore(score, playerNumber);
	}

	public void GameOver()
	{
		StopAllCoroutines();
		state = State.Dead;
	}

	IEnumerator RotateFixed()
	{
		State tempState = state;
		state = State.Rotating;
		currentTarget = null;
		yield return new WaitForSeconds(1);

		//if (tempState != State.Rotating)
		//	state = tempState;
		//else
		//{
			state = State.Moving;
			StartCoroutine(KeepMoving());
		//}
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
