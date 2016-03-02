using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Controller2D))]//Doing this ensures the gameobject this is attached to will have a Controller2D
public class Player : MonoBehaviour {

	public Vector3 velocity;
    public Vector2 climbVelocity;
    public Vector2 staggerVelocity;
    public Transform fireball;
    public float jumpHeight = 4;
    public float jumpTime = .4f;
    public float gravity;
	public float run=10;
    public float jumpSpeed;
	public Controller2D controller;
    public Checkpoint lastCheckPoint;
    public int maxHitPoint=3;
    public Vector2 input;
    float velocityXSmoothing;
    public PlayerState playerState;
    float staggerTime = .5f;
    float timeSinceHit = 0;
    public enum PlayerState
    {
        Dead,
        Staggering,
        Climbing,
        Reading,
        Jumping,
        Falling,
        Standing,
    }
    // Use this for initialization
    void Start ()
    {
		controller = GetComponent<Controller2D> ();
        gravity = -(2 * jumpHeight) / Mathf.Pow(jumpTime, 2);//derived from kinematic equations. PHYSICS!
        jumpSpeed = Mathf.Abs(gravity) * jumpTime;
        playerState = PlayerState.Jumping;
        //velocity.y = jumpSpeed;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(controller.hitPoints<=0)
        {
            die();
        }
        if (playerState==PlayerState.Dead|| playerState == PlayerState.Reading)
        {
            velocity = Vector2.zero;
            return;
        }
        if(playerState==PlayerState.Climbing)
        {
            transform.Translate(climbVelocity * Time.deltaTime);
            return;
        }

        if(playerState== PlayerState.Staggering)
        {
            transform.Translate(staggerVelocity * Time.deltaTime);
            if (Time.realtimeSinceStartup > timeSinceHit + staggerTime)
            {
                playerState = PlayerState.Standing;
            }
            return;
        }
        if(Input.GetKeyDown(KeyCode.Joystick1Button2)||Input.GetKeyDown(KeyCode.LeftShift))
        {
            Fire();
        }

        //if we're jumping check if we want to start falling afterwords
        if(playerState==PlayerState.Jumping&&(Input.GetKeyUp(KeyCode.Space)
            ||Input.GetKeyUp(KeyCode.Joystick1Button0)) 
            &&velocity.y>0)//this will create the option to short jump. 
        {
            velocity.y =0;
            playerState = PlayerState.Falling;
        }

        //check if we're jumping and we hit our head on ceiling...
        if ((controller.collisions.above|| controller.collisions.below) )
        {
            velocity.y = 0;
            if (controller.collisions.above&&playerState==PlayerState.Jumping)
            {
                playerState = PlayerState.Falling;//we hit our head and gonna fall!
            }
            else if(controller.collisions.below)
            {
                playerState = PlayerState.Standing;
            }

        }
       
        //Check if we're gonna start jumping this frame
        if(controller.collisions.below
            &&(Input.GetKeyDown(KeyCode.Space)
            ||Input.GetKeyDown(KeyCode.Joystick1Button0)))
        {
            velocity.y = jumpSpeed;
            playerState = PlayerState.Jumping;
        }
		input = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
        float targetVelocityX = input.x * run;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, .1f);
        //velocity.x = targetVelocityX;
		velocity.y += gravity * Time.deltaTime;
        
		controller.Move (velocity * Time.deltaTime);
        if (velocity.y < 0 && !controller.collisions.below)
        {
            playerState = PlayerState.Falling;//if we're going down and there is nothing to stand on we're falling!
        }
    }
    public void Fire()
    {
        
        Object fire=Instantiate(fireball,transform.position,new Quaternion());

        //fire.SetParent(transform);
    }

    public void heal(int hp)
    {
        controller.hitPoints += hp;
        if(controller.hitPoints>maxHitPoint)
        {
            controller.hitPoints = maxHitPoint;
        }
    }
    void die()
    {
        //Start some dying animation around here

        //BOOM! this makes it so we're invisible and do not update
        
        playerState = Player.PlayerState.Dead;
        gameObject.SetActive(false);
        if (lastCheckPoint)
        {
           // controller.hitPoints = maxHitPoint;
            lastCheckPoint.SetRespawnTime();
           
        }
    }
    public void Respawn()
    {
        gameObject.SetActive(true);
        heal(maxHitPoint);
        transform.position = lastCheckPoint.transform.position;
        playerState = Player.PlayerState.Jumping;
        //we do this to make the player jump up when they do respawn from the checkpoint
        velocity = new Vector3(0, jumpSpeed / 1.5f, 0);
        controller.collisions.Reset();
    }
}
