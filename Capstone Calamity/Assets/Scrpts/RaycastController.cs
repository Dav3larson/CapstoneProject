using UnityEngine;
using System.Collections;


[RequireComponent (typeof (BoxCollider2D))]//Doing this ensures the gameobject this is attached to will have a BoxCollider2D
public class RaycastController : MonoBehaviour {
    
    [HideInInspector]
    public float horizontalRaySpacing;
    [HideInInspector]
    public float verticalRaySpacing;
    [HideInInspector]

    public LayerMask collisionMask;

    public BoxCollider2D thisCollider;
    public RaycastOrigins raycastOrigins;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;
    public bool inside=true;

    public const float SKINWIDTH = .015f;

    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;

    }


    public virtual void Start()
    {
        Bounds bounds = GetComponent<BoxCollider2D>().bounds;
        verticalRayCount = (int)bounds.size.x * 2;
        horizontalRayCount = (int)bounds.size.y * 2;
        thisCollider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
        //collisions = new CollisionInfo();
        //inside = true;
    }


    public virtual void UpdateRaycastOrigins()
    {
        Bounds bounds = thisCollider.bounds;
        //expanding the extends by SKINWIDTH will shrink it to the small amounts we need
        if (inside)
        {
            bounds.Expand(SKINWIDTH * -2);//we multiply by 2 because the extents are half of the size of the bounds
            raycastOrigins.bottomLeft = new Vector2(bounds.min.x  , bounds.min.y  );
            raycastOrigins.bottomRight = new Vector2(bounds.max.x  , bounds.min.y  );
            raycastOrigins.topLeft = new Vector2(bounds.min.x  , bounds.max.y  );
            raycastOrigins.topRight = new Vector2(bounds.max.x  , bounds.max.y  );
        }
        else
        {
            raycastOrigins.bottomLeft = new Vector2(bounds.min.x - SKINWIDTH, bounds.min.y - SKINWIDTH);
            raycastOrigins.bottomRight = new Vector2(bounds.max.x + SKINWIDTH, bounds.min.y - SKINWIDTH);
            raycastOrigins.topLeft = new Vector2(bounds.min.x - SKINWIDTH, bounds.max.y + SKINWIDTH);
            raycastOrigins.topRight = new Vector2(bounds.max.x + SKINWIDTH, bounds.max.y + SKINWIDTH);
        }
    }

    public void CalculateRaySpacing()
    {
        Bounds bounds = thisCollider.bounds;
        if (inside)
        {
            bounds.Expand(SKINWIDTH * -2);
        }
        //we want at least 4 horizontal and 2 vertical rays. 
        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 4, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 4, int.MaxValue);
        
        //now we calculate spacing between each ray
        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

}
