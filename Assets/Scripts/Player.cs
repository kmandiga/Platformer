using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

	public float jumpHeight = 4;
	public float timeToJumpApex = .4f;
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
	float moveSpeed = 6;

	bool doubleJump = false;	
	
	float gravity;
	float jumpVelocity;
	Vector3 velocity;
	float velocityXSmoothing;

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
		// if(Input.GetKeyDown(KeyCode.Space) && controller.collisions.below)
		// {
		// 	velocity.y = jumpVelocity;
		// }

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

		controller.Move(velocity * Time.deltaTime, directionalInput);

		//fix for accumulation of gravitational force
		// if(controller.collisions.above || controller.collisions.below)
		// {
		// 	velocity.y = 0;
		// }
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
		//float targetVelocityX = directionalInput.x * moveSpeed;
		//smooths turning in x direction. It is set to be different for when in the air or grounded. May or may not actually want to implement this
		//velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
		velocity.x = directionalInput.x * moveSpeed;
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
}
