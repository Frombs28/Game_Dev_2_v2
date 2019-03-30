using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//spawns characters and assigns virtual cameras to them

public class InstantiateScript : MonoBehaviour
{
    public GameObject inputManager;
    public GameObject meleePrefab;  //0
    public GameObject rangedPrefab; //1
    public GameObject sniperPrefab; //2
    public GameObject gruntPrefab;  //3
    public GameObject cam; //the main camera //for now just set in the inspector might do it dynamically in a hot sec
    public Transform spawn;
    public int prefabNum = 0;       //determines what player spawns as

    private GameObject myCharacter; //to be instantiated

    private GameObject myPlayer;

    private void Start()
    {
        InstantiateCharacter();   
    }

    //makes a guy
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
        myCharacter = Instantiate(myPrefab, spawn.position, Quaternion.identity);
        inputManager.SendMessage("AssignPlayer", myCharacter);
        cam.SendMessage("AssignPlayer", myCharacter.transform.GetChild(1).gameObject);
        myPlayer = myCharacter;
        myCharacter.SendMessage("AssignPlayer", myPlayer);
        inputManager.SendMessage("PopulateCharacterList", myCharacter);
    }

    void ChangeNumber(int val)
    {
        prefabNum = val;
    }
}