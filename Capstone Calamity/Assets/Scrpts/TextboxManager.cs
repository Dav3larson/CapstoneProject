using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextboxManager : MonoBehaviour
{
    public TextAsset textFile;
    private int firstLine=0;
    public int currentLine;
    public int lastLine;

    public GameObject textbox;
    public Text content;
    public string[] textLines;
   

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
        content.text = null;
    }

    void Update()
    {
        content.text = textLines[currentLine];
        
        if (Input.GetKeyDown(KeyCode.Space)|| Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            currentLine=(currentLine+1);
        }
        if(currentLine>=lastLine)
        {
            Disable(); 
        }
    }
    public void Enable()
    {
        gameObject.SetActive(true);
    }
    public void Enable(TextAsset file, int start, int end)
    {
        Debug.Log("Enable worked!");
        gameObject.SetActive(true);
        textFile = file;
        firstLine = start;
        currentLine = start;
        lastLine = end;
        textLines = (textFile.text.Split('\n'));//we seperate all the text by lines
        Debug.Log(textFile.text);
    }
    public void Disable()
    {
        gameObject.SetActive(false);
        currentLine = firstLine;
        player.playerState = Player.PlayerState.Standing;
        content.text = "";
    }
}