using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndScene : MonoBehaviour {

    public Text text;
    float time1;
    float time2;
	// Use this for initialization
	void Start () {
        time1 = PlayerPrefs.GetFloat("time1");
        time2 = PlayerPrefs.GetFloat("time2");
        text.text = "Your total time: " + (time1 + time2).ToString("#.00") + " seconds";
	}
	
	// Update is called once per frame
	void Update () {
        //Quit the game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            SceneManager.LoadScene("start", LoadSceneMode.Single);
        }
    }
}
