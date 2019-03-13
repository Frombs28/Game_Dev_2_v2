using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public GameObject boom;
    GameObject curBoom;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Possessable")
        {
            curBoom = Instantiate(boom, transform);
        }
        Destroy(gameObject,0.01f);
        Debug.Log(collision.gameObject.tag);
    }
}
