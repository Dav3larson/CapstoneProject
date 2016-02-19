using UnityEngine;
using System.Collections;

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

    public const float SKINWIDTH = .015f;

    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;

    }


    public virtual void Start()
    {
        thisCollider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
        //collisions = new CollisionInfo();

    }


    public void UpdateRaycastOrigins()
    {
        Bounds bounds = thisCollider.bounds;
        //expanding the extends by SKINWIDTH will shrink it to the small amounts we need
        bounds.Expand(SKINWIDTH * -2);//we multiply by 2 because the extents are half of the size of the bounds
        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    void CalculateRaySpacing()
    {
        Bounds bounds = thisCollider.bounds;
        bounds.Expand(SKINWIDTH * -2);

        //we want at leasat 2 horizontal and 2 vertical rays. This will ensure that
        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);

        //no we calculate spacing between each ray
        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

}
