using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtboxController : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other)
	{
		Debug.Log("Hurtbox OnTriggerEnter has been triggered");
	}
}
