using UnityEngine;
using System.Collections;

public class Sign : RaycastController
{
    public TextboxManager tbm;
    // Use this for initialization
    public override void Start()
    {
        inside = false;
        base.Start();
        collisionMask = LayerMask.GetMask("Player");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRaycastOrigins();
        float rayLength = 1;// Mathf.Abs(SKINWIDTH * 1.5f);

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rightRayOrigin = new Vector2(raycastOrigins.bottomRight.x, raycastOrigins.bottomRight.y + (horizontalRaySpacing * i));
            Vector2 leftRayOrigin = new Vector2(raycastOrigins.bottomLeft.x, raycastOrigins.bottomRight.y + (horizontalRaySpacing * i));


            RaycastHit2D hitRight = Physics2D.Raycast(rightRayOrigin, Vector2.right, rayLength, collisionMask);
            RaycastHit2D hitLeft = Physics2D.Raycast(leftRayOrigin, Vector2.right * -1, rayLength, collisionMask);

            if (hitRight || hitLeft)
            {
                Read();
            }

            Debug.DrawRay(rightRayOrigin, Vector2.right * rayLength, Color.red);
            Debug.DrawRay(leftRayOrigin, Vector2.right * -1 * rayLength, Color.red);
        }
        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 topRayOrigin = new Vector2(raycastOrigins.topLeft.x + (verticalRaySpacing * i), raycastOrigins.topLeft.y);
            Vector2 bottomRayOrigin = new Vector2(raycastOrigins.bottomLeft.x + (verticalRaySpacing * i), raycastOrigins.bottomLeft.y);

            RaycastHit2D hitUp = Physics2D.Raycast(topRayOrigin, Vector2.up, rayLength, collisionMask);
            RaycastHit2D hitBelow = Physics2D.Raycast(bottomRayOrigin, Vector2.up * -1, rayLength, collisionMask);

            Debug.DrawRay(topRayOrigin, Vector2.up * 1 * rayLength, Color.red);
            Debug.DrawRay(bottomRayOrigin, Vector2.up * -1 * rayLength, Color.red);

            if (hitUp || hitBelow)
            {
                Read();
            }
        }
    }
    public void Read()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (input.y>0)
        {
            Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            player.playerState = Player.PlayerState.Reading;
            
                if (tbm)
                {
                    //Debug.Log("Found the manager!");
                    tbm.Enable();
                }
        }
    }
}
