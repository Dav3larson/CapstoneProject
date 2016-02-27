using UnityEngine;
using System.Collections;

public class Ladder : Combustible
{
    Player player;// = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    Vector2 topExit, bottomExit;
    float climbSpeed;
    public override void Start()
    {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        climbSpeed=10f;
    }
    public override void Update()
    {

        UpdateRaycastOrigins();
        topExit = new Vector2((raycastOrigins.topLeft.x + raycastOrigins.topRight.x)/2, (raycastOrigins.topLeft.y + raycastOrigins.topRight.y) / 2);
        bottomExit = new Vector2((raycastOrigins.bottomLeft.x + raycastOrigins.bottomRight.x) / 2, (raycastOrigins.bottomLeft.y + raycastOrigins.bottomRight.y) / 2);
        if (!burntOut && !onFire)
        {
            checkForBurns(playerMask);
        }
        if (onFire)
        {
            Spread(obstacle);
            if (Time.realtimeSinceStartup > timeSet + burnTime)
            {
                Extinguish();
            }
                
            BoxCollider2D playerCollider = player.GetComponent<BoxCollider2D>();
            //it's our ascent 
            if (player.climbVelocity.y>0&&playerCollider.bounds.min.y>topExit.y&&player.isClimbing)
            {

                player.isClimbing = false;
                player.velocity.y =player.jumpSpeed;
            }
            if(player.climbVelocity.y<0&&playerCollider.bounds.min.y<bottomExit.y+1)
            {
                player.isClimbing = false;
            }
        }
    }
    public override void checkForBurns(LayerMask layer)
    {
        for (int i = 0; i < horizontalRayCount; i++)
        {
            float rayLength =Mathf.Abs( raycastOrigins.topLeft.y - raycastOrigins.bottomLeft.y);

            
            Vector2 topRayOrigin = new Vector2(raycastOrigins.topLeft.x + (verticalRaySpacing * i), raycastOrigins.topLeft.y );
            RaycastHit2D playerHit = Physics2D.Raycast(topRayOrigin, Vector2.up*-1, rayLength, layer);
            Debug.DrawRay(topRayOrigin, Vector2.up * -1 * rayLength, Color.red);
            if (!onFire && playerHit&&player.input.y!=0&&!player.isClimbing)
            {
                player.isClimbing = true;
                player.currentLadder = this;
                player.controller.collisions.Reset();
               // Debug.Log("we know we're climbing...");
                if(player.input.y > 0)
                {
                    Burn();
                    //player.climbVelocity.y = climbSpeed;
                    Vector2 move = topExit - (Vector2)player.transform.position;
                    player.climbVelocity = move.normalized * climbSpeed;
                    ///Debug.Log("UP!");
                }     
                else
                {
                    Burn();
                    player.climbVelocity.y = -1 * climbSpeed;// Vector2.Distance(bottomExit,player.transform.position) * Vector3.up / climbTime;
                   //Debug.Log("Distance: "+Vector2.Distance(bottomExit,player.transform.position) * Vector3.up / climbTime);
                }           
            }

        }
        //base.checkForBurns(layer);
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(topExit,bottomExit);
        //Gizmos.DrawLine(globalPoints - Vector3.left * size, globalPoints + Vector3.left * size);
    }
}
