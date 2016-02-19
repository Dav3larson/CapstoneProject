using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformController : RaycastController {

    public Vector3 move;
    public LayerMask passengerMask;

    public override void Start()
    {
        base.Start();

    }

    // Update is called once per frame
    void Update ()
    {
        UpdateRaycastOrigins();
        Vector3 velocity = move * Time.deltaTime;
        movePassengers(velocity);
        transform.Translate(velocity);
	}
    //moves any Controller2D on the platform
    void movePassengers(Vector3 velocity)
    {
        //fast at adding things
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);


        //first we handle vertical collisions 
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + SKINWIDTH;
            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;//if we're going down(-1) the rays should point downward. If we're going up(1) the rays should point up.
                rayOrigin += Vector2.right * (verticalRaySpacing * i);//now that we chose the Y, this line finds the X along the collider.

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);
                Debug.DrawRay(rayOrigin, Vector2.up * rayLength * directionY, Color.red);
                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushY = velocity.y - (hit.distance - SKINWIDTH * directionY);
                        float pushX;//passengers will only be pushed with the platform if they're on the top
                        if (directionY == 1)
                        {
                            pushX = velocity.x;
                        }
                        else
                        {
                            pushX = 0;
                        }
                        hit.transform.Translate(new Vector3(pushX, pushY));
                    }
                }
            }
        }
        //Now we work on Horizontal collisions i.e. pushing them to the left/right
        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + SKINWIDTH;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;//if we're going left(-1) the rays should point left. If we're going right(1) the rays should point right.
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);//now that we chose the X, this line finds the Ys along the collider.

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);
                Debug.DrawRay(rayOrigin, Vector2.right * rayLength * directionX, Color.red);
                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x - (hit.distance - SKINWIDTH * directionX);
                        
                        hit.transform.Translate(new Vector3(pushX, 0));
                    }
                }
            }
        }
        //check if they're standing on top, even when we're going down
        if (directionY == -1|| velocity.y == 0 && velocity.x != 0)
        {
            float rayLength = 2*SKINWIDTH;
            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
                rayOrigin += Vector2.right * (verticalRaySpacing * i);//now that we chose the Y, this line finds the X along the collider.

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);
                Debug.DrawRay(rayOrigin, Vector2.up * rayLength * directionY, Color.red);
                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushY = velocity.y - (hit.distance - SKINWIDTH * directionY);
                        float pushX = velocity.x; 
                        
                        hit.transform.Translate(new Vector3(pushX, pushY));
                    }
                }
            }
        }
    }
}
