using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxInformation : MonoBehaviour {

	Vector2 knockback;
	float damage = 12f;
	//baseAngle must be in quadrant 1 or 4 (-pi/2 to pi/2)
	float baseAngle = 45;
	float angleTransform;
	float baseHitstun = .2f;
	float baseKnockback = .5f;
	float knockbackMagnitude;
	float knockbackDirection;
	void Start()
	{
		knockbackMagnitude = 0;
		knockbackDirection = 1;
		angleTransform = (baseAngle * Mathf.PI) / 180;
	}
	public Vector2 CalculateKnockback(float pPercentage, float pWeight, Collider2D hurtBox)
	{
		// knockbackMagnitude = (pPercentage/100)*pWeight*(damage/10) + baseKnockback;
		knockbackMagnitude = 20;
		Vector2 difference = hurtBox.transform.position - transform.position;
		//direction of knockback (left or right)
		knockbackDirection = Mathf.Sign(difference.x);
		if(knockbackDirection < 0)
		{
			if(baseAngle > 0)
			{
				//puts angle in 2nd quadrant
				angleTransform = Mathf.PI - angleTransform;
			}
			else if(baseAngle < 0)
			{
				//puts angle in 3rd quadrant
				angleTransform = 3*Mathf.PI - angleTransform;
			}
		}
		knockback.x = Mathf.Cos(angleTransform)*knockbackMagnitude;
		knockback.y = Mathf.Sin(angleTransform)*knockbackMagnitude;

		return knockback;
	}
	public float calculateHitstun(float pPercentage)
	{
		return baseHitstun;
	}
	public float getDamage()
	{
		return damage;
	}
}
