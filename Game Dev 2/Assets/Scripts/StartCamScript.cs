using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartCamScript : MonoBehaviour {


    public Transform pos1;
    public Transform pos2;
    Transform dest;
    bool forward;
    public float lerpSpeed = 5f;
    float t;
    float startTime;
    Vector3 startPosition;
    float totalDist;

    void Start () {
        forward = true;
        dest = pos2;
        startTime = Time.time;
        startPosition = pos1.position;
	}
	
	void Update () {
        if((transform.position == pos2.position) && forward)
        {
            forward = false;
            dest = pos1;
            startTime = Time.time;
            startPosition = pos2.position;
        }
        else if((transform.position == pos1.position) && !forward)
        {
            forward = true;
            dest = pos2;
            startTime = Time.time;
            startPosition = pos1.position;
        }

        totalDist = Vector3.Distance(startPosition, dest.position);
        t = (float)(((Time.time - startTime) * lerpSpeed) / totalDist);
        transform.position = Vector3.Lerp(startPosition, dest.position, t);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }

    }

    public void StartGame()
    {
        SceneManager.LoadScene("level1", LoadSceneMode.Single);
    }
}
