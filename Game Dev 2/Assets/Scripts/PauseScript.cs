using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript : MonoBehaviour {

    public static bool paused = false;
    public GameObject myUI;

    void Awake()
    {
        myUI.SetActive(false);
        Time.timeScale = 1f;
        paused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (paused)
            {
                myUI.SetActive(false);
                Time.timeScale = 1f;
                paused = false;
                Cursor.visible = false;
            }
            else
            {
                myUI.SetActive(true);
                Time.timeScale = 0f;
                paused = true;
                Cursor.visible = true;
            }
        }
    }

    public void Resume()
    {
        myUI.SetActive(false);
        Time.timeScale = 1f;
        paused = false;
        Cursor.visible = false;
    }
}
