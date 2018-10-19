using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Controller2D : RaycastController {

	float maxClimbAngle = 80;
	float maxDescendAngle = 80;

	public CollisionInfo collisions;
	[HideInInspector]
	public Vector2 playerInput;

	public override void Start()
	{
		base.Start();
	}
	public void Move(Vector2 moveAmount, bool standingOnPlatform)
	{
		Move(moveAmount, Vector2.zero, standingOnPlatform);
	}
	public void Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false)
	{
		//ref keyword: instead of a copy of the moveAmount variable being created to pass to vertical collisions, it will now reference the original variable
		UpdateRaycastOrigins();
		collisions.Reset();
		collisions.moveAmountOld = moveAmount;
		playerInput = input;

		if(moveAmount.y < 0)
		{
			DescendSlope(ref moveAmount);
		}
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
			//Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

			if(hit)
			{
				if(hit.collider.tag == "Through")
				{
					continue;
				}
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
				//check to see if on slope
				if(i == 0 && slopeAngle <= maxClimbAngle)
				{
					if(collisions.descendingSlope)
					{
						collisions.descendingSlope = false;
						moveAmount = collisions.moveAmountOld;
					}
					float distanceToSlopeStart = 0;
					//this is a check to see if the ray collided with a slope and to remove any distance between object and slope before moving up the slope
					if(slopeAngle != collisions.slopeAngleOld)
					{
						distanceToSlopeStart = hit.distance-skinWidth;
						moveAmount.x -= distanceToSlopeStart*directionX;
					}
					ClimbSlope(ref moveAmount, slopeAngle);
					moveAmount.x += distanceToSlopeStart * directionX;
				}

				if(!collisions.climbingSlope || slopeAngle > maxClimbAngle)
				{
					moveAmount.x = (hit.distance - skinWidth) * directionX;
					//need to set this so if object is hanging off a ledge and there is an object below it, it will be ignored
					rayLength = hit.distance;

					if(collisions.climbingSlope)
					{
						moveAmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad)*Mathf.Abs(moveAmount.x);
					}

					collisions.left = directionX == -1;
					collisions.right = directionX == 1;
				}
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
			//Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

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

				if(collisions.climbingSlope)
				{
					moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad)*Mathf.Sign(moveAmount.x);
				}

				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
			}
		}
		//if changing slopes, this fixes an issue where sometimes the block gets stuck on the slope for a frame or two while it readjusts
		if (collisions.climbingSlope)
		{
			float directionX = Mathf.Sign(moveAmount.x);
			rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
			Vector2 rayOrigin = ((directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
			RaycastHit2D hit =Physics2D.Raycast(rayOrigin,Vector2.right * directionX, rayLength, collisionMask);

			if(hit)
			{//get angle of hit
				float slopeAngle = Vector2.Angle(hit.normal,Vector2.up);
				//if collided with a new slope
				if(slopeAngle != collisions.slopeAngle)
				{
					moveAmount.x = (hit.distance - skinWidth) * directionX;
					collisions.slopeAngle = slopeAngle;
				}
			}
		}
	}

	void ClimbSlope(ref Vector2 moveAmount, float slopeAngle)
	{
		float moveDistance = Mathf.Abs(moveAmount.x);
		float climbVelocityY =  Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
		if(moveAmount.y <= climbVelocityY)
		{
			//can assume jumping if true
			moveAmount.y = climbVelocityY;
			moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
			//because we are in climbslope method, we can assume we are grounded, even though the y moveAmount is not zero
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
		}
		
	}

	void DescendSlope(ref Vector2 moveAmount)
	{
		float directionX = Mathf.Sign (moveAmount.x);
		Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomRight:raycastOrigins.bottomLeft;
		RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

		if(hit)
		{
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if(slopeAngle != 0 && slopeAngle <= maxDescendAngle)
			{//need to check if there we are going from a descending slope to a rising slope, if so fix the moveAmount to the old moveAmount so the object does not slow down before climbing a slope
				if(Mathf.Sign(hit.normal.x) == directionX)
				{
					if(hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
					{
						float moveDistance = Mathf.Abs(moveAmount.x);
						float descendVelocityY =  Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
						moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
						moveAmount.y -= descendVelocityY;

						collisions.slopeAngle = slopeAngle;
						collisions.descendingSlope = true;
						collisions.below = true;
					}
				}
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

		public bool climbingSlope;
		public bool descendingSlope;
		public float slopeAngle, slopeAngleOld;
		public Vector2 moveAmountOld;
		public bool fallingThroughPlatform;

		public void Reset()
		{
			above = below = false;
			left = right = false;
			climbingSlope = descendingSlope = false;

			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}
	}
	
}
