using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour 
{
	public static LevelLoader levelLoader;

	public GameObject loadingScreen;
	public Slider slider;
	public Text progressText;


	void Awake()
	{
		levelLoader = this;
	}


	public void LoadLevel(int sceneIndex)
	{
		loadingScreen.SetActive (true);
		StartCoroutine (LoadAsynchronously(sceneIndex));
	}

	IEnumerator LoadAsynchronously(int sceneIndex)
	{

		AsyncOperation operation = SceneManager.LoadSceneAsync (sceneIndex);

		while(operation.isDone == false)
		{
			float progress = Mathf.Clamp01 (operation.progress / 0.9f);

			slider.value = progress;
			progressText.text = progress * 100f + "%";

			yield return null;
		}
	}
}
