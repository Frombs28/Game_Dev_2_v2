using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Intro : MonoBehaviour {


    public List<Sprite> images;
    public Image picture;
    float curTime;
    public float duration;
    int i;
    // Use this for initialization
    void Start () {
        curTime = Time.time;
        i = 0;
        picture.sprite = images[0];
        i++;
	}
	
	// Update is called once per frame
	void Update () {
		if((i < images.Count) && ((Time.time - curTime) > duration))
        {
            NextSlide();
        }
        else if(i == images.Count)
        {
            SceneManager.LoadScene("level1", LoadSceneMode.Single);
        }
	}

    void NextSlide()
    {
        picture.sprite = images[i];
        curTime = Time.time;
        i++;
    }
}
