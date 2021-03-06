﻿using System.Collections;
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

    private void Awake()
    {
        InstantiateCharacter(spawn);
    }

    public void AssignPrefabNum(int myNum)
    //called from the character that the player just possessed into just now
    //sets prefabNum
    //tells us which character to re-spawn the player as when they die
    {
        if (myNum == 7) { myNum = 0; }
        prefabNum = myNum;
    }

    //makes a guy
    private GameObject InstantiateCharacter(Transform pos)
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
        myCharacter = Instantiate(myPrefab, pos.position, pos.rotation);
        inputManager.SendMessage("AssignPlayer", myCharacter);
        cam.SendMessage("AssignPlayer", myCharacter.transform.GetChild(1).gameObject);
        myPlayer = myCharacter;
        myCharacter.SendMessage("AssignPlayer", myPlayer);
        inputManager.SendMessage("PopulateCharacterList", myCharacter);
        inputManager.SendMessage("NewHealth", myPlayer.GetComponent<CharacterScript>().GetHealth());

        return myCharacter;
    }

    public void Respawn(Transform pos)
    //called from InputManager in Update once R is pressed after the player dies
    //plops the player back down at the last checkpoint, passed as a vector3
    //i will eventually add functionality to spawn the correct character type and with the correct health, etc.
    {
        myCharacter = InstantiateCharacter(pos);
        myCharacter.SendMessage("Invincibility");
        inputManager.SendMessage("AssignEveryCharacterPlayer", myCharacter);
    }

    void ChangeNumber(int val)
    {
        prefabNum = val;
    }
}