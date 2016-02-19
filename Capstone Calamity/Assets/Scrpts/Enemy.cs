using UnityEngine;
using System.Collections;
[RequireComponent (typeof(Controller2D))]
public class Enemy : MonoBehaviour {
	public int EnemyHealth=3, damage=1;
    public BoxCollider2D myCollider, playerCollider;
    public bool touchedLastFrame;
    protected Controller2D myController;

    protected Vector3 velocity;

    // Use this for initialization
    protected void Start () 
	{
        myCollider = gameObject.GetComponent<BoxCollider2D>();
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>();
        velocity = new Vector3();
        myController = gameObject.GetComponent<Controller2D>();
	}
	
	// Update is called once per frame
	void Update () 
	{
        
        gameObject.GetComponent<Controller2D>().Move(velocity * Time.smoothDeltaTime);

        int side = CollisionChecker.checkSide( playerCollider, myCollider);
        if (side != CollisionChecker.MISS)
        {
            if (!touchedLastFrame)
            {

                Controller2D playerController=playerCollider.gameObject.GetComponent<Controller2D>();
                playerController.hitPoints-=damage;
                touchedLastFrame = true;
            }
        }
        else
        {
            touchedLastFrame = false;
        }
         if (EnemyHealth <= 0) 
		{
			GameObject.Destroy (gameObject);
		}

        Move();
	}

    public virtual void Move()
    {

    }

}
