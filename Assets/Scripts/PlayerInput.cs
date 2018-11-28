using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Player))]
public class PlayerInput : MonoBehaviour 
{
	Player player;
	Vector2 touchOrigin = -Vector2.one;
	Vector2 directionalInput = Vector2.zero;
	Animator animator;
	bool doubleJump = false;
	Controller2D controller;
	Touch movementTouch;
	float deadzoneX;
	float deadzoneY;
	float accumulatedDeltaX;
	float accumulatedDeltaY;
	void Start () 
	{
		player = GetComponent<Player>();
		animator = GetComponent<Animator>();
		controller = GetComponent<Controller2D>();

		deadzoneX = .05f * Screen.width;
		deadzoneY = .1f * Screen.height;

		accumulatedDeltaX = 0;
		accumulatedDeltaY = 0;
	}
	void Update () 
	{
		if(Input.touchCount > 0)
		{
			if(Input.touches[0].position.x < Screen.width / 2)
			{
				//do movement stuff if input is on left side of screen
				movementTouch = Input.touches[0];
				if(movementTouch.phase == TouchPhase.Began)
				{
					touchOrigin = movementTouch.position;
				}
				else if(movementTouch.phase == TouchPhase.Moved)
				{
					accumulatedDeltaX += movementTouch.deltaPosition.x;
					accumulatedDeltaY += movementTouch.deltaPosition.y;

					//x direction
					if(accumulatedDeltaX > deadzoneX)
					{
						directionalInput.x = 1;
					}
					else if(accumulatedDeltaX < -deadzoneX)
					{
						directionalInput.x = -1;
					}
					//y direction
					if(accumulatedDeltaY > deadzoneY)
					{
						Jump();
					}
					else if(accumulatedDeltaY < -deadzoneY)
					{
						directionalInput.y = -1;
					}
				}
				else if(movementTouch.phase == TouchPhase.Ended)
				{
					accumulatedDeltaX = 0;
					accumulatedDeltaY = 0;
					directionalInput = Vector2.zero;
				}
			}
		}

		UpdateAnimations();
		player.SetDirectionalInput(directionalInput);
		
	}
	void SetFacingDirection()
	{
		//set direction facing
		if(directionalInput.x > 0)
		{
			transform.localScale = new Vector2(1,1);
		}
		if(directionalInput.x < 0)
		{
			transform.localScale = new Vector2(-1,1);
		}
	}
	void UpdateAnimations()
	{
		if(controller.collisions.below)
		{
			SetFacingDirection();
		}
		//set animation state
		if(directionalInput.x != 0)
		{
			if(animator.GetBool("Attacking") == false)
			{
				if(Input.GetButtonDown("Fire1"))
				{
					animator.SetTrigger("Striking");
					
				}
				else if(Input.GetButtonDown("Fire2"))
				{
					animator.SetTrigger("Flykicking");
					
				}
				else if(Input.GetButtonDown("Fire3"))
				{
					animator.SetTrigger("Fireball");
					
				}
			}
			animator.SetBool("isRunning",true);
			animator.SetBool("isIdle", false);
		}
		else
		{
			if(animator.GetBool("Attacking") == false)
			{
				if(Input.GetButtonDown("Fire1"))
				{
					animator.SetTrigger("Striking");
					
				}
				else if(Input.GetButtonDown("Fire2"))
				{
					animator.SetTrigger("Flykicking");
					
				}
				else if(Input.GetButtonDown("Fire3"))
				{
					animator.SetTrigger("Fireball");
					
				}
			}
			animator.SetBool("isRunning",false);
			animator.SetBool("isIdle", true);
		}
	}
	void Jump()
	{
		if(controller.collisions.below)
		{
			player.setVelocity(player.getVelocity().x, player.jumpVelocity);
			doubleJump = true;
		}
		else
		{
			if(doubleJump)
			{
				SetFacingDirection();
				player.setVelocity(player.getVelocity().x, player.jumpVelocity);
				doubleJump = false;
			}
		}
	}
}
