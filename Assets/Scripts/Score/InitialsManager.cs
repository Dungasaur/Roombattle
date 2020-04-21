using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitialsManager : MonoBehaviour
{
	public string finalInitials;

	private readonly string initialArray = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_!?@$&";

	public Text[] initialFields;

	public int[] initialIndexes;

	public Button oneUpButton, oneDownButton, twoUpButton, twoDownButton, threeUpButton,threeDownButton;
	
	void Start()
	{
		oneUpButton.onClick.AddListener(() => ChangeInitial(0, true));
		oneDownButton.onClick.AddListener(() => ChangeInitial(0, false));
		twoUpButton.onClick.AddListener(() => ChangeInitial(1, true));
		twoDownButton.onClick.AddListener(() => ChangeInitial(1, false));
		threeUpButton.onClick.AddListener(() => ChangeInitial(2, true));
		threeDownButton.onClick.AddListener(() => ChangeInitial(2, false));
	}
	public void OnEnable()
	{
		initialIndexes = new int[3] { 0, 0, 0 };
		for (int i = 0; i < 3; i++)
		{
			initialFields[i].text = initialArray[initialIndexes[i]].ToString();
		}
	}

	public void ChangeInitial(int initialNumber, bool Increment)
	{
		Debug.Log("pressed");
		if(Increment)
		{
			initialIndexes[initialNumber]++;
			if(initialIndexes[initialNumber]>=initialArray.Length)
			{
				initialIndexes[initialNumber] = 0;
			}
		}
		else
		{
			initialIndexes[initialNumber]--;
			if (initialIndexes[initialNumber] <0)
			{
				initialIndexes[initialNumber] = initialArray.Length-1;
			}
		}
		initialFields[initialNumber].text = initialArray[initialIndexes[initialNumber]].ToString();
	}

	public void SetFinalInitials()
	{
		finalInitials = initialArray[initialIndexes[0]].ToString() + initialArray[initialIndexes[1]].ToString() + initialArray[initialIndexes[2]].ToString();
		GameManager.instance.SaveHighScore(finalInitials);
	}
}
