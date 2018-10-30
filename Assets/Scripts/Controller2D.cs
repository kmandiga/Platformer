using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Controller2D : RaycastController {
	public CollisionInfo collisions;

	[HideInInspector]
	public Vector2 playerInput;

	Transform transform;
	bool inKnockback = false;

	public override void Start()
	{
		base.Start();
		transform = GetComponent<Transform>();
	}
	public void Move(Vector2 moveAmount, bool standingOnPlatform, bool hit)
	{
		Move(moveAmount, Vector2.zero, standingOnPlatform, inKnockback);
	}
	public void Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform, bool hit)
	{
		//ref keyword: instead of a copy of the moveAmount variable being created to pass to vertical collisions, it will now reference the original variable
		UpdateRaycastOrigins();
		collisions.Reset();
		collisions.moveAmountOld = moveAmount;
		playerInput = input;
		inKnockback = hit;

		if(moveAmount.x != 0)
		{
			HorizontalCollisions(ref moveAmount);
		}

		if(moveAmount.y != 0)
		{
			VerticalCollisions (ref moveAmount);
		}
		
		transform.Translate(moveAmount);

		if(standingOnPlatform)
		{
			collisions.below = true;
		}
	}
	//ref keyword: see in move function. Any change to the moveAmount variable will change it in memory
	void HorizontalCollisions(ref Vector2 moveAmount)
	{
		float directionX = Mathf.Sign(moveAmount.x);
		float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

		for(int i = 0; i < horizontalRayCount; i++)
		{
			//start raycast origin on bottom left if moving left and start on bottom right if moving right
			Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
			
			//draws rays for visibility on scene view. enable gizmos to see on game view
			Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

			if(hit)
			{
				if(hit.collider.tag == "Through")
				{
					continue;
				}
				moveAmount.x = (hit.distance - skinWidth) * directionX;
			}
			
		}
	}

	void VerticalCollisions(ref Vector2 moveAmount)
	{
		float directionY = Mathf.Sign(moveAmount.y);
		float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;


		for(int i = 0; i < verticalRayCount; i++)
		{
			//start raycast origin on bottom left if moving down and start on top left if moving up
			Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
			
			//draws rays for visibility on scene view
			Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

			if(hit)
			{
				if(hit.collider.tag == "Through")
				{
					if(directionY == 1 || hit.distance == 0)
					{
						continue;
					}
					if(collisions.fallingThroughPlatform)
					{
						continue;
					}
					if(playerInput.y == -1)
					{
						collisions.fallingThroughPlatform = true;
						Invoke("ResetFallingThroughPlatform", .1f);
						continue;
					}
				}
				moveAmount.y = (hit.distance - skinWidth) * directionY;
				//need to set this so if object is hanging off a ledge and there is an object below it, it will be ignored
				rayLength = hit.distance;

				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
			}
		}
	}
	void ResetFallingThroughPlatform()
	{
		collisions.fallingThroughPlatform = false;
	}

	//public information to know if there is a collision on some part of the player
	public struct CollisionInfo 
	{
		public bool above, below;
		public bool left, right;
		public Vector2 moveAmountOld;
		public bool fallingThroughPlatform;


		public void Reset()
		{
			above = below = false;
			left = right = false;
		}
	}
}
