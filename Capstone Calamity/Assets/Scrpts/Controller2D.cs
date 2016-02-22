using UnityEngine;
using System.Collections;


[RequireComponent (typeof (BoxCollider2D))]//Doing this ensures the gameobject this is attached to will have a BoxCollider2D
public class Controller2D : RaycastController {

	
	
	
    public int hitPoints = 10;

    float maxClimbAngle = 80;
    float maxDescendAngle = 75;


	
    public CollisionInfo collisions; 


	// Use this for initialization


	
    public struct CollisionInfo
    {
        public bool above, below, left, right,isClimbing,isDescending;
        public float slopeAngle, slopeAngleOld;
        public Vector3 velocityOld;
        public void Reset()
        {
            isDescending = false;
            isClimbing = false;
            above = false;
            below = false;
            left = false;
            right = false;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }

    
	//these will give us the updated locations of the rays in relation to the player
	

	public void Move(Vector3 velocity, bool standignOnPlatform=false)
	{
		UpdateRaycastOrigins ();
        collisions.Reset();//so its a fresh slate each time
        collisions.velocityOld = velocity;
        if(velocity.y<0)
        {
            DescendSlope(ref velocity);
        }
		if (velocity.x != 0)
		{
			HorizontalCollisions (ref velocity);
		}

        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }
        
        transform.Translate (velocity);
        if(standignOnPlatform)
        {
            collisions.below = true;
        }
	}

	void VerticalCollisions(ref Vector3 velocity)//shallow copy
	{
		float directionY = Mathf.Sign (velocity.y);
		float rayLength = Mathf.Abs (velocity.y)+SKINWIDTH;

		for(int i=0;i<verticalRayCount;i++)
		{
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;//if we're going down(-1) the rays should point downward. If we're going up(1) the rays should point up.
			rayOrigin+= Vector2.right*(verticalRaySpacing*i);//now that we chose the Y, this line finds the X along the collider.
            
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
			Debug.DrawRay(rayOrigin,Vector2.up*rayLength*directionY,Color.red);

			if (hit)//if the raycast hits something
			{
				//           how far from the ray....and which direction
				velocity.y = (hit.distance-SKINWIDTH)*directionY;
				rayLength = hit.distance; //this will ensure only the smallest length will be used. in other words the closest obstacle
               
                if (collisions.isClimbing)
                {
                    velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad )* Mathf.Sign(velocity.x);
                   
                }
                if (directionY==-1)
                {
                    collisions.below = true;
                }
                else
                {
                    collisions.above = true;
                }
			}
		}
        if(collisions.isClimbing)//if we're climbing and we hit another slope, we'll want to switch slopes smoothly
        {
            float directionX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x + SKINWIDTH);
            Vector2 rayOrigin;
            if (directionX < 0)
            {
                rayOrigin = raycastOrigins.bottomLeft + Vector2.up * velocity.y;
            }
            else
            {
                rayOrigin = raycastOrigins.bottomRight + Vector2.up * velocity.y;
            }
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            if(hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if(slopeAngle!=collisions.slopeAngle)//specifically THIS is where we handle the new slope
                {
                    velocity.x = hit.distance - SKINWIDTH * directionX;
                    collisions.slopeAngle = slopeAngle;
                }

            }
            
        }
	}


	void HorizontalCollisions(ref Vector3 velocity)//shallow copy
	{
		float directionX = Mathf.Sign (velocity.x);
		float rayLength = Mathf.Abs (velocity.x)+SKINWIDTH;

		for(int i=0;i<horizontalRayCount;i++)
		{
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;//if we're going left(-1) the rays should point left. If we're going right(1) the rays should point right.
			rayOrigin+= Vector2.up*(horizontalRaySpacing*i);//now that we chose the X, this line finds the Ys along the collider.

			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
			Debug.DrawRay(rayOrigin,Vector2.right*rayLength*directionX,Color.red); ;

            if (hit)//if the raycast hits something
            {
                if (hit.distance == 0)
                {
                    continue;
                }

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (i == 0 && slopeAngle <= maxClimbAngle)//i==0 because only the first line detects the slope
                {
                    if(collisions.isDescending)
                    {
                        //if we have 2 slopes sitting together in a V formation and you go down&right back up
                        //this is to keep you going at the same pace. Otherwise your X velocity will slowdown a lot.
                        collisions.isDescending = false;
                        velocity = collisions.velocityOld;

                    }
                    float distancetoSlope = 0;
                    if(slopeAngle!=collisions.slopeAngleOld)
                    {
                        
                        distancetoSlope = hit.distance-SKINWIDTH;
                        velocity.x -= distancetoSlope * directionX;//this wil make it so we will always be in contact with the slope when it brings us up
                    }
                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += (hit.distance - SKINWIDTH) * directionX;
                }
                if (!collisions.isClimbing || slopeAngle > maxClimbAngle)
                {
                   
                
                    velocity.x = (hit.distance - SKINWIDTH) * directionX;
                    rayLength = hit.distance; //this will ensure only the smallest length will be used. in other words the closest obstacle
                    if(collisions.isClimbing)
                    {
                        //this is used to check if there is another obstacle we hit on the side as we climb the slope
                        velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad * Mathf.Abs(velocity.x));
                        //Debug.Log(velocity.y);
                    }
                    if (directionX == 1)
                    {
                        collisions.right = true;
                    }
                    else
                    {
                        collisions.left = true;
                    }
                }
            }
		}
	}

    public void ClimbSlope(ref Vector3 velocity,float slopeAngle)
    {
        
        float moveDistance = Mathf.Abs(velocity.x);//This will serve as our hypotenuse. TRIGONOMETRY! 
        
        float climbY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;// Sin(Θ)*Hypotenuse= Y
        if(climbY>velocity.y)//shorthand to see if we're jumping or not
        {
            velocity.y = climbY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            collisions.isClimbing = true;
            collisions.slopeAngle = slopeAngle;
            collisions.below = true;
        }
        //velocity.y = climbY;
        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);//CAH
    }
    public void DescendSlope(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin;
        if(directionX==-1)
        {
            rayOrigin = raycastOrigins.bottomRight;// remember, on a slope. so this is the part thats actually touching
        }
        else
        {
            rayOrigin = raycastOrigins.bottomLeft;
        }
        //-Vector2.up because we're shooting downward. Mathf.Infinity because we're just shooting down as far as we can
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity,collisionMask);
        if(hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);  //remember hit.normal is perpendicular to the slope 
            if(slopeAngle!=0&&slopeAngle<=maxDescendAngle)//if its ==0 then its just a flat surface
            {
                if(Mathf.Sign(hit.normal.x)==directionX)//check if we are going down the slope
                {
                    //remember how we used Mathf.Infinity? Well now lets make sure the slope we hit is actually close
                    if(hit.distance-SKINWIDTH<=Mathf.Tan(slopeAngle*Mathf.Deg2Rad)*Mathf.Abs(velocity.x))
                    {
                        float moveDistance = Mathf.Abs(velocity.x);
                        float decendY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                        velocity.y -= decendY;
                        collisions.isDescending = true;
                        collisions.below = true;
                    }
                }
            }
        }
    }
    public void takeDamage(int dmg)
    {
       hitPoints -= dmg;
        if (hitPoints < 0)
        {
            hitPoints = 0;
        }
    }
}
