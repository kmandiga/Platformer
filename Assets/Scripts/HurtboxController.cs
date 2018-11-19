using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.layer == 10 && other.transform.parent != transform.parent)
		{
			// hit = true;
			// hitboxInfo = other.GetComponent<HitboxInformation>();
			// knockback = hitboxInfo.CalculateKnockback(player.playerPercentage, player.playerWeight, thisCollider);
			// hitstun = hitboxInfo.calculateHitstun(player.playerPercentage);
			// player.gotHit(knockback, hitstun, hitboxInfo.getDamage());

			Destroy(transform.parent.gameObject);
		}
	}
	void OnTriggerExit2D(Collider2D other)
	{
		if(other.gameObject.layer == 12)
		{
			Destroy(transform.parent.gameObject);
		}
	}
}
