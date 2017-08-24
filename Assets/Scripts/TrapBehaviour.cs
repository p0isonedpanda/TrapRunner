using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBehaviour : MonoBehaviour
{
	playerController pc;

	Animator anim;
	bool ready = true;

    // Initialise variables
    void Start ()
	{
		pc = playerController.instance;
		anim = GetComponent<Animator>();
	}	

    void OnTriggerEnter (Collider col)
	{
		Debug.Log("Hit player");
		if (ready)
		{
            pc.ApplyDamage(10.0f);
		    anim.SetBool("triggered", true);
			ready = false;
			pc.trapped = true;
			StartCoroutine(ResetTrap());
		}
	}

	IEnumerator ResetTrap()
	{
		yield return new WaitForSeconds(2.0f);
		anim.SetBool("triggered", false);
		ready = true;
		pc.trapped = false;
	}
}
