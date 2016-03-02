using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextboxManager : MonoBehaviour
{
    public GameObject textbox;
    public Text content;

    public TextAsset textFile;
    public string[] textLines;
    public int currentLine;
    public int lastLine;

    public Player player;
    void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        //Make Player state for text box
        if (textFile)
        {
            textLines = (textFile.text.Split('\n'));//we seperate all the text by lines
        }
        if(lastLine==0)
        {
            lastLine = textLines.Length - 1;
        }
        gameObject.SetActive(false);
    }

    void Update()
    {
        content.text = textLines[currentLine];
        
        if (Input.GetKeyDown(KeyCode.Space)|| Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            currentLine=(currentLine+1);
        }
        if(currentLine>=textLines.Length)
        {
            Disable(); 
        }
    }
    public void Enable()
    {
        gameObject.SetActive(true);
    }
    public void Disable()
    {
        gameObject.SetActive(false);
        currentLine = 0;
        player.playerState = Player.PlayerState.Standing;
    }
}