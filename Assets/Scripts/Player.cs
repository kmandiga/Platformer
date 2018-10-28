using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//drift in the air (after getting hit could be improved)
//make player percentage a factor

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

	public float jumpHeight = 4;
	public float timeToJumpApex = .4f;
	float groundSpeed = 10;
	float airSpeed = 6;
	bool doubleJump = false;
	bool inKnockback = false;
	float gravity;
	float jumpVelocity;
	Vector2 velocity;
	Vector2 velocityOld;
	Vector2 knockbackForce;
	Controller2D controller;
	Vector2 directionalInput;
	public float playerPercentage = 0;
	public float playerWeight = 2;
	
	void Start () 
	{
		controller = GetComponent<Controller2D>();

		gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

		knockbackForce = Vector2.zero;
		velocity = Vector2.zero;
		velocityOld = Vector2.zero;
	}
	
	void Update()
	{
		if(Input.GetButtonDown("Jump"))
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
					velocity.x = 0;
					velocityOld.x = 0;
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
				if(directionalInput.x == 0)
				{
					velocity.x = velocityOld.x;
				}
				else
				{
					velocity.x = directionalInput.x * airSpeed;
				}
			}
			velocity.y += gravity * Time.deltaTime;
			//always wants to be done post velocity calculations
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
			velocity.x = knockbackForce.x * airSpeed;
			velocity.y = knockbackForce.y * airSpeed;
			//velocity.y += gravity * Time.deltaTime;
			if(!controller.SpriteFacingRight)
			{
				velocity.x = -velocity.x;
			}
		}
		velocityOld = velocity;
	}
	public void gotHit(Vector2 knockback, float hitstun)
	{
		inKnockback = true;
		knockbackForce = knockback;
		if(inKnockback)
		{
			Invoke("resetInKnockbackBool", hitstun);
		}

	}
	
	void resetInKnockbackBool()
	{
		inKnockback = false;
	}
}
