using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Switch2ToEnd : MonoBehaviour
{
    public GameObject inputManager;
    float time2;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == 2)
        {
            time2 = inputManager.GetComponent<InputManagerScript>().GetTime();
            PlayerPrefs.SetFloat("time2", time2);
            SceneManager.LoadScene("outro", LoadSceneMode.Single);
        }
    }
}
