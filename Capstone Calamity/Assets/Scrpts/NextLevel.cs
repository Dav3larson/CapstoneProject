using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]//Doing this ensures the gameobject this is attached to will have a BoxCollider2D
public class NextLevel : MonoBehaviour {

	public string NextRoom;
	BoxCollider2D playerCollider;

	// Use this for initialization
	void Start () {
		if (NextRoom == null) {
			NextRoom = "SplashMenu";
		}
		playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {
		int collision=CollisionChecker.checkSide((Collider2D)playerCollider, (Collider2D)gameObject.GetComponent<BoxCollider2D>());
		if(collision!= CollisionChecker.MISS) {
			Debug.Log ("Touched the door, bruh");
			SceneManager.LoadScene(NextRoom);
		}
	}
}
