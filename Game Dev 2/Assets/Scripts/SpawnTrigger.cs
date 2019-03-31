using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTrigger : MonoBehaviour {

    Transform spawns;
    bool spawned;
    int spawn_number = 0;
    // Use this for initialization
    void Start () {
        spawned = false;
        spawns = gameObject.transform.GetChild(0);
        spawn_number = spawns.childCount;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void Reset()
    {
        spawned = false;
    }

    void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.layer == 2 && !spawned)
        {
            for (int i = 0; i < spawn_number; i++)
            {
                spawns.GetChild(i).SendMessage("SetPlayer", collider.gameObject);
                spawns.GetChild(i).SendMessage("InstantiateCharacter", false);
            }
            spawned = true;
        }
    }
}
