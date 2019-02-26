using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//spawns characters and assigns virtual cameras to them

public class InstantiateScript : MonoBehaviour
{
    public GameObject inputManager;
    public GameObject meleePrefab;
    public GameObject rangedPrefab;
    public GameObject cam; //the main camera //for now just set in the inspector might do it dynamically in a hot sec
    public GameObject spawns;
    public int spawn_number = 1;

    private GameObject myCharacter; //to be instantiated

    private GameObject myPlayer;

    private void Start()
    {
        //populate the scene with some characters
        for (int i = 0; i < spawn_number; i++)
        {
            if (i == 0)
            {
                InstantiateCharacter(rangedPrefab, 
                    new Vector3(spawns.transform.GetChild(i).transform.position.x, 
                    spawns.transform.GetChild(i).transform.position.y, spawns.transform.GetChild(i).transform.position.z), true);
            }
            else
            {
                InstantiateCharacter(meleePrefab,
                    new Vector3(spawns.transform.GetChild(i).transform.position.x,
                    spawns.transform.GetChild(i).transform.position.y, spawns.transform.GetChild(i).transform.position.z), false);
            }
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //makes a guy
    private void InstantiateCharacter(GameObject myPrefab, Vector3 myPos, bool isPlayer)
    {
        myCharacter = Instantiate(myPrefab, myPos, Quaternion.identity);
        if (isPlayer)
        {
            inputManager.SendMessage("AssignPlayer", myCharacter);
            cam.SendMessage("AssignPlayer", myCharacter.transform.GetChild(1).gameObject);
            myPlayer = myCharacter;
        }
        myCharacter.SendMessage("AssignPlayer", myPlayer);
        inputManager.SendMessage("PopulateCharacterList", myCharacter);
    }
}