using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Controller2D))]//Doing this ensures the gameobject this is attached to will have a Controller2D
public class Block : MonoBehaviour {

    Controller2D controller;
    float gravity = -55f;
    public Vector2 velocity;
	void Start ()
    {
        controller = GetComponent<Controller2D>();
        controller.Start();

        Bounds bounds = GetComponent<BoxCollider2D>().bounds;
        controller.collisionMask = LayerMask.GetMask("Obstacle");
        controller.inside = false;
        velocity = Vector2.zero;
        gravity=GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().gravity-5;
    }

    // Update is called once per frame
    void Update()
    {
        //if you last frame we didn't fall down we shoot down some rays
        if (!controller.collisions.below)
        {
            velocity.y += gravity*Time.deltaTime;
            float rayLength = Mathf.Abs(velocity.y * Time.deltaTime) ;
            for (int i = 0; i < controller.verticalRayCount; i++)
            {
                controller.UpdateRaycastOrigins();
                Vector2 bottomRayOrigin = new Vector2(controller.raycastOrigins.bottomLeft.x + (controller.verticalRaySpacing * i), controller.raycastOrigins.bottomLeft.y);
                RaycastHit2D hitBelow = Physics2D.Raycast(bottomRayOrigin, Vector2.up*-1 , rayLength, LayerMask.GetMask("Player"));
                //this will not show if the Controller2D.Vertical coliision's DrawRay is called.
                Debug.DrawRay(bottomRayOrigin, Vector2.up * -1* rayLength, Color.cyan);

                if (hitBelow)
                {
                    Controller2D playerController = hitBelow.transform.GetComponent<Controller2D>();
                    if (playerController)
                    {
                        playerController.hitPoints = 0;
                    }
                }
            }
    }
        else
        {
            velocity.y = 0;
        }
            controller.Move(velocity * Time.deltaTime);
    }
}
