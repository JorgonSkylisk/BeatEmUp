using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranged : Enemy
{

    public GameObject boomerang;
    public float minBoomerangTime, maxBoomerangTime;

    public Transform[] buzz = new Transform[1];

    public override void Start()
    {
        base.Start();
        Invoke("ThrowBoomerang", Random.Range(minBoomerangTime, maxBoomerangTime));
    }

    GameObject tempBoomerang;

    void ThrowBoomerang()
    {
        if (!isDead && !highDamage && Mathf.Abs(targetDistance.x) > 2f)
        {
            anim.SetTrigger("Boomerang");
            for (int i = 0; i <= 0; i++)
            {
                tempBoomerang = Instantiate(boomerang, buzz[i].transform.position, buzz[i].transform.rotation);
                if (facingRight)
                {
                    tempBoomerang.GetComponent<Boomerang>().direction = 1;

                }
                else
                {
                    tempBoomerang.GetComponent<Boomerang>().direction = -1;

                }
            }
        }
        Invoke("ThrowBoomerang", Random.Range(minBoomerangTime, maxBoomerangTime));
    }


    void AddHighDamageForce()
    {
        GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, 0), ForceMode.Impulse);
    }

}