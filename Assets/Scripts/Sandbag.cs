using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//maybe hitstun should end when velocity.y = 0 (either hit the ground or at top of knockback arc)
//knockback is kinda balloony


[RequireComponent (typeof (Controller2D))]
public class Sandbag : MonoBehaviour, IHittable {

	public float jumpHeight = 4;
	public float timeToJumpApex = .4f;
	float maxSpeedGrounded = 8f;
	float maxSpeedAerial = 3f;
	float baseAcceleration = .2f;
	bool doubleJump = false;
	bool inKnockback = false;
	bool stopMoving = false;
	int frames;
	float gravity;
	float jumpVelocity;
	Vector2 velocity;
	Vector2 knockbackForce;
	Controller2D controller;
	Vector2 directionalInput;
	public float playerPercentage {get;set;}
	public float playerWeight {get;set;}
	public Text OnScreenDebugInfo;
	void Start () 
	{
		controller = GetComponent<Controller2D>();

		gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

		frames = 0;

		knockbackForce = Vector2.zero;
		velocity = Vector2.zero;

		OnScreenDebugInfo.text = "";
		playerPercentage = 0;
		playerWeight = 2;
	}
	void Update()
	{
		CalculateVelocity();
		controller.Move(velocity * Time.deltaTime, directionalInput, false, inKnockback);
		UpdateCollisionBools();

		UpdateDebugInformation();
	}
	public void SetDirectionalInput (Vector2 input)
	{
		directionalInput = input;
	}
	void CalculateVelocity()
	{
		if(!inKnockback)
		{
			//calculate x velocity
			if(directionalInput.x != 0)
			{	
				velocity.x += directionalInput.x * baseAcceleration;
				stopMoving = false;
			}
			else
			{
				if(controller.collisions.below)
				{
				StopMovement();
				}
			}
			if(controller.collisions.below)
			{
				velocity.x = Mathf.Clamp(velocity.x, -maxSpeedGrounded, maxSpeedGrounded);
			}
			else
			{
				velocity.x = Mathf.Clamp(velocity.x, -maxSpeedAerial, maxSpeedAerial);
			}
			//calculate y velocity
			//Jump();//test pre and post velocity.x calculations
			velocity.y += gravity * Time.deltaTime;
		}
		else
		{
			velocity.y += gravity * Time.deltaTime;
		}
	}
	void UpdateCollisionBools()
	{
		if(controller.collisions.below)
		{
			velocity.y = 0;
			doubleJump = true;
		}
	}
	void Jump()
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
				if(doubleJump)
				{
					//double jump cancels x momentum
					velocity.x = directionalInput.x * baseAcceleration;
					velocity.y = jumpVelocity;
					doubleJump = false;
				}
			}
		}
	}
	public void GotHit(Vector2 knockback, float hitstun, float damage)
	{
		inKnockback = true;
		velocity = knockback;
		knockbackForce = knockback;
		playerPercentage += damage;
		UpdateDebugInformation();
		if(inKnockback)
		{
			Invoke("resetInKnockbackBool", hitstun);
		}

	}
	void StopMovement()
	{
		if(!stopMoving)
		{
			frames++;
			//number of frames should depend on player weight eventually
			if(frames == 20)
			{
				stopMoving = true;
				frames = 0;
			}
		}
		else
		{
			velocity.x = 0;
		}
	}
	void resetInKnockbackBool()
	{
		inKnockback = false;
	}
	void UpdateDebugInformation()
	{
		OnScreenDebugInfo.text = "Player % = "+ playerPercentage 
								 +"\nvelocity (x) = "+ velocity.x;
	}
}
