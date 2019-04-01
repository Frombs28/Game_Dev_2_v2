using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointScript : MonoBehaviour {

    public bool activated = false;
    public Vector3 myPosition;

    void Awake()
    {
        myPosition = transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            activated = true;
        }
    }
}
