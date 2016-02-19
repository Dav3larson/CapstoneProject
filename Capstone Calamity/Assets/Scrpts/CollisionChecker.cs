using UnityEngine;
using System.Collections;

public class CollisionChecker : MonoBehaviour {

	//Static class used to check between two colliders

	//Used as input answer from the check method. used in reference to where the
	//"Pitcher" is coming from, MISS means no collision
	public static int MISS=0,LEFT=1,RIGHT=2,UP=3,DOWN=4;



	public static int checkSide(Collider pitcher, Collider catcher)
	{
		//First we check if there IS a collision at all
		if(pitcher.bounds.max.x>=catcher.bounds.min.x&&
		   pitcher.bounds.min.y<=catcher.bounds.max.y&&
		   pitcher.bounds.max.y>=catcher.bounds.min.y&&
		   pitcher.bounds.min.x<=catcher.bounds.max.x)
		{
			//Debug.Log("Contact");
			//check distance towards borders
			float fromleft=Mathf.Abs(pitcher.bounds.max.x-catcher.bounds.min.x);
			float fromright=Mathf.Abs(pitcher.bounds.min.x-catcher.bounds.max.x);
			float fromup= Mathf.Abs(pitcher.bounds.min.y-catcher.bounds.max.y);
			float fromdown=Mathf.Abs(pitcher.bounds.max.y-catcher.bounds.min.y);
			//and save it all in this array
			float[] distances= new float[4];
			distances[0]=fromleft;
			distances[1]=fromright;
			distances[2]=fromup;
			distances[3]=fromdown;

			//after comparint all distanes we take the shortest distance and there's the edge you collided on!
			float shortest=Mathf.Min(distances);
			if(shortest==fromleft)
			{
				return LEFT;
			}
			if(shortest==fromright)
			{
				return RIGHT;
			}
			if(shortest==fromup)
			{
				return UP;
			}
			if(shortest==fromdown)
			{
				return DOWN;
			}
			//Vector3 distance= player.transform.position-transform.position;
			//player.transform.Translate(distance);
		}

	
			return MISS;

			
	}


    public static int checkSide(Collider2D pitcher, Collider2D catcher)
    {
        //First we check if there IS a collision at all
        if (pitcher.bounds.max.x >= catcher.bounds.min.x &&
           pitcher.bounds.min.y <= catcher.bounds.max.y &&
           pitcher.bounds.max.y >= catcher.bounds.min.y &&
           pitcher.bounds.min.x <= catcher.bounds.max.x)
        {
            //Debug.Log("Contact");
            //check distance towards borders
            float fromleft = Mathf.Abs(pitcher.bounds.max.x - catcher.bounds.min.x);
            float fromright = Mathf.Abs(pitcher.bounds.min.x - catcher.bounds.max.x);
            float fromup = Mathf.Abs(pitcher.bounds.min.y - catcher.bounds.max.y);
            float fromdown = Mathf.Abs(pitcher.bounds.max.y - catcher.bounds.min.y);
            //and save it all in this array
            float[] distances = new float[4];
            distances[0] = fromleft;
            distances[1] = fromright;
            distances[2] = fromup;
            distances[3] = fromdown;

            //after comparint all distanes we take the shortest distance and there's the edge you collided on!
            float shortest = Mathf.Min(distances);
            if (shortest == fromleft)
            {
                return LEFT;
            }
            if (shortest == fromright)
            {
                return RIGHT;
            }
            if (shortest == fromup)
            {
                return UP;
            }
            if (shortest == fromdown)
            {
                return DOWN;
            }
            //Vector3 distance= player.transform.position-transform.position;
            //player.transform.Translate(distance);
        }


        return MISS;


    }
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
