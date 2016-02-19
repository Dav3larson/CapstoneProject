using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	public int HitPoints,damage;
	public Collider2D thisCollider;
	public Collider2D heroCollider;
	public float timeSinceLastHit;
	public float hitTime=.25f;

	// Use this for initialization
	void Start () {
		HitPoints = 3;
		damage=1;
		thisCollider = GetComponent<Collider2D> ();
		heroCollider= GameObject.FindGameObjectWithTag ("Player").GetComponent<Collider2D> ();
		timeSinceLastHit=0f;
		
	}
	
	// Update is called once per frame
	void Update () 
	{
        int collisionResult = CollisionChecker.checkSide(heroCollider,thisCollider);
		PlayerControl pc=GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();

		if(collisionResult==CollisionChecker.UP)//if player jumps on top of us
		{
			if(Time.timeSinceLevelLoad-timeSinceLastHit>hitTime)
			{
				HitPoints--;//we lose 1HP
				timeSinceLastHit=Time.timeSinceLevelLoad;
				pc.jumpState=PlayerControl.STANDING;
				GameObject player= GameObject.FindGameObjectWithTag("Player");
				Collider playerCollider= player.GetComponent<Collider>();
				float travelDistance= thisCollider.bounds.max.y-playerCollider.bounds.min.y;
				player.transform.Translate (0,travelDistance,0);
				pc.jumpTarget=player.transform.position.y+pc.jumpDistance/2;
				pc.jumpState=PlayerControl.BOUNCING;
			}

		}

		else if(collisionResult!=CollisionChecker.MISS)//but so long as we DO make contact some other way...
		{
			if(Time.timeSinceLevelLoad-timeSinceLastHit>=hitTime)
			{
				if(!pc.isStaggering)
				{
					//Debug.Log("STAGGER");
					pc.takeDamage(damage);
					pc.isStaggering=true;
					pc.timeSinceLastStagger=Time.timeSinceLevelLoad;
					//deal damage to the player
					timeSinceLastHit=Time.timeSinceLevelLoad;
					GameObject player= GameObject.FindGameObjectWithTag("Player");
					Collider playerCollider= player.GetComponent<Collider>();
					if(collisionResult==CollisionChecker.LEFT||collisionResult== CollisionChecker.DOWN)
					{
						float travelDistance= playerCollider.bounds.max.x-thisCollider.bounds.min.x;
						player.transform.Translate (-travelDistance*1.5f,0,0);
					}
					if(collisionResult==CollisionChecker.RIGHT)
					{
						float travelDistance=thisCollider.bounds.max.x-playerCollider.bounds.min.x;
						player.transform.Translate(travelDistance,0,0);
					}
				}
			}
		}


		if (HitPoints <= 0) 
		{
			Destroy(gameObject);
			if(collisionResult!=CollisionChecker.MISS){
				pc.jumpState=PlayerControl.FALLING;
			}
		}
		move();
	}




	public virtual void move()
	{
		//standard won't do anything. Will be inherited to do actions
	}

	public virtual void attack()
	{
		//standard won't do anything. Will be inherited to do actions
	}

	public virtual void takeDamage(int dmg)
	{

	}
	

//Will NOT BE IN USE
 /*void OnCollisionEnter(Collision other)
	{
		
		if (other.collider.CompareTag("Player"))
		{
			//Debug.Log ("Contact has been made");
			if(thisCollider.bounds.max.y-other.collider.bounds.min.y<.5f)
			{
				//Debug.Log ("EnemyDeath");
				HitPoints--;
				//PlayerControl PC=other.gameObject.GetComponent<PlayerControl>();
				//jumpState=JUMPING;
				//PC.jumpTarget=thisCollider.bounds.max.y+PC.jumpDistance;
				//PC.jumpState=PlayerControl.JUMPING;
			}
			else
			{
				//Debug.Log("PlayerDeath");
				PlayerControl PC=other.gameObject.GetComponent<PlayerControl>();//access the player's script to take down HP
				if(PC!=null)
				{
					PC.takeDamage(1);
				}
			}
		}
	}
*/
}
