using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heavy : Enemy
{

    public float minSpecialAttackTime, maxSpecialAttackTime;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        Invoke("SpecialAttack", Random.Range(minSpecialAttackTime, maxSpecialAttackTime));
    }

    void SpecialAttack()
    {
        if (!isDead && !highDamage && Mathf.Abs(targetDistance.x) > 1f)
        {
            anim.SetTrigger("Boomerang");
            //Debug.Log("SpecialAttack");
        }
        Invoke("SpecialAttack", Random.Range(minSpecialAttackTime, maxSpecialAttackTime));
    }

    void SpecialSpeed()
    {
        currentSpeed = 3 * maxSpeed;
    }


}