using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRandomColor : MonoBehaviour
{
	Renderer rend;
	private void Awake()
	{
		rend = GetComponent<Renderer>();
		Color32 col = new Color32((byte)Random.Range(20, 234), (byte)Random.Range(20, 234), (byte)Random.Range(20, 234), (byte)rend.material.color.a);

		rend.material.SetColor("_Color", col);
	}
}
