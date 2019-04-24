using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndDialogue : MonoBehaviour {

    public List<Sprite> images;
    public Image picture;
    bool begin;
    int i;
    public GameObject inputManager;

    // Use this for initialization
    void Start () {
        begin = false;
        i = 0;
        picture.enabled = false;
        picture.sprite = images[0];
        i++;
	}
	
	// Update is called once per frame
	void Update () {
        if (begin)
        {
            if ((i < images.Count) && (Input.GetKeyDown(KeyCode.Space)))
            {
                picture.sprite = images[i];
                i++;
            }
            else if (i == images.Count && (Input.GetKeyDown(KeyCode.Space)))
            {
                Time.timeScale = 1f;
                picture.enabled = false;
                inputManager.SendMessage("SetReceiveInputTrue");
            }
        }
	}

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == 2)
        {
            Time.timeScale = 0f;
            begin = true;
            picture.enabled = true;
            inputManager.SendMessage("SetReceiveInputFalse");
        }
    }
}
