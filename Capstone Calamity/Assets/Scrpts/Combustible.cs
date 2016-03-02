using UnityEngine;
using System.Collections;

public class Combustible : RaycastController {

    public LayerMask fireMask;
    public LayerMask playerMask;
    public LayerMask obstacle;
    public float burnTime = 1.5f;

    public bool onFire, burntOut;
    public float timeSet;
    // Use this for initialization
    public override void Start()
    {
        base.Start();

        playerMask = LayerMask.GetMask("Player");
        fireMask = LayerMask.GetMask("Fire");
        obstacle = LayerMask.GetMask("Obstacle");
        MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
        mr.material.color = Color.green;
        inside = false;
        Bounds bounds = GetComponent<BoxCollider2D>().bounds;
       
        
    }

    // Update is called once per frame
    public virtual void Update ()
    {
        
        UpdateRaycastOrigins();

        CalculateRaySpacing();
        if (!burntOut && !onFire)
        {
            checkForBurns(playerMask);
            checkForBurns(fireMask);
        }
        if(onFire)
        {
            Spread(obstacle);
            if(Time.realtimeSinceStartup > timeSet + burnTime)
           {
                Extinguish();
           }
        }
	}
    public virtual void checkForBurns(LayerMask layer)
    {
        float rayLength = Mathf.Abs(SKINWIDTH * 1.5f);

        for (int i = 0; i < horizontalRayCount; i++)
        {        
            Vector2 rightRayOrigin = new Vector2(raycastOrigins.bottomRight.x , raycastOrigins.bottomRight.y + (horizontalRaySpacing * i));
            Vector2 leftRayOrigin = new Vector2(raycastOrigins.bottomLeft.x , raycastOrigins.bottomRight.y + (horizontalRaySpacing * i));
            
            
            RaycastHit2D hitRight = Physics2D.Raycast(rightRayOrigin, Vector2.right, rayLength, layer);
            RaycastHit2D hitLeft = Physics2D.Raycast(leftRayOrigin, Vector2.right * -1, rayLength, layer);
           


            Debug.DrawRay(rightRayOrigin, Vector2.right  * rayLength, Color.red);
            Debug.DrawRay(leftRayOrigin, Vector2.right*-1* rayLength, Color.red);
            

            if (!onFire && (hitRight || hitLeft))
            {
                Burn();

            }

        }
        for(int i=0; i < verticalRayCount; i++)
        {
            Vector2 topRayOrigin = new Vector2(raycastOrigins.topLeft.x + (verticalRaySpacing * i), raycastOrigins.topLeft.y);
            Vector2 bottomRayOrigin = new Vector2(raycastOrigins.bottomLeft.x + (verticalRaySpacing * i), raycastOrigins.bottomLeft.y);

            RaycastHit2D hitUp = Physics2D.Raycast(topRayOrigin, Vector2.up, rayLength, playerMask);
            RaycastHit2D hitBelow = Physics2D.Raycast(bottomRayOrigin, Vector2.up * -1, rayLength, layer);

            Debug.DrawRay(topRayOrigin, Vector2.up * 1 * rayLength, Color.red);
            Debug.DrawRay(bottomRayOrigin, Vector2.up * -1 * rayLength, Color.red);

            if(!onFire && ( hitUp || hitBelow))
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
    //This method attempts to spread the flame to other objects on the Combustible layer. 
    public virtual void Spread(LayerMask layer)
    {
        float rayLength =  .25f;
        //We first give the combustible a bit of time before spreading
        if (timeSet + burnTime/2  > Time.realtimeSinceStartup)
        {
            return;
        }
        //Check for combustibles to the left or right
        for (int i = 0; i < horizontalRayCount; i++)
        {   
            Vector2 rightRayOrigin = new Vector2(raycastOrigins.bottomRight.x,raycastOrigins.bottomRight.y +  (horizontalRaySpacing * i));
            Vector2 leftRayOrigin = new Vector2(raycastOrigins.bottomLeft.x , raycastOrigins.bottomRight.y + (horizontalRaySpacing * i));
            
            RaycastHit2D hitRight = Physics2D.Raycast(rightRayOrigin, Vector2.right , rayLength, layer);
            RaycastHit2D hitLeft= Physics2D.Raycast(leftRayOrigin, Vector2.right * -1, rayLength, layer);
            


            Debug.DrawRay(rightRayOrigin, Vector2.right * rayLength, Color.black);
            Debug.DrawRay(leftRayOrigin, Vector2.right * -1 * rayLength, Color.black);


            if (hitRight)
            {
                Combustible nextVictim = hitRight.collider.gameObject.GetComponent<Combustible>();
                if (nextVictim!=null)
                {    
                    nextVictim.Burn();
                }
            }
            if (hitLeft)
            {
                Combustible nextVictim = hitLeft.collider.gameObject.GetComponent<Combustible>();
                if (nextVictim != null)
                {
                    nextVictim.Burn();
                }
            }
        }

        //Now we check for combustibles above and below us!
        for(int i=0;i<verticalRayCount;i++)
        {

        
            Vector2 topRayOrigin = new Vector2(raycastOrigins.topLeft.x + (verticalRaySpacing * i), raycastOrigins.topLeft.y);
            Vector2 bottomRayOrigin = new Vector2(raycastOrigins.bottomLeft.x + (verticalRaySpacing * i), raycastOrigins.bottomLeft.y);

            RaycastHit2D hitUp = Physics2D.Raycast(topRayOrigin, Vector2.up, rayLength, layer);
            RaycastHit2D hitBelow = Physics2D.Raycast(bottomRayOrigin, Vector2.up * -1, rayLength, layer);
            Debug.DrawRay(topRayOrigin, Vector2.up * 1 * rayLength, Color.black);
            Debug.DrawRay(bottomRayOrigin, Vector2.up * -1 * rayLength, Color.black);

            if (hitUp)
            {
                Combustible nextVictim = hitUp.collider.gameObject.GetComponent<Combustible>();
                if (nextVictim != null)
                {
                    nextVictim.Burn();
                }
            }
            if (hitBelow)
            {
                Combustible nextVictim = hitBelow.collider.gameObject.GetComponent<Combustible>();
                if (nextVictim != null)
                {
                    nextVictim.Burn();
                }
            }
        }
    }
    public virtual void Extinguish()
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
