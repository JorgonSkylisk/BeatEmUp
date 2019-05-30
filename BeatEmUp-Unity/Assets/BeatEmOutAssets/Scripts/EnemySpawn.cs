using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour 
{
	public float maxZ,minZ;
	public GameObject[] enemy; 
	public int numberOfEnemies;
	public float spawnTime;

	private int currentEnemies;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(currentEnemies >= numberOfEnemies)
		{
			int enemies = FindObjectsOfType<Enemy>().Length;
			if(enemies <= 0) // if no enemies left end spawner
			{
				FindObjectOfType<ResetCameraScript>().Active();
				gameObject.SetActive(false);
			}
		}
	}

	void SpawnEnemy()
	{
		bool positionX = Random.Range (0, 2) == 0 ? true : false; // random range for spawning on left or right side of the screen
		Vector3 spawnPosition;
		spawnPosition.z = Random.Range (minZ,maxZ);
		if(positionX)
		{
			spawnPosition = new Vector3(transform.position.x+10,-8,spawnPosition.z);
		}
		else
		{
			spawnPosition = new Vector3(transform.position.x-10,-8,spawnPosition.z);
		}
		Instantiate (enemy[Random.Range(0,enemy.Length)],spawnPosition,Quaternion.identity); // randomly select enemy type
		currentEnemies++;
		if(currentEnemies < numberOfEnemies) // keep spawning enemies at set intervals till the max number for the encounter has been fulfilled
		{
			Invoke ("SpawnEnemy",spawnTime);
		}
	}

	private void OnTriggerEnter(Collider other) // when player enters the area
	{
		if(other.CompareTag("Player"))
		{
			GetComponent<BoxCollider>().enabled = false; // turn off collider
			FindObjectOfType<CameraFollow>().maxXAndY.x = transform.position.x; // lock camera in place
			SpawnEnemy();
		}
	}
}
