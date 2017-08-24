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

    void OnCollisonEnter (Collision col)
	{
		Debug.Log("Hit player");
		if (ready)
		{
            pc.ApplyDamage(10.0f);
		    anim.SetBool("triggered", true);
			ready = false;
			StartCoroutine(ResetTrap());
		}
	}

	IEnumerator ResetTrap()
	{
		yield return new WaitForSeconds(2.0f);
		anim.SetBool("triggered", false);
		ready = true;
	}
}
