using UnityEngine;
using System.Collections;

public class PlayerControl1 : MonoBehaviour 
{
	public int HitPoints=3;
	public float jumpSpeed=10f;
	public float jumpDistance=2.0f;
	public float jumpTime=0.25f;
	public float peakLimit=.5f;
	public float jumpTarget=0f;
	public float staggerSpeed=.1f,timeSinceLastStagger=0f, realtime,staggerLimit=.15f;
	public static int STANDING=0,JUMPING=1,PEAKRISE=2,PEAKFALL=3,FALLING=4,LEFT=0,RIGHT=1;
	public static int BOUNCING=5, BOUNCEPEAK=6;
	public bool isStaggering=false;
	public int jumpState=FALLING;
	public int facing=RIGHT;

	void Start () 
	{
		HitPoints=3;
	}

	// Update is called once per frame
	void Update ()
	{
		Rigidbody body = gameObject.GetComponent<Rigidbody> ();
		float velocityY = body.velocity.y;
		float velocityX = body.velocity.x;
		realtime=Time.timeSinceLevelLoad;
		if(isStaggering)
		{
			if(facing==LEFT)
			{
				transform.Translate(staggerSpeed,-jumpSpeed,0);
			}
			else
			{
				transform.Translate(-staggerSpeed,-jumpSpeed,0);
			}
			if(Time.timeSinceLevelLoad-timeSinceLastStagger>=staggerLimit)
			{
				isStaggering=false;
				timeSinceLastStagger=Time.timeSinceLevelLoad; 
			}
			return;
		}
		//************Player Control Input Section************
		if(Input.GetKey (KeyCode.LeftArrow))
		{
			body= gameObject.GetComponent<Rigidbody>();
			facing=LEFT;
			body.velocity=new Vector3(-10,velocityY,0);
		}

		if(Input.GetKey (KeyCode.RightArrow))
		{
			//body.velocity = new Vector3(10, velocityY, 0);
			facing=RIGHT;
			body.AddForce(new Vector2(1,0),ForceMode.VelocityChange);
		}
		//When releasing a key you stop moving
		if((Input.GetKeyUp (KeyCode.RightArrow)||Input.GetKeyUp (KeyCode.LeftArrow)))
		{
			
			body.velocity=new Vector3(0,velocityY,0);
		}

		//updates horizontal velocity
		velocityX = body.velocity.x;

		//JUMPING STARTS HERE
		if((Input.GetKeyDown(KeyCode.Space))&&jumpState==STANDING)
		{
			//transform.Translate(0,Yspeed,0);

			jumpState=JUMPING;
			jumpTarget = Time.realtimeSinceStartup+jumpTime;	//transform.position.y+jumpDistance;//calculate our max height our jump will reach
			peakLimit= Time.realtimeSinceStartup+jumpTime/2;
		}

		if(Input.GetKey (KeyCode.DownArrow))
		{
			//transform.Translate(0,-Yspeed,0);
		}


		//************JUMPING State machine************
		if(jumpState==JUMPING)
		{
			if(Input.GetKey(KeyCode.Space))
			{
				body.velocity = new Vector3(velocityX,jumpSpeed,0);

				if(Time.realtimeSinceStartup>=peakLimit)//when we get closer to the peak, we will slow down.
				{
					jumpState = FALLING;//PEAKRISE;
				}
			}
			else
			{
				jumpState = FALLING;//PEAKFALL;
			}
		}


		if(jumpState==PEAKRISE)
		{
			if(Input.GetKey (KeyCode.Space))
			{
				//transform.Translate(0,jumpSpeed/2.0f,0);
				body.velocity = new Vector3(velocityX,jumpSpeed/2,0);
				if(Time.realtimeSinceStartup>=jumpTarget)
				{
					jumpState=PEAKFALL;
				}
			}
			else
			{
				jumpState=PEAKFALL;
			}
		}

		if(jumpState==PEAKFALL)
		{
			body.velocity = new Vector3(velocityX,-jumpSpeed/2,0);
			if(Time.realtimeSinceStartup>=jumpTime*.25f+jumpTarget)//when we get further from the peak, we will speed up.
			{
				jumpState=FALLING;

			}
		}
		if(jumpState==FALLING)
		{
			body.velocity = new Vector3(velocityX,-jumpSpeed,0);
			if (Time.realtimeSinceStartup > jumpTarget)
			{
				jumpState = STANDING;
			}
		}

		if(jumpState==STANDING)
		{

		}

		if(jumpState==BOUNCING)
		{
			body.velocity = new Vector3(velocityX,jumpSpeed,0);
			if(jumpTarget-transform.position.y<=peakLimit)//when we get closer to the peak, we will slow down.
			{
				jumpState=BOUNCEPEAK;
			}
		}
		if(jumpState==BOUNCEPEAK)
		{
			body.velocity = new Vector3(velocityX,jumpSpeed,0);
			if(transform.position.y>=jumpTarget)
			{
				jumpState=PEAKFALL;
			}
		}

	}

	//************_Utility/Mechanic_Functions_************

	public void takeDamage(int dmg)
	{
		if(isStaggering)
		{
			return;
		}
		HitPoints-=dmg;
		if(HitPoints<0)
		{
			HitPoints=0;
		}
	}


}


















