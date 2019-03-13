using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boom : MonoBehaviour {

    float scale;
    public float speed = 2f;
    public float size = 3.2f;
    
    // Use this for initialization
	void Start () {
        scale = 1f;
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.localScale = new Vector3(scale, scale, scale);
        scale += speed*Time.deltaTime;
        if(scale >= size)
        {
            //Destroy(gameObject);
            Debug.Log("derp");
        }
	}
}
