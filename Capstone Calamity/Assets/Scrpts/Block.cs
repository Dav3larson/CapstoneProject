using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(Controller2D))]//Doing this ensures the gameobject this is attached to will have a Controller2D

public class Block : MonoBehaviour {

    Controller2D controller;
    float gravity = -5f;
	void Start ()
    {
        controller = GetComponent<Controller2D>();
        gravity = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().gravity;
        Bounds box = GetComponent<BoxCollider2D>().bounds;
        controller.collisionMask = LayerMask.GetMask("Obstacle");
        controller.inside = false;
    }
	
	// Update is called once per frame
	void Update ()
    {

        controller.Move(new Vector3(0, gravity) * Time.deltaTime);
        if(controller.collisions.below)
        {
            float rayLength = .15f;
            for (int i = 0; i < controller.verticalRayCount; i++)
            {
               
                Vector2 bottomRayOrigin = new Vector2(controller.raycastOrigins.bottomLeft.x + (controller.verticalRaySpacing * i), controller.raycastOrigins.bottomLeft.y);
                RaycastHit2D hitBelow = Physics2D.Raycast(bottomRayOrigin, Vector2.up * -1, rayLength, LayerMask.GetMask("Player"));

                
                Debug.DrawRay(bottomRayOrigin, Vector2.up * -1 * rayLength, Color.red);

                if (hitBelow)
                {
                    Controller2D playerController = hitBelow.transform.GetComponent<Controller2D>();
                    if (playerController)
                    {
                        playerController.takeDamage(playerController.hitPoints);
                    }
                }
            }
        }
	}
}
