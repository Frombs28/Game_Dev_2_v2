using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSpawner : MonoBehaviour {

    public List<Sprite> dialogues;
    bool triggered;
    public Image image;
    public float dialogue_timer = 0f;
    public float dialogue_length = 5f;
	// Use this for initialization
	void Start () {
        triggered = false;
        dialogue_timer = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == 2 && !triggered)
        {
            int i = Random.Range(0, dialogues.Count);
            image.sprite = dialogues[i];
            image.enabled = true;
            dialogue_timer = Time.time;
            StartCoroutine("Timer");
            triggered = true;
        }
    }

    IEnumerator Timer()
    {
        while ((Time.time - dialogue_timer) <= dialogue_length)
        {
            yield return null;
        }
        image.enabled = false;
    }

    void Reset()
    {
        triggered = false;
    }
}
