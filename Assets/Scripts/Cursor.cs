using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[SelectionBase]
public class Cursor : PlayerInput
{
	// Strings for Input

	public float speed;
	public float relativeSpeed;
	public int playerNumber;
	public bool gameOver;
	private RectTransform rectTransform;
	RaycastHit hit;

	//For virtual mouse clicks
	PointerEventData pe;

	// idea from https://gamedev.stackexchange.com/questions/154954/setting-object-position-in-screen-space-camera
	RectTransform canvasRT;
	Vector2 scale;
	bool overlapping;

	private void Awake()
	{
		canvasRT = (RectTransform)transform.parent.GetComponent<Canvas>().transform;
		//scale = new Vector2( canvasRT.rect. / Screen.width, canvasRT.height / Screen.height);
		pe = new PointerEventData(EventSystem.current);
		rectTransform = GetComponentInChildren<RectTransform>();
		gameOver = true;
	}

	private void Start()
	{
		SetRelativeSpeed();
		overlapping = false;
	}

	protected override void Update()
	{
		base.Update();
	}

	private void FixedUpdate()
	{

		//SetRelativeSpeed();
		//var y = Screen.Height - Screen.Height / 2;      // 50% 
		//var y = Screen.Height - Screen.Height / 10;    // 10% 
		//var y = Screen.Height - Screen.Height / 5;      // 20% 
		//var y = Screen.Height - Screen.Height / 3;      // 33% 

		// Set a speed relative to a 1920x1080 screen. Get screen size and area. howmuch bigger the screen is than 100x100, multiply speed by that ratio to get the conistent speed.


		rectTransform.anchoredPosition += (new Vector2(hValue * relativeSpeed, vValue * relativeSpeed));
		if (rectTransform.anchoredPosition.y > canvasRT.rect.height)
		{
			rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, canvasRT.rect.height);
		}
		else if (rectTransform.anchoredPosition.y < 0)
		{
			rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 0);
		}

		if (rectTransform.anchoredPosition.x > canvasRT.rect.width)
		{
			rectTransform.anchoredPosition = new Vector2(canvasRT.rect.width, rectTransform.anchoredPosition.y);
		}
		else if (rectTransform.anchoredPosition.x < 0)
		{
			rectTransform.anchoredPosition = new Vector2(0, rectTransform.anchoredPosition.y);
		}

		bool isNowOverlapping = false;
		foreach(Cursor c in GameManager.instance.cursors)
		{
			
			if(c!=this)
			{
				if (is_rectTransformsOverlap(Camera.main,c.rectTransform))
				{
					isNowOverlapping = true;
				}
			}
		}
		if(!overlapping && isNowOverlapping)
		{
			overlapping = true;
			Debug.Log("overlap");
			relativeSpeed /= 3f;
		}
		if(overlapping==true && !isNowOverlapping)
		{
			Debug.Log("NoOverlap");
			relativeSpeed *= 3f;
		}
		

	}
	//private void OnCollisionEnter2D(Collision2D collision)
	//{
	//	relativeSpeed /= 3f;
	//	Debug.Log("Collided");
	//}
	//private void OnCollisionExit2D(Collision2D collision)
	//{
	//	relativeSpeed *= 3f;
	//	Debug.Log("Exited");
	//}
	protected override void Fire1Down()
	{// used to both put dirt down and select Menu Options.
		if (gameOver)
		{ // adapted from https://answers.unity.com/questions/783279/46-ui-how-to-detect-mouse-over-on-button.html
			pe.position = rectTransform.anchoredPosition;
			List<RaycastResult> hits = new List<RaycastResult>();
			EventSystem.current.RaycastAll(pe, hits);

			for (int i = 0; i < hits.Count; i++)
			{
				Button b = hits[i].gameObject.GetComponent<Button>();
				if (b != null)
				{
					b.onClick.Invoke();
				}
			}

		}
		else
		{
			PlaceDirt();
		}
	}

	protected override void Fire2Down()
	{
		relativeSpeed /= 2f;
	}

	protected override void Fire2Up()
	{
		relativeSpeed *= 2f;
	}

	void SetRelativeSpeed()
	{
		// current speed is based off 1024*768 resolution. Get ratio of current screen size and adjust speed to match ratio.
		relativeSpeed = canvasRT.rect.width / (speed * 115);

	}



	void PlaceDirt()
	{

		if (CastDown())
		{
			if (hit.collider.gameObject.tag == "Ground")
			{
				if (GameManager.instance.ReduceScore(1, playerNumber))
				{
					GameObject dirt = ObjectPool.instance.GetDirt();
					dirt.SetActive(true);
					dirt.transform.position = hit.point;
				}

			}
			else if (hit.collider.gameObject.tag == "Bucket")
			{
				GameManager.instance.ResetScore(playerNumber);
			}
		}
	}

	bool CastDown()
	{
		bool hitSomething = false;
		Ray ray = Camera.main.ViewportPointToRay(new Vector3(rectTransform.anchoredPosition.x / canvasRT.rect.width, rectTransform.anchoredPosition.y / canvasRT.rect.height));
		if (Physics.Raycast(ray, out hit))
		{
			Debug.Log(hit.collider.gameObject.name);
			hitSomething = true;
		}
		else
		{
			Debug.Log("Missed");
		}
		return hitSomething;
	}

	public static bool is_rectTransformsOverlap(Camera cam,	RectTransform elem, RectTransform viewport = null)
	{
		// https://forum.unity.com/threads/check-if-2-recttransforms-overlapping.515670/
		Vector2 viewportMinCorner;
		Vector2 viewportMaxCorner;

		if (viewport != null)
		{
			//so that we don't have to traverse the entire parent hierarchy (just to get screen coords relative to screen),
			//ask the camera to do it for us.
			//first get world corners of our rect:
			Vector3[] v_wcorners = new Vector3[4];
			viewport.GetWorldCorners(v_wcorners); //bot left, top left, top right, bot right

			//+ow shove it back into screen space. Now the rect is relative to the bottom left corner of screen:
			viewportMinCorner = cam.WorldToScreenPoint(v_wcorners[0]);
			viewportMaxCorner = cam.WorldToScreenPoint(v_wcorners[2]);
		}
		else
		{
			//just use the scren as the viewport
			viewportMinCorner = new Vector2(0, 0);
			viewportMaxCorner = new Vector2(Screen.width, Screen.height);
		}

		//give 1 pixel border to avoid numeric issues:
		viewportMinCorner += Vector2.one;
		viewportMaxCorner -= Vector2.one;

		//do a similar procedure, to get the "element's" corners relative to screen:
		Vector3[] e_wcorners = new Vector3[4];
		elem.GetWorldCorners(e_wcorners);

		Vector2 elem_minCorner = cam.WorldToScreenPoint(e_wcorners[0]);
		Vector2 elem_maxCorner = cam.WorldToScreenPoint(e_wcorners[2]);

		//perform comparison:
		if (elem_minCorner.x > viewportMaxCorner.x) { return false; }//completelly outside (to the right)
		if (elem_minCorner.y > viewportMaxCorner.y) { return false; }//completelly outside (is above)

		if (elem_maxCorner.x < viewportMinCorner.x) { return false; }//completelly outside (to the left)
		if (elem_maxCorner.y < viewportMinCorner.y) { return false; }//completelly outside (is below)

		/*
             commented out, but use it if need to check if element is completely inside:
            Vector2 minDif = viewportMinCorner - elem_minCorner;
            Vector2 maxDif = viewportMaxCorner - elem_maxCorner;
            if(minDif.x < 0  &&  minDif.y < 0  &&  maxDif.x > 0  &&maxDif.y > 0) { //return "is completely inside" }
        */

		return true;//passed all checks, is inside (at least partially)
	}

}
