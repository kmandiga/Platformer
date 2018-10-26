using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//how to get the direction the hit came from

public class HurtboxController : MonoBehaviour {

	Player player;
	bool hit = false;

	Collider2D hitbox;
	void Start()
	{
		player = GetComponentInParent<Player>();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		hit = true;
		hitbox = other.GetComponent<Collider2D>();
		player.gotHit(hit, hitbox);
	}
}
