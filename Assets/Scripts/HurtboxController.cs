using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtboxController : MonoBehaviour {

	IHittable player;
	Collider2D hitbox;
	Collider2D thisCollider;
	HitboxInformation hitboxInfo;
	Vector2 knockback;
	float hitstun;

	void Start()
	{
		player = GetComponentInParent<IHittable>();
		thisCollider = GetComponent<Collider2D>();
	}
	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.layer == 10 && other.transform.parent != transform.parent)
		{
			hitboxInfo = other.GetComponent<HitboxInformation>();
			knockback = hitboxInfo.CalculateKnockback(player.percentage, player.weight, thisCollider);
			hitstun = hitboxInfo.calculateHitstun(player.percentage);
			player.GotHit(knockback, hitstun, hitboxInfo.getDamage());
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
