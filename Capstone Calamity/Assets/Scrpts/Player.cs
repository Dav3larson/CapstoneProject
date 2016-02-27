using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Controller2D))]//Doing this ensures the gameobject this is attached to will have a Controller2D
public class Player : MonoBehaviour {

	public Vector3 velocity;
    public Transform fireball;
    public float jumpHeight = 4;
    public float jumpTime = .4f;
    bool isJumping;
    public bool isClimbing;
    public float gravity;
	public float run=10;
    public float jumpSpeed;
	public Controller2D controller;
    public Checkpoint lastCheckPoint;
    public int maxHitPoint=3;
    public Vector2 input;
    float velocityXSmoothing;
    public Vector2 climbVelocity;
    public bool didDie = false;
    public Ladder currentLadder;

    // Use this for initialization
    void Start ()
    {
		controller = GetComponent<Controller2D> ();
        gravity = -(2 * jumpHeight) / Mathf.Pow(jumpTime, 2);//derived from kinematic equations. PHYSICS!
        jumpSpeed = Mathf.Abs(gravity) * jumpTime;
        
        //velocity.y = jumpSpeed;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(controller.hitPoints<=0)
        {
            die();
        }
        if(isClimbing)
        {
            transform.Translate(climbVelocity * Time.deltaTime);
            return;
        }
        if(Input.GetKeyDown(KeyCode.Joystick1Button2)||Input.GetKeyDown(KeyCode.LeftShift))
        {
            Fire();
        }
        //if we're jumping check if we want to start falling afterwords
        if(isJumping&&(Input.GetKeyUp(KeyCode.Space)
            ||Input.GetKeyUp(KeyCode.Joystick1Button0)) 
            &&velocity.y>0)//this will create the option to short jump. 
        {
            velocity.y =0;
        }
        //check if we hit the ceiling with our head or if we land on the floor
        if(controller.collisions.above||controller.collisions.below)
        {
            velocity.y = 0;
            isJumping = false;
        }
        //Check if we're gonna start jumping this frame
        if(controller.collisions.below
            &&(Input.GetKeyDown(KeyCode.Space)
            ||Input.GetKeyDown(KeyCode.Joystick1Button0)))
        {
            velocity.y = jumpSpeed;
            isJumping = true;
        }
		input = new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical"));
        float targetVelocityX = input.x * run;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, .1f);
        //velocity.x = targetVelocityX;
		velocity.y += gravity * Time.deltaTime;
        //Debug.Log(gravit);
		controller.Move (velocity * Time.deltaTime);
      
    }
    public void Fire()
    {
        //Debug.Log("FIRE!");
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
        //Debug.Log("We ded");
        //Start some dying animation
        //wait some time... and then
        if (lastCheckPoint)
        {
           // controller.hitPoints = maxHitPoint;
            lastCheckPoint.respawn();
           // Debug.Log("respawned");
        }
    }
}
