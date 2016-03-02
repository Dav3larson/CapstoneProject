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
        climbSpeed=20f;
    }
    public override void Update()
    {

        UpdateRaycastOrigins();
        //CalculateRaySpacing();
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
            if (player.climbVelocity.y>0&&playerCollider.bounds.min.y>topExit.y&&player.playerState==Player.PlayerState.Climbing)
            {
                //you know how we kinda hop out of the checkpoint fires? we'll do the same thing here
                player.playerState = Player.PlayerState.Jumping;
                player.velocity.y =player.jumpSpeed;
            }
            if(player.climbVelocity.y<0&&playerCollider.bounds.min.y<bottomExit.y+1)
            {
                player.playerState = Player.PlayerState.Falling;
                //we're falling off the bottom of the ladder, if the ladder's bottom is on the floor we'll start Standing in the next frame
            }
        }
    }
    public override void checkForBurns(LayerMask layer)
    {
        for (int i = 0; i < verticalRayCount; i++)
        {
            float rayLength =Mathf.Abs( raycastOrigins.topLeft.y - raycastOrigins.bottomLeft.y);

            
            Vector2 topRayOrigin = new Vector2(raycastOrigins.topLeft.x + (verticalRaySpacing * i), raycastOrigins.topLeft.y );
            RaycastHit2D playerHit = Physics2D.Raycast(topRayOrigin, Vector2.up*-1, rayLength, layer);
            Debug.DrawRay(topRayOrigin, Vector2.up * -1 * rayLength, Color.red);
            if (!onFire && playerHit&&player.input.y!=0)
            {
                player.playerState = Player.PlayerState.Climbing;
                player.controller.collisions.Reset();
               // Debug.Log("we know we're climbing...");
                if(player.input.y > 0)
                {
                    
                    //player.climbVelocity.y = climbSpeed;
                    Vector2 move = topExit - (Vector2)player.transform.position;
                    if (move.y < 0)
                    {
                        player.playerState = Player.PlayerState.Falling;
                    }
                    else
                    {
                        player.climbVelocity = move.normalized * climbSpeed;
                        Burn();
                    }
                   
                    ///Debug.Log("UP!");
                }     
                else
                {
                    Burn();
                    Vector2 move = bottomExit - (Vector2)player.transform.position;
                    player.climbVelocity = move.normalized * climbSpeed;
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
