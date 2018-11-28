using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//maybe hitstun should end when velocity.y = 0 (either hit the ground or at top of knockback arc)
//knockback is kinda balloony


[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour, IHittable {

	public float jumpHeight = 4;
	public float timeToJumpApex = .4f;
	float maxSpeedGrounded = 8f;
	float maxSpeedAerial = 5f;
	
	bool inKnockback = false;
	bool stopMoving = false;
	int frames;
	public float gravity;
	public float jumpVelocity;
	Vector2 velocity;
	Vector2 knockbackForce;
	Controller2D controller;
	Vector2 directionalInput;
	public float percentage {get;set;}
	public float weight {get;set;}
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
		percentage = 0;
		weight = 2;
	}
	void Update()
	{
		CalculateVelocity();
		controller.Move(velocity * Time.deltaTime, directionalInput, false, inKnockback);
		UpdateCollisionBools();
		UpdateDebugInformation();
	}
	public void setVelocity(float vx, float vy)
	{
		velocity.x = vx;
		velocity.y = vy;
	}
	public Vector2 getVelocity()
	{
		return velocity;
	}
	public void SetDirectionalInput (Vector2 input)
	{
		directionalInput = input;
	}
	public void CalculateVelocity()
	{
		if(!inKnockback)
		{
			//calculate x velocity
			if(directionalInput.x != 0)
			{	
				velocity.x= directionalInput.x * maxSpeedGrounded;
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
		}
	}
	public void GotHit(Vector2 knockback, float hitstun, float damage)
	{
		inKnockback = true;
		velocity = knockback;
		knockbackForce = knockback;
		percentage += damage;
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
		OnScreenDebugInfo.text = "Player % = "+ percentage 
								 +"\nvelocity (x) = "+ velocity.x;
	}
}
