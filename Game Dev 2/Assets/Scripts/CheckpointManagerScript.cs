using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CheckpointManagerScript : MonoBehaviour {

    [SerializeField]
    public List<GameObject> checkpoints = new List<GameObject>();
    public int numCheckpoints;
    //this script is reliant on these ^ two values being set correctly in the inspector; there's no error checking
    //put the checkpoints in the list in increasing order from first to last; we iterate through them backwards so we find the most recently activated one

    private CheckpointScript myCheckScript;

    public Transform GetLastCheckpoint()
    //called from the function "GameOver" in InputManager
    //tells it the transform to yeet the player into when they up and die
    {
        Debug.Log("made it into GetLastCheckpoint");
        for (int i = numCheckpoints-1; i >= 0; i--)
        {
            Debug.Log("looping");
            myCheckScript = checkpoints[i].GetComponent<CheckpointScript>();
            if (myCheckScript.activated)
            {
                return myCheckScript.myTransform;
            }
        }
        //ya done messed up if you ever reach this point
        //just yeet the player into space
        return gameObject.transform;
    }

}
