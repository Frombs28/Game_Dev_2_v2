using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public GameObject meleePrefab;  //0
    public GameObject rangedPrefab; //1
    public GameObject sniperPrefab; //2
    public GameObject gruntPrefab;  //3
    public int prefabNum = 0;       //determines what enemy this spawner creates
    GameObject myCharacter;
    GameObject myPlayer;
    public GameObject inputManager;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void InstantiateCharacter()
    {
        GameObject myPrefab;
        if (prefabNum == 0)
        {
            myPrefab = meleePrefab;
        }
        else if (prefabNum == 1)
        {
            myPrefab = rangedPrefab;
        }
        else if (prefabNum == 2)
        {
            myPrefab = sniperPrefab;
        }
        else
        {
            myPrefab = gruntPrefab;
        }

        myCharacter = Instantiate(myPrefab, gameObject.transform.position, Quaternion.identity);
        myCharacter.SendMessage("AssignPlayer", myPlayer);
        inputManager.SendMessage("PopulateCharacterList", myCharacter);
    }

    void SetPlayer(GameObject player)
    {
        myPlayer = player;
    }
}
