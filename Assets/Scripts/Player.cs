using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Things to do:
//disable player input while in knockback [x]
//update calculate velocity (add a force to send player back: based on percentage) [half done]
//reset isHit bool [done]
//need to cast rays in both horizontal directions while in knockback (optional, necesary if there will be walls)
//rework physics engine for smooth hit arcs
//***************************
//add boundaries to the stage to destroy player

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

	public float jumpHeight = 4;
	public float timeToJumpApex = .4f;

	float groundSpeed = 10;
	float airSpeed = 3;

	bool doubleJump = false;

	bool inKnockback = false;
	Collider2D lastAttack;
	float knockbackDirection = 0;
	
	float gravity;
	float jumpVelocity;
	Vector3 velocity;

	Controller2D controller;

	Vector2 directionalInput;
	
	void Start () 
	{
		controller = GetComponent<Controller2D>();

		gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
	}
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			if(controller.collisions.below)
			{
				velocity.y = jumpVelocity;
				doubleJump = true;
			}
			else
			{
				if(doubleJump && !controller.collisions.below)
				{
					velocity.y = jumpVelocity;
					doubleJump = false;
				}
			}
		}

		CalculateVelocity();

		controller.Move(velocity * Time.deltaTime, directionalInput, false, inKnockback);

		//split into 2 if statements to add double jump for when running off the ground
		if(controller.collisions.above)
		{
			velocity.y = 0;
		}
		if(controller.collisions.below)
		{
			velocity.y = 0;
			doubleJump = true;
		}
	}
	public void SetDirectionalInput (Vector2 input)
	{
		directionalInput = input;
	}

	//
	void CalculateVelocity()
	{
		if(!inKnockback)
		{
			if(controller.collisions.below)
			{
				velocity.x = directionalInput.x * groundSpeed;
			}
			else
			{
				velocity.x = directionalInput.x * airSpeed;
			}
			velocity.y += gravity * Time.deltaTime;
			if(Mathf.Sign(directionalInput.x) < 0)
			{
				controller.SpriteFacingRight = false;
				velocity.x = -velocity.x;
			}
			if(Mathf.Sign(directionalInput.x) > 0 && directionalInput.x != 0)
			{
				controller.SpriteFacingRight = true;
			}
		}
		if(inKnockback)
		{
			velocity.x = knockbackDirection * airSpeed * 5;
			velocity.y = Vector2.up.y * airSpeed * 5;
			velocity.y += gravity * Time.deltaTime;
			if(!controller.SpriteFacingRight)
			{
				velocity.x = -velocity.x;
			}
		}
	}
	public void gotHit(bool isHit, Collider2D attack)
	{
		inKnockback = isHit;
		lastAttack = attack;
		if(inKnockback)
		{
			//replace .5f with a knockback formula
			Invoke("resetInKnockbackBool", .2f);
			Vector2 difference = transform.position - attack.transform.position;
			knockbackDirection = Mathf.Sign(difference.x);
		}
	}
	
	void resetInKnockbackBool()
	{
		inKnockback = false;
	}
}
