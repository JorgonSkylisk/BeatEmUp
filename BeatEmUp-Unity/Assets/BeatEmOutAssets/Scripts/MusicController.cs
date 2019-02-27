using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour 
{
	public static MusicController instance{ get; set;}

	public AudioClip  levelSong,bossSong,levelClearSong;

	private AudioSource audioS;

	void Awake()
	{
		instance = this;
	}

	void Start () 
	{
		audioS = GetComponent<AudioSource> ();
		PlaySong (levelSong);
	}


	public void PlaySong(AudioClip clip)
	{
		audioS.clip = clip;
		audioS.Play ();
	}
}
