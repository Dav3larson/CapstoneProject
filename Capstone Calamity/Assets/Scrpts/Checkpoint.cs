using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class Checkpoint : MonoBehaviour {

    bool isOn;
    BoxCollider2D playerCollider, thisCollider;
    Player player;
    float deadTime = 1, timeSinceDeath = 0;

    // Use this for initialization
    void Start() {
        isOn = false;
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>();
        player = playerCollider.GetComponent<Player>();
        thisCollider = GetComponent<BoxCollider2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //Let's check if we should respawn the player
        if(player.playerState==Player.PlayerState.Dead//yo, you dead?
            &&Time.realtimeSinceStartup>timeSinceDeath+deadTime//have you been dead long enough?
            &&this==player.lastCheckPoint)//is it my job 
        {
            player.Respawn();
        }
        if (CollisionChecker.checkSide(playerCollider, thisCollider) != CollisionChecker.MISS)
        {
            isOn = true;
            player.lastCheckPoint = this;
            player.heal(player.maxHitPoint);
        }
        if(isOn)
        {
            //Debug.Log("Burning! we're burning!");
        }
    }
    public void SetRespawnTime()
    {        
        timeSinceDeath = Time.realtimeSinceStartup;
    }
}
