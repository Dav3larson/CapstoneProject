using UnityEngine;
using System.Collections;

public class Combustable : RaycastController {

    public LayerMask fireMask;
    public LayerMask playerMask;
    public LayerMask obstacle;
    public float burnTime=2.5f;

    public bool onFire, burntOut;
    float timeSet;
    // Use this for initialization
    public override void Start()
    {
        base.Start();

        playerMask = LayerMask.GetMask("Player");
        fireMask = LayerMask.GetMask("Fire");
        obstacle = LayerMask.GetMask("Obstacle");
        MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
        mr.material.color = Color.green;

    }

    // Update is called once per frame
    public virtual void Update ()
    {
        
        UpdateRaycastOrigins();
        if (!burntOut && !onFire)
        {
        checkForBurns();
        }
        if(onFire)
        {
            Spread();
            Extinguish();
        }
	}
    public virtual void checkForBurns()
    {
        for (int i = 0; i < horizontalRayCount; i++)
        {
            float rayLength = SKINWIDTH * 1.5f;

            Vector2 rightRayOrigin = new Vector2(raycastOrigins.bottomRight.x + (2 * SKINWIDTH), raycastOrigins.bottomRight.y + (horizontalRaySpacing * i));
            Vector2 leftRayOrigin = new Vector2(raycastOrigins.bottomLeft.x - (2 * SKINWIDTH), raycastOrigins.bottomRight.y + (horizontalRaySpacing * i));
            Vector2 topRayOrigin = new Vector2(raycastOrigins.topLeft.x + (verticalRaySpacing * i), raycastOrigins.topLeft.y + (2 * SKINWIDTH));
            Vector2 bottomRayOrigin = new Vector2(raycastOrigins.bottomLeft.x + (verticalRaySpacing * i), raycastOrigins.bottomLeft.y - (2 * SKINWIDTH));


            RaycastHit2D hitRight = Physics2D.Raycast(rightRayOrigin, Vector2.right, rayLength, playerMask);
            RaycastHit2D hitLeft = Physics2D.Raycast(leftRayOrigin, Vector2.right * -1, rayLength, playerMask);
            RaycastHit2D hitUp = Physics2D.Raycast(topRayOrigin, Vector2.up, rayLength, playerMask);
            RaycastHit2D hitBelow = Physics2D.Raycast(bottomRayOrigin, Vector2.up * -1, rayLength, playerMask);


            Debug.DrawRay(rightRayOrigin, Vector2.right * 1 * rayLength, Color.red);
            Debug.DrawRay(leftRayOrigin, Vector2.right * 1 * rayLength, Color.red);
            Debug.DrawRay(topRayOrigin, Vector2.right * 1 * rayLength, Color.red);
            Debug.DrawRay(bottomRayOrigin, Vector2.right * 1 * rayLength, Color.red);

            if (!onFire && (hitRight || hitLeft || hitUp || hitBelow))
            {
                Burn();

            }

        }
    }
    public virtual void Burn()
    {
        if(burntOut||onFire)
        {
            //Debug.Log("No use");
            return;
        }
        //Debug.Log("We're good to burn!");
        onFire = true;
        timeSet = Time.realtimeSinceStartup;
        MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
        mr.material.color = Color.red;
    }
    public virtual void Spread()
    {
        
        if (timeSet + burnTime/2  < Time.realtimeSinceStartup)
        {
            return;
        }
        for (int i = 0; i < horizontalRayCount; i++)
        {
            float rayLength =  SKINWIDTH*1.5f;
            
            Vector2 rightRayOrigin = new Vector2(raycastOrigins.bottomRight.x+(2*SKINWIDTH),raycastOrigins.bottomRight.y +  (horizontalRaySpacing * i));
            Vector2 leftRayOrigin = new Vector2(raycastOrigins.bottomLeft.x - (2 * SKINWIDTH), raycastOrigins.bottomRight.y + (horizontalRaySpacing * i));
            Vector2 topRayOrigin = new Vector2(raycastOrigins.topLeft.x + (verticalRaySpacing * i), raycastOrigins.topLeft.y + (2 * SKINWIDTH));
            Vector2 bottomRayOrigin = new Vector2(raycastOrigins.bottomLeft.x + (verticalRaySpacing * i), raycastOrigins.bottomLeft.y - (2 * SKINWIDTH));
            

            RaycastHit2D hitRight = Physics2D.Raycast(rightRayOrigin, Vector2.right , rayLength, obstacle);
            RaycastHit2D hitLeft= Physics2D.Raycast(leftRayOrigin, Vector2.right * -1, rayLength, obstacle);
            RaycastHit2D hitUp = Physics2D.Raycast(topRayOrigin, Vector2.up , rayLength, obstacle);
            RaycastHit2D hitBelow = Physics2D.Raycast(bottomRayOrigin, Vector2.up * -1, rayLength, obstacle);


            Debug.DrawRay(rightRayOrigin, Vector2.right * 1 * rayLength, Color.black);
            Debug.DrawRay(leftRayOrigin, Vector2.right * 1 * rayLength, Color.black);
            Debug.DrawRay(topRayOrigin, Vector2.right * 1 * rayLength, Color.black);
            Debug.DrawRay(bottomRayOrigin, Vector2.right * 1 * rayLength, Color.black);

            if (hitRight)
            {
                Combustable nextVictim = hitRight.collider.gameObject.GetComponent<Combustable>();
                if (nextVictim!=null)
                {    
                    nextVictim.Burn();
                }
            }
            if (hitLeft)
            {
                Combustable nextVictim = hitLeft.collider.gameObject.GetComponent<Combustable>();
                if (nextVictim != null)
                {
                    nextVictim.Burn();
                }
            }
            if (hitUp)
            {
                Combustable nextVictim = hitUp.collider.gameObject.GetComponent<Combustable>();
                if (nextVictim != null)
                {
                    nextVictim.Burn();
                }
            }
            if (hitBelow)
            {
                Combustable nextVictim = hitBelow.collider.gameObject.GetComponent<Combustable>();
                if (nextVictim != null)
                {
                    nextVictim.Burn();
                }
            }
        }
    }
    public virtual void Extinguish()
    {
        
            if (Time.realtimeSinceStartup > timeSet + burnTime)
           {
            gameObject.layer = LayerMask.GetMask("none");

            //just to change the color
            MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
                mr.material.color = Color.gray;
                //GameObject.Destroy(gameObject);
                onFire = false;
                burntOut = true;
            }
        
    }
}
