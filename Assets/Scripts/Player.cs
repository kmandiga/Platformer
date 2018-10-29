using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//maybe hitstun should end when velocity.y = 0 (either hit the ground or at top of knockback arc)

//movement was underestimated in complexity
//new approach: come up with an fsm for possible states
//				look at forces acting on char at each state

//idea to decouple animation and movement
//			Instead of flipping whole gameobject, calculate transforms
//			of child hitboxes and hurtboxes and updata them based on direction


[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

	public float jumpHeight = 4;
	public float timeToJumpApex = .4f;
	float baseSpeed = 1f;
	float baseAcceleration = .2f;
	bool doubleJump = false;
	bool inKnockback = false;
	bool startGravity = false;
	float gravity;
	float jumpVelocity;
	Vector2 velocity;
	Vector2 prevGlobalVelocity;
	Vector2 knockbackForce;
	Controller2D controller;
	Vector2 directionalInput;
	public float playerPercentage = 0;
	public float playerWeight = 2;
	public Text percentageOnScreen;
	void Start () 
	{
		controller = GetComponent<Controller2D>();

		gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

		knockbackForce = Vector2.zero;
		velocity = Vector2.zero;
		prevGlobalVelocity = directionalInput;

		percentageOnScreen.text = "";
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
		//calculate x velocity
		if(directionalInput.x != 0)
		{
			if(velocity.x < 1f && velocity.x > -1f)
			{
				velocity.x = directionalInput.x * baseSpeed;
			}
			else
			{		
				velocity.x += directionalInput.x * baseAcceleration;
			}
		}
		//calculate y velocity
		Jump();//test pre and post velocity.x calculations
		velocity.y += gravity * Time.deltaTime;
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
					velocity.x = 0;
					velocity.y = jumpVelocity;
					doubleJump = false;
				}
			}
		}
	}
	public void gotHit(Vector2 knockback, float hitstun, float damage)
	{
		inKnockback = true;
		startGravity = false;
		knockbackForce = knockback;
		playerPercentage += damage;
		UpdateDebugInformation();
		if(inKnockback)
		{
			Invoke("resetInKnockbackBool", hitstun);
		}

	}
	void resetInKnockbackBool()
	{
		inKnockback = false;
	}
	void UpdateDebugInformation()
	{
		percentageOnScreen.text = "Player % = "+ playerPercentage 
								 +"\nvelocity (x) = "+ velocity.x
								 +"\nglobal velocity (x) = "+ prevGlobalVelocity.x;
	}
}
