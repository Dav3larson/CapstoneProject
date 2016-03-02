using UnityEngine;
using System.Collections;

public class TextImport : MonoBehaviour {

    
    public TextAsset textFile;
    public string[] textLines;
	// Use this for initialization
	void Start ()
    {
	    if(textFile)
        {
            textLines = (textFile.text.Split('\n'));//we seperate all the text by lines
        }
	}
	
	
}
