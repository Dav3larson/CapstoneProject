using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

    public Controller2D playerController;
    public Vector2 focusAreaSize;
    FocusArea focusArea;

    public float lookaheadDistanceX;
    public float lookSmoothTimeX;
    public float verticalSmoothTime;
    public float verticalOffset;

    float currentLookAheadX;
    float targetLookAheadX;
    float lookAheadDirX;
    float smoothLookVelocityX;
    float smoothVelocityY;

    bool lookAheadStopped;

    struct FocusArea
    {
        public Vector2 center,velocity;

        float left, right, top, bottom;
        public FocusArea(Bounds bounds, Vector2 size)//this is the area where the player can move WITHOUT moving the camera
        {
            left = bounds.center.x - size.x / 2;
            right = bounds.center.x + size.x / 2;
            bottom = bounds.min.y;
            top = bounds.min.y + size.y;
            center = new Vector2((bounds.center.x), (top + bottom )/ 2);
            velocity = Vector2.zero;
        }
        public void Update(Bounds targetBounds)
        {
            float shiftX = 0;
            if(targetBounds.min.x<left)//if the player moved left since we last checked...
            {
                shiftX = targetBounds.min.x - left;//then the focus rectangle will move left
            }
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }
            //now we know how much the player shifted horizontally, update the horiztonal values
            left += shiftX;
            right += shiftX;
            float shiftY = 0;
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;//
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }
            //Same story in the Y direction. Not hard stuff so far
            top += shiftY;
            bottom += shiftY;
            center = new Vector2((left + right)/2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);
        }
    }
	// Use this for initialization
	void Start ()
    {
        focusAreaSize = new Vector2(3, 5);
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<Controller2D>();
        focusArea = new FocusArea(playerController.GetComponent<BoxCollider2D>().bounds, focusAreaSize);
        //focusArea = new FocusArea(playerController.playerCollider.bounds, focusAreaSize);

    }
	
	// Update is called once per frame
	void LateUpdate ()
    {
        focusArea.Update(playerController.GetComponent<BoxCollider2D>().bounds);
        Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;

        if(focusArea.velocity.x!=0)
        {
            lookAheadDirX = Mathf.Sign(focusArea.velocity.x);//find which direction we're facing
            //if the direction we're going is the same way the controlstick is facing....
            if(Mathf.Sign(playerController.GetComponent<Player>().input.x)==Mathf.Sign(focusArea.velocity.x)
                && playerController.GetComponent<Player>().input.x!=0)
            {
                lookAheadStopped = false;
                targetLookAheadX = lookAheadDirX * lookaheadDistanceX;//this is where we aim to go
            }
            else //otherwise...
            {
                if(!lookAheadStopped)//if we haven't changed our direction in the last frame but did it now...
                {
                    //we make the change to reflect that with a slightly differnt distance
                    lookAheadStopped = true;
                    targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookaheadDistanceX - currentLookAheadX) / 4f;
                }
            }
        }

        //Debug.Log("Smooth Velocity X " + smoothLookVelocityX);
        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);//this is how far we'll get there this frame
        focusPosition += Vector2.right * currentLookAheadX;
        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
        
        
        transform.position = (Vector3)focusPosition + Vector3.forward * -10;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawCube(focusArea.center, focusAreaSize);
    }
}
