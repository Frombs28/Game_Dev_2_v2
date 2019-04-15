using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusScript : MonoBehaviour {

    void CameraNotZoomedIn()
    {
        transform.localPosition = new Vector3(1.8f, 2.5f, 0f);
    }

    void CameraZoomedIn()
    {
        transform.localPosition = new Vector3(0.7f, 1.7f, 2f);
    }
}
