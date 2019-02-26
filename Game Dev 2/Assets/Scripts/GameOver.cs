using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour {

    public Button reset;

	// Use this for initialization
	void Start () {
        reset.onClick.AddListener(Reset);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void Reset()
    {
        SceneManager.LoadScene("Scene1");
    }
}
