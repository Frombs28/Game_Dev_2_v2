using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public GameObject boom;
    GameObject curBoom;
    int damage = 2;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetDamage(int num)
    {
        damage = num;
    }

    public int GetDamage()
    {
        return damage;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Possessable")
        {
            curBoom = Instantiate(boom, transform);
        }
        Destroy(gameObject);
        //Debug.Log(collision.gameObject.tag);
    }
}
