using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamScript : MonoBehaviour
{
    public GameObject lookAtObject;
    private Vector3 lookAtTransform;
    private Vector3 temp;

    public float distance = 5f;
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
    bool free;
    bool wall_cam;

    public GameObject inputManager; //set in the inspector

    void Awake()
    {
        NormCam();
        free = true;
        wall_cam = false;
    }

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
        inputManager.SendMessage("NewHealth", newlookAtObject.GetComponent<CharacterScript>().GetHealth());
        NormCam();
        transitioning = false;
    }

    private void Update()
    {
        if (!transitioning && !PauseScript.paused)
        {
            currentX += Input.GetAxis("Mouse X") * sensitivityX;
            currentY += Input.GetAxis("Mouse Y") * sensitivityY;
            currentY = Mathf.Clamp(currentY, minY, maxY);

            if (wall_cam)
            {
                RaycastHit hit;
                Debug.DrawRay(transform.position, lookAtObject.transform.position - transform.position);
                if (Physics.Raycast(transform.position, ((lookAtObject.transform.position - transform.position) / Vector3.Distance(transform.position, lookAtObject.transform.position)), out hit, 4f))
                {
                    //if it cam hits a wall
                }
                else
                {
                    NormCam();
                    lookAtObject.transform.parent.gameObject.SendMessage("ChangeWallCam");
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (!transitioning && !PauseScript.paused && lookAtObject)
        {
            if (free)
            { 
                Vector3 direction = new Vector3(0, 0, -distance);
                Quaternion rotation = Quaternion.Euler(-currentY, currentX, 0);
                transform.position = lookAtObject.transform.position + (rotation * direction);
            }
            
            /*
            temp = new Vector3(offset, 0f, 0f);
            transform.position += temp;
            */
            //transform.Translate(Vector3.right * offset); //it's weird and i'm scared
            //moving this beneath the other stuff didn't help and just made it choppy

            lookAtTransform = lookAtObject.transform.position;
            //RaycastHit hit;
            //Debug.DrawRay(transform.position, lookAtObject.gameObject.transform.position - transform.position);
            //if (Physics.Raycast(transform.position, lookAtObject.gameObject.transform.position - transform.position, out hit))
            //{
            //    if (hit.transform != lookAtObject.transform)
            //    {
            //        transform.position = hit.transform.position + (rotation * direction);
            //    }
            //}
            //else
            //{
            //    Debug.Log("Nothing?");
            //}
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

    void SlowCam()
    {
        distance = 2f;
        sensitivityX = 2f;
        sensitivityY = 2f;
    }

    void AimCam()
    {
        gameObject.GetComponent<Camera>().fieldOfView = 30;
        distance = 5f;
        sensitivityX = 0.5f;
        sensitivityY = 0.5f;

        if (lookAtObject) { lookAtObject.SendMessage("CameraZoomedIn"); }
    }

    void NormCam()
    {
        gameObject.GetComponent<Camera>().fieldOfView = 60;
        sensitivityX = 2f;
        sensitivityY = 2f;
        distance = 5f;
        wall_cam = false;
        Debug.Log(wall_cam);
        if (lookAtObject) { lookAtObject.SendMessage("CameraNotZoomedIn"); }
    }

    void WallCam(Transform otherTransform)
    {
        //Vector3 direction = new Vector3(0, 0, Vector3.Distance(otherTransform.position, lookAtObject.transform.position));
        //Quaternion rotation = Quaternion.Euler(-currentY, currentX, 0);
        //transform.position = lookAtObject.transform.position + (rotation * direction);
        //free = false;
        distance = 1f;
        gameObject.GetComponent<Camera>().fieldOfView = 100;
        wall_cam = true;
        Debug.Log(wall_cam);
    }

    void NoWallCam()
    {
        //free = true;
        //distance = 5f;
        NormCam();
    }

    void Recoil(float amount)
    {
        //transform.Rotate(amount, 0f, 0f);
        currentY += amount;
    }
}
