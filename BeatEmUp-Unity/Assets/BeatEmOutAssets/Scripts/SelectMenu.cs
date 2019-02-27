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
		characterIndex = 1;
		audioS = GetComponent<AudioSource> ();
		defaultColor = axelImage.color; 
	}
	

	void Update ()
	{
		if(Input.GetKeyDown(KeyCode.A))
		{
			characterIndex = 1;
			PlaySound();
		}
		else if(Input.GetKeyDown(KeyCode.D))
		{
			characterIndex = 2;
			PlaySound();
		}

		if (characterIndex == 1)
		{
			adamImage.color = Color.yellow;
			adamAnim.SetBool ("Attack", true);
			axelImage.color = defaultColor;
			axelAnim.SetBool ("Attack", false);
		}
		else if(characterIndex == 2)
		{
			axelImage.color = Color.yellow;
			axelAnim.SetBool ("Attack", true);
			adamImage.color = defaultColor;
			adamAnim.SetBool ("Attack", false);
		}

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


	public void SelectAdam()
	{
		characterIndex = 1;
	}

	public void SelectAxel()
	{
		characterIndex = 2;
	}

	public void Confirm()
	{
		FindObjectOfType<GameManager>().characterIndex = characterIndex;
		//SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		LevelLoader.levelLoader.LoadLevel (SceneManager.GetActiveScene().buildIndex + 1);
	}
}
