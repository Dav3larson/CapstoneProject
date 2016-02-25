using UnityEngine;
using System.Collections;

public class GreenKoopa : Enemy {

    // Use this for initialization
    void Start()
    {

        base.Start();
        velocity = new Vector3(0f, -5, 0);
    }
    public override void Move()
    {
        
        if(myController.collisions.left|| myController.collisions.right)
        {
            velocity = new Vector3(velocity.x * -1, velocity.y, 0);
        }
        base.Move();
    }

}
