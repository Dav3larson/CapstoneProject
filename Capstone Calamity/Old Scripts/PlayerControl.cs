using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour 
{
	public int HitPoints=3;
	public float Xspeed=.15f,jumpSpeed=.15f;
	public float jumpDistance=2.0f;
	public float jumpTime;
	public float peakLimit=.5f;
	public float jumpTarget=0;
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
			transform.Translate(-Xspeed,0,0);
			facing=LEFT;
		}

		if(Input.GetKey (KeyCode.RightArrow))
		{
			//transform.Translate(Xspeed,0,0);

			facing=RIGHT;
		}

		if((Input.GetKeyDown(KeyCode.Space))&&jumpState==PlayerControl.STANDING)
		{
			//transform.Translate(0,Yspeed,0);
			jumpState=JUMPING;
			jumpTarget=transform.position.y+jumpDistance;//calculate our max height our jump will reach

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
				transform.Translate(0,jumpSpeed,0);
				if(jumpTarget-transform.position.y<=peakLimit)//when we get closer to the peak, we will slow down.
				{
					jumpState=PEAKRISE;
				}
			}
			else
			{
				jumpState=PEAKFALL;
			}
		}


		if(jumpState==PEAKRISE)
		{
			if(Input.GetKey (KeyCode.Space))
			{
				transform.Translate(0,jumpSpeed/2.0f,0);
				if(transform.position.y>=jumpTarget)
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
			transform.Translate(0,jumpSpeed/-2.0f,0);
			if(jumpTarget-transform.position.y>=peakLimit)//when we get further from the peak, we will speed up.
			{
				jumpState=FALLING;
				
			}
		}
		if(jumpState==FALLING)
		{
			transform.Translate(0,jumpSpeed*-1.0f,0);
		}

		if(jumpState==STANDING)
		{

		}

		if(jumpState==BOUNCING)
		{
			transform.Translate(0,jumpSpeed,0);
			if(jumpTarget-transform.position.y<=peakLimit)//when we get closer to the peak, we will slow down.
			{
				jumpState=BOUNCEPEAK;
			}
		}
		if(jumpState==BOUNCEPEAK)
		{
			transform.Translate(0,jumpSpeed/2.0f,0);
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
 

















