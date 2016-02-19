using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]//Doing this ensures the gameobject this is attached to will have a BoxCollider2D
public class Heart : MonoBehaviour {

   
    BoxCollider2D playerCollider;
    int healPower=1;
  
                           // Use this for initialization
    void Start ()
    {
        //RectTransform rt = gameObject.GetComponent<RectTransform>();
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>();
       
    }
	
	// Update is called once per frame
	void Update ()
    {
        int collision=CollisionChecker.checkSide((Collider2D)playerCollider, (Collider2D)gameObject.GetComponent<BoxCollider2D>());
        if(collision!= CollisionChecker.MISS)
        {
            playerCollider.gameObject.GetComponent<Player>().heal(healPower);
            Destroy(gameObject);
        }

	}
}
