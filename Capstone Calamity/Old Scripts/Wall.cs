using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {

    // Wall1 relies on onCollisionEnter and Wall relies on CollisionChecker 
    public GameObject player;
	public int  frameCount;
	// Use this for initialization
	void Start () 
	{
	
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

    //************_Collision_Detection_************
   void OnCollisionEnter(Collision other)
    {
        PlayerControl1 pc=other.gameObject.GetComponent<PlayerControl1>();

        Debug.Log("Entered");
        if(pc!= null)
        {
            
            foreach (ContactPoint contact in other.contacts)
            {
                //Debug.Log(contact.point);
            }
			Collider thisCollider = gameObject.GetComponent<Collider> ();
			//Collider otherCollider = other.collider;
			if (thisCollider != null) 
			{
				int side = CollisionChecker.checkSide (other.collider, thisCollider);
				if(side==CollisionChecker.LEFT)
				{
					Debug.Log ("From the Left!");
					//float difference=otherCollider.bounds.max.x - thisCollider.bounds.min.x;
					//pc.transform.Translate (-difference, 0, 0);
				}
				if(side==CollisionChecker.RIGHT)
				{
					Debug.Log ("From the right!");
				}
				if(side==CollisionChecker.UP)
				{
					pc.jumpState = PlayerControl1.STANDING;
					Rigidbody rb = pc.GetComponent<Rigidbody> ();
					rb.velocity = new Vector2 (rb.velocity.x, 0);

					Debug.Log ("landed on top!");
				}
				if(side==CollisionChecker.DOWN)
				{
					Debug.Log ("From below!");
				}
			}
     
        }
    }
    void OnCollisionExit(Collision other)
    {

    }

	void OnCollisionStay(Collision other)
	{
		Collider catcher = gameObject.GetComponent<Collider> ();
		Collider pitcher=other.collider;
		int side = CollisionChecker.checkSide (pitcher, catcher);
		if(side==CollisionChecker.LEFT)
		{
			//Debug.Log ("From the Left!");
			//float difference=pitcher.bounds.max.x - catcher.bounds.min.x;
			//pitcher.transform.Translate (-difference, 0, 0);
			//Debug.Log ("Distance is " + difference);
		}
		if(side==CollisionChecker.RIGHT)
		{
			Debug.Log ("From the right!");
		}
		if(side==CollisionChecker.UP)
		{
			PlayerControl1 pc = pitcher.gameObject.GetComponent<PlayerControl1> ();
			if (pc != null)
			{
				pc.jumpState = PlayerControl1.STANDING;
			}
			Debug.Log ("Standing on top!");
		}
		if(side==CollisionChecker.DOWN)
		{
			Debug.Log ("From below!");
		}
	}
		
}
