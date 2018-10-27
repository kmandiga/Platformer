using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//how to get the direction the hit came from

public class HurtboxController : MonoBehaviour {

	Player player;
	bool hit = false;
	Collider2D hitbox;
	Collider2D thisCollider;
	HitboxInformation hitboxInfo;
	Vector2 knockback;
	float hitstun;

	void Start()
	{
		player = GetComponentInParent<Player>();
		thisCollider = GetComponent<Collider2D>();
	}

	// void OnTriggerEnter2D(Collider2D other)
	// {
	// 	hit = true;
	// 	hitbox = other.GetComponent<Collider2D>();
	// 	player.gotHit(hit, hitbox);
	// }
	void OnTriggerEnter2D(Collider2D other)
	{
		hit = true;
		hitboxInfo = other.GetComponent<HitboxInformation>();
		knockback = hitboxInfo.CalculateKnockback(player.playerPercentage, player.playerWeight, thisCollider);
		hitstun = hitboxInfo.calculateHitstun(player.playerPercentage);
		player.gotHit(knockback, hitstun);
	}
}
