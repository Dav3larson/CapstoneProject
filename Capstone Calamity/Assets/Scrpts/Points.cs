using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]//Doing this ensures the gameobject this is attached to will have a BoxCollider2D
public class Points : MonoBehaviour {


	BoxCollider2D playerCollider;
	// Would be easy to change the value and the image
	// Bigger diamonds or w/e are worth more points yayyyyy
	// Welcome to Whose Line is it Anyway, where the points don't matter!
	int value = 100;

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
			playerCollider.gameObject.GetComponent<GameManager>().addPoints(value);
			Destroy(gameObject);
		}

	}
}
