using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformController : RaycastController {

    public Vector3 move;
    public bool reversable;
    public float speed;
    int fromWaypointIndex;
    float percentBetweenWaypoints;
    public LayerMask passengerMask;
    List<PassengerMovement> passengerList;
    Dictionary<Transform, Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D>();

    public Vector3[] localWaypoints;
    public Vector3[] globalWaypoints;

    public override void Start()
    {
        base.Start();
        globalWaypoints = new Vector3[localWaypoints.Length];
        for(int i = 0; i < localWaypoints.Length; i++)
        {
            //for every local waypoints we have set, convert them to the global waypoints
            globalWaypoints[i] = localWaypoints[i] + transform.position;

        }
    }

    // Update is called once per frame
    void Update ()
    {
        UpdateRaycastOrigins();
        Vector3 velocity = GetSpeed();
        CalculateMovement(velocity);
        MovePassengers(true);
        transform.Translate(velocity);
        MovePassengers(false);
	}

    void MovePassengers(bool beforeMovePlaftorm)
    {
        foreach(PassengerMovement passenger in passengerList)
        {
            if(!passengerDictionary.ContainsKey(passenger.transform))
            {
                //by adding to the dictionary we don't have to deal with calling too many GetComponents
                passengerDictionary.Add(passenger.transform, passenger.transform.GetComponent<Controller2D>());
            }
            //if it is their turn to move we move them
            if(passenger.moveBeforePlatform==beforeMovePlaftorm)
            {
                passengerDictionary[passenger.transform].Move(passenger.velocity,passenger.standingOnPlatform);
            }
        }
    }
    Vector3 GetSpeed()
    {
        int toWaypointIndex = (fromWaypointIndex + 1)%globalWaypoints.Length;
        float distance = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
        //percentage should increase slower the further the waypoints
        percentBetweenWaypoints += Time.deltaTime * speed/distance;
        //use linear interpolation to find the point between waypoints. Our % will tell us how far we are along the way to it.
        Vector3 newPosition = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], percentBetweenWaypoints);
        if(percentBetweenWaypoints>=1)
        {
            percentBetweenWaypoints = 0;
            fromWaypointIndex++;
            if(fromWaypointIndex>=globalWaypoints.Length-1)
            {
                fromWaypointIndex = 0;
                if(reversable)
                {
                    System.Array.Reverse(globalWaypoints);
                }
                
            }
        }
        return newPosition - transform.position;
    }
    //moves any Controller2D on the platform
    void CalculateMovement(Vector3 velocity)
    {
        //fast at adding things
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        passengerList = new List<PassengerMovement>();
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
                        bool isStanding = directionY == 1;//if we hit someone and we're going up, the passenger must be standing on it.
                        //We state enter in True because either the player is on top and we push it up OR the player is below the platform and we want to move it first
                        passengerList.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), isStanding, true));
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
                Debug.DrawRay(rayOrigin, Vector2.right * rayLength * directionX, Color.white);
                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x - (hit.distance - SKINWIDTH * directionX);
                        //we pushed the passenger down ever so slightly so that the player can check if their on ground and be able to jump
                        //passenger is on the side of the platform, so we're not standing on it, hence the first false
                        //like above the passenger must be moved first to make way for the platform, so the second bool is true
                        passengerList.Add(new PassengerMovement(hit.transform, new Vector3(pushX, -SKINWIDTH), false, true));
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
                Vector2 rayOrigin = raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);//now that we chose the Y, this line finds the X along the collider.

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);
                Debug.DrawRay(rayOrigin, Vector2.up * rayLength * directionY, Color.green);
                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushY = velocity.y - (hit.distance - SKINWIDTH * directionY);
                        float pushX = velocity.x;
                        //since we are shooting rays upward if there's a hit, they are on top. first bool must be true
                        //Because we're heading down, platform must move first to make room
                        passengerList.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), true, false));
                    }
                }
            }
        }
    }
    struct PassengerMovement
    {
        public Transform transform;
        public Vector3 velocity;
        public bool standingOnPlatform;
        public bool moveBeforePlatform;

        public PassengerMovement(Transform newTransform, Vector3 newVelcoity, bool isStanding, bool moved)
        {
            transform = newTransform;
            velocity = newVelcoity;
            standingOnPlatform = isStanding;
            moveBeforePlatform = moved;
        }
    }
    void OnDrawGizmos()
    {
        if(localWaypoints!=null)
        {
            Gizmos.color = Color.white;
            float size = .3f;
            for(int i=0; i< localWaypoints.Length;i++)
            {
                //if we're playing, then we'll show the global waypoints, otherwise we'll just show the local.
                //this way you can see the changes in comparison to the platform's location but when we start playing you'll see what their locations are exactly
                Vector3 globalPoints =(Application.isPlaying)?globalWaypoints[i]: localWaypoints[i] + transform.position;
                Gizmos.DrawLine(globalPoints - Vector3.up * size, globalPoints + Vector3.up * size);
                Gizmos.DrawLine(globalPoints - Vector3.left * size, globalPoints + Vector3.left * size);
            }
        }
    }
}
