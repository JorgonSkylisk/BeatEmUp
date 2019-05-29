using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectMenu : MonoBehaviour 
{
	public Image adamImage, axelImage;
	public Animator adamAnim, axelAnim;

	private Color defaultColor;
	private int characterIndex;
	private AudioSource audioS;

	void Start () 
	{
		characterIndex = 2;
		audioS = GetComponent<AudioSource> ();

	}
	

	void Update ()
	{


		if(Input.GetKeyDown(KeyCode.Return))
		{
			FindObjectOfType<GameManager>().characterIndex = characterIndex;
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		}
	}

	void PlaySound()
	{
		if(!audioS.isPlaying)
		{
			audioS.Play ();
		}
	}



	public void Confirm() // Start the game
	{
		FindObjectOfType<GameManager>().characterIndex = characterIndex;;
		LevelLoader.levelLoader.LoadLevel (SceneManager.GetActiveScene().buildIndex + 1);
	}
}
