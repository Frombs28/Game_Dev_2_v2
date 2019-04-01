using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnMenuScript : MonoBehaviour {

    public GameObject myUI;

    void Awake()
    {
        myUI.SetActive(false);
    }

    void ActivateRespawnUI()
    {
        myUI.SetActive(true);
    }
    void DeActivateRespawnUI()
    {
        myUI.SetActive(false);
    }
}