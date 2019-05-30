using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeavyBoss : Enemy
{

	public float minSpecialAttackTime,maxSpecialAttackTime;

	// Use this for initialization
	public override void Start ()
	{
		base.Start ();
		Invoke ("SpecialAttack",Random.Range(minSpecialAttackTime,maxSpecialAttackTime));
		MusicController.instance.PlaySong (MusicController.instance.bossSong);
	}

	void SpecialAttack()
	{
		if(!isDead&&!highDamage&&Mathf.Abs(targetDistance.x)>1f)// if the target is far enough away perform charge attack
        {
			anim.SetTrigger("Boomerang");
			//Debug.Log("SpecialAttack");
		}
		Invoke ("SpecialAttack",Random.Range(minSpecialAttackTime,maxSpecialAttackTime)); // perform attack at random time between two set values
    }

	void SpecialSpeed()
	{
		currentSpeed = 7 * maxSpeed;
	}

	void BossDefeated()
	{
		MusicController.instance.PlaySong (MusicController.instance.levelClearSong);
		UIManager.instance.UpdateDisplayMessage ("DESTROYED");
		Invoke ("LoadScene",8f);
	}
	void LoadScene()
	{
		SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex + 1);
	}
}
