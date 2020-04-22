using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer: MonoBehaviour
{
	public AudioSource audioSource;
	public AudioClip[] randomClips;
	public AudioClip[] sortedClips;

	public void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}
	public void PlayRandomClip()
	{
		if(randomClips.Length>0)
		{
			audioSource.pitch = 1;
			audioSource.clip = randomClips[Random.Range(0, randomClips.Length)];
			audioSource.Play();
		}
	}

	public void PlayClip(int index, float pitch)
	{
		if(sortedClips.Length>0)
		{
			audioSource.pitch = pitch;
			audioSource.clip = sortedClips[index];
			audioSource.Play();
		}
	}
}
