using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using System.Text;
using System;
public class Iam : MonoBehaviour {
    private Text Text;
    
    [SerializeField]
    public Component scriptText;
    
    [SerializeField]
    public TextAsset myText;
    // Use this for initialization
    void Start () {
        Text = gameObject.GetComponent<Text>();
        scriptText = GameObject.Find("/Login/textI").GetComponent("Text(Script)");
        
        //  StartCoroutine(textView());


        StartCoroutine(textView());
    }

    // Update is called once per frame
    void Update() {

    }


    
    private IEnumerator textView(){

        
        Text.text = "yaniv and \n eran ";

        char[] delimiterChars = { '.'};

        string text = myText.ToString();
        System.Console.WriteLine("Original text: '{0}'", text);

        string[] words = text.Split(delimiterChars);
        System.Console.WriteLine("{0} words in text:", words.Length);

        foreach (string s in words)
        {
            Text.text = s;
            yield return new WaitForSeconds(1);
        }
        throw new NotImplementedException();
    }



    
}
