using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Switch1To2 : MonoBehaviour {

    public GameObject inputManager;
    float time1;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == 2)
        {
            time1 = inputManager.GetComponent<InputManagerScript>().GetTime();
            PlayerPrefs.SetFloat("time1", time1);
            SceneManager.LoadScene("level2", LoadSceneMode.Single);
        }
    }
}
