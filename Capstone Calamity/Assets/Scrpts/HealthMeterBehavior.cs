using UnityEngine;
using System.Collections;

public class HealthMeterBehavior : MonoBehaviour 
{
	public int HP; //used as index and heart count
	public int scaleX=5,scaleY=15,spacing=0; //deals with size and spacing of each heart(unit of HP)
	private Controller2D PC; 
	public Transform heart;//Prefab for the heart image
	public ArrayList heartMeter;

	// Use this for initialization
	void Start () 
	{
		GameObject player=GameObject.FindGameObjectWithTag("Player");
		if(player==null)
		{
			Debug.Log("Failed to find player");
			return;
		}
		PC = player.GetComponent<Controller2D>();
		if(PC==null)
		{
			Debug.Log("Failed to find Controller2D");
		}
		heartMeter=new ArrayList(3);
		HP=0;

		//make the smallest anchor be at (0,0) and highest anchor be (1,1)
		RectTransform rt= gameObject.GetComponent<RectTransform>();
		rt.anchorMax=new Vector2(0,1);
		rt.anchorMin=new Vector2(0,1);
		rt.pivot=new Vector2(0,1);

	}
	
	// Update is called once per frame
	void Update () 
	{
		if(HP==PC.hitPoints){return;} //we're fine, the HP_meter is up to date.

		if(HP>PC.hitPoints)//Looks like the player got hurt. time to break some hearts!
		{
			int deltaHearts=HP-PC.hitPoints;
			for(int i=0;i<deltaHearts;i++){breakHeart();}
		}
		else//Seems The player healed up since the last frame, lets show our support!
		{
			int deltaHearts=PC.hitPoints-HP;
			for(int i=0;i<deltaHearts;i++){makeHeart();} // <3 <3 <3
		}
	}

	void makeHeart()//create a heart and add it on the screen
	{
		//Create heart from the heart prefab and make sure its our child
		Transform firstHeart= Instantiate(heart);
		firstHeart.SetParent(transform);
		RectTransform rectHeart= (RectTransform) firstHeart;

		//make the smallest anchor be at (0,0) and highest anchor be (1,1)
		rectHeart.anchorMax=new Vector2(0,1);
		rectHeart.anchorMin=new Vector2(0,1);
		rectHeart.pivot=new Vector2(0,1);

		//This deals with the size and placement of the units
		rectHeart.anchoredPosition=new Vector2(HP*spacing,0);
		rectHeart.localScale=new Vector3(scaleX,scaleY,0);
		heartMeter.Add(rectHeart);
		HP++;

	
	}

	public void breakHeart()//Baby don't hurt me...
	{
		if(HP<=0){return;}

		HP--;
		Transform heartToBreak=(Transform)heartMeter[HP];
		heartMeter.RemoveAt(HP);
		GameObject.Destroy(heartToBreak.gameObject); //let 'em down easy
	}
}
