using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtboxController : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other)
	{
		Debug.Log("OnTriggerEnter Method");
	}
	void OnTriggerStay2D(Collider2D other)
	{
		Debug.Log("OnTriggerStay Method");
	}
	void OnTriggerExit2D(Collider2D other)
	{
		Debug.Log("OnTriggerExit Method");
	}
}
