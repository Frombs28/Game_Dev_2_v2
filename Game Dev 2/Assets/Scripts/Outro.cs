using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Outro : MonoBehaviour
{


    public List<Sprite> images;
    public Image picture;
    int i;
    // Use this for initialization
    void Start()
    {
        i = 0;
        picture.sprite = images[0];
        i++;
    }

    // Update is called once per frame
    void Update()
    {
        if ((i < images.Count) && (Input.GetKeyDown(KeyCode.Space)))
        {
            NextSlide();
        }
        else if (i == images.Count && (Input.GetKeyDown(KeyCode.Space)))
        {
            SceneManager.LoadScene("end", LoadSceneMode.Single);
        }
    }

    void NextSlide()
    {
        picture.sprite = images[i];
        i++;
    }
}
