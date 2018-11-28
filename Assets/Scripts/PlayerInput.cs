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
	float jumpVelocity;
	Controller2D controller;
	void Start () 
	{
		player = GetComponent<Player>();
		animator = GetComponent<Animator>();
		controller = GetComponent<Controller2D>();
	}
	void Update () 
	{
		// Vector2 directionalInput =  new Vector2 (Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		// player.SetDirectionalInput(directionalInput);

		if(Input.touchCount > 0)
		{
			Touch myTouch = Input.touches[0];
			if(myTouch.phase == TouchPhase.Began)
			{
				touchOrigin = myTouch.position;
			}
			else if(myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
			{
				Vector2 touchEnd = myTouch.position;

				float x = touchEnd.x - touchOrigin.x;
				float y = touchEnd.y - touchOrigin.y;

				touchOrigin.x = -1;

				if(Mathf.Abs(x) > Mathf.Abs(y))
				{
					directionalInput.x = x > 0 ? 1 : -1;
				}
				else
				{
					directionalInput.y = y > 0 ? 1 : -1;
					Jump();
				}
			}
		}
		else
		{
			touchOrigin = -Vector2.one;
			directionalInput = Vector2.zero;
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
		if(directionalInput.y > 0)
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
}
