using UnityEngine;
using System.Collections;

public class Combustable : RaycastController {

    public LayerMask fireMask;
    public LayerMask playerMask;
    public LayerMask obstacle;
    public float burnTime=.5f;

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
            Vector2 rayOrigin = raycastOrigins.bottomLeft + Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D playerHit = Physics2D.Raycast(rayOrigin, Vector2.right * -1, rayLength, playerMask);
            RaycastHit2D fireHit = Physics2D.Raycast(rayOrigin, Vector2.right * -1, rayLength, fireMask);
            Debug.DrawRay(rayOrigin, Vector2.right * -1 * rayLength, Color.red);
            if (!onFire && (playerHit || fireHit))
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
        for (int i = 0; i < horizontalRayCount; i++)
        {
            float rayLength =  SKINWIDTH*1.5f;
            
            Vector2 rayOrigin = raycastOrigins.bottomRight + Vector2.up * (horizontalRaySpacing * i);
            rayOrigin.x += 2*SKINWIDTH;
            RaycastHit2D hitRight = Physics2D.Raycast(rayOrigin, Vector2.right * 1, rayLength, obstacle);
            Debug.DrawRay(rayOrigin, Vector2.right * 1 * rayLength, Color.black);
            if (hitRight)
            {
                Combustable nextVictim = hitRight.collider.gameObject.GetComponent<Combustable>();
               
                if (nextVictim==null)
                {
                    //nextVictim.Burn();
                    Debug.Log("null");
                }
                else
                {

                    Debug.Log("Not null at all...");
                    //GameObject.Destroy(nextVictim.gameObject);
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
