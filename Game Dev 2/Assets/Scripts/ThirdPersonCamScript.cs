using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamScript : MonoBehaviour
{
    public GameObject lookAtObject;
    private Vector3 lookAtTransform;
    private Vector3 temp;

    public float distance = 10f;
    public float sensitivityX = 4f;
    public float sensitivityY = 4f;
    public float offset = 5f;
    public float lookOffset = 5f;
    public float minY = -70f;
    public float maxY = 70f;
    private float currentX = 0f;
    private float currentY = 0f;

    public bool transitioning = false;
    private float startTime;
    public GameObject newlookAtObject;
    private Vector3 startPosition;
    private float totalDist;
    public float lerpSpeed = 5f;
    public float lerpCurve = 5f; //bigger value, bigger ease in / ease out
    private float t;

    public GameObject inputManager; //set in the inspector

    public void AssignPlayer(GameObject player)
    {
        lookAtObject = player;
    }

    //let's make the magic happen
    //gotta be a co-routine
    public void PossessionTransitionStarter(GameObject newPlayer)
    {
        transitioning = true;
        startTime = Time.time;
        newlookAtObject = newPlayer;
        startPosition = transform.position;
        totalDist = Vector3.Distance(startPosition, newlookAtObject.transform.position);
        t = 0;
        StartCoroutine("PossessionTransition");
    }

    IEnumerator PossessionTransition()
    {
        while (t < 1)
        {
            //basically trying to do a custom ease in / ease out
            //t = (float)((Time.deltaTime - startTime) * (lerpSpeed + ((1.5 - (Mathf.Max((0.5f - t), (t - 0.5f)))) * (lerpCurve * (1.5 - (Mathf.Max((0.5f - t), (t - 0.5f)))))))) / totalDist;
            //holy shit that's fucked up
            //it's supposed to go from 0 to 1 and that shit was hitting -8 my god
            //i'll try to re-do the math later but for now we're straight linear lerping.
            totalDist = Vector3.Distance(startPosition, newlookAtObject.transform.position); //bc the guy might move (tbh idk if this is the best solution or not i'd have to do the math)
            t = (float)(((Time.time - startTime) * lerpSpeed) / totalDist); //based on this, it will take longer when the thing is farther away //if you want it to take constant time, replace totalDist with a number
            //irl it'll probably be something like min(totalDist, constTimeNum) or whatevs
            transform.position = Vector3.Lerp(startPosition, newlookAtObject.transform.position, t);
            yield return null;
        }
        lookAtObject = newlookAtObject.transform.GetChild(1).gameObject;
        inputManager.SendMessage("SetReceiveInputTrue");
        transitioning = false;
    }

    private void Update()
    {
        if (!transitioning)
        {
            currentX += Input.GetAxis("Mouse X") * sensitivityX;
            currentY += Input.GetAxis("Mouse Y") * sensitivityY;
            currentY = Mathf.Clamp(currentY, minY, maxY);
        }
    }

    private void LateUpdate()
    {
        if (!transitioning)
        {
            Vector3 direction = new Vector3(0, 0, -distance);
            Quaternion rotation = Quaternion.Euler(-currentY, currentX, 0);
            transform.position = lookAtObject.transform.position + (rotation * direction);
            /*
            temp = new Vector3(offset, 0f, 0f);
            transform.position += temp;
            */
            //transform.Translate(Vector3.right * offset); //it's weird and i'm scared
            //moving this beneath the other stuff didn't help and just made it choppy

            lookAtTransform = lookAtObject.transform.position;
            /*
            temp = new Vector3(lookOffset * Vector3.Project(transform.rotation.eulerAngles, lookAtObject.transform.rotation.eulerAngles).x, 0f, 0f);
            lookAtTransform.position += temp;
            temp = new Vector3(0f, lookOffset * Vector3.Project(transform.rotation.eulerAngles, lookAtObject.transform.rotation.eulerAngles).y, 0f);
            lookAtTransform.position += temp;
            */
            transform.LookAt(lookAtTransform);
        }
    }

    void ChargeCam()
    {
        sensitivityX = 0.5f;
        sensitivityY = 0.5f;
    }

    void NormCam()
    {
        sensitivityX = 4f;
        sensitivityY = 4f;
    }
}
