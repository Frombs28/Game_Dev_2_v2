using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InputManagerScript : MonoBehaviour
{
    private List<GameObject> characters = new List<GameObject>();

    public GameObject player; //whoms't'd've'ever is possessed rn
    private Camera mainCam; //the main camera in the scene, which should usually be showing the player's POV
    public bool receiveInput = true; //flag //set false by this script when i start possessing; set true by the camera (which sends a message to this) when the transition is complete
    private int playerhealth=10;
    float timer = 0f;
    float possess_timer = 0f;
    public float fire_rate = 1f;
    public float possession_rate = 0.5f;
    private bool startingPossessing = false; //flag for slomo
    public Slider healthBar;
    public Slider movementBar;
    public Slider abilityBar;
    float traversalRechargeStartTime;
    float abilityRechargeStartTime;
    int num_shots = 15;
    int reload_speed = 3;

    public GameObject reticle;

    //when the player possesses a character, this is called to set the new character to our variable "player," which is used in turn to call movement functions on whichever character the player is controlling
    //remember to set the old player's layer back to 0 (so it can be hit by raycasts and be possessed) before you call this
    //i would put that in this function but setting up optional arguments is a hassle
    //in fact i'll have to do this if we want to call this from anywhere other than here so i'll probably actually do it eventually
    //this function is also called by InstantiateScript to determine who the player is when the scene just starts
    void Start()
    {
        healthBar.value = playerhealth;
        movementBar.maxValue = player.gameObject.GetComponent<CharacterScript>().TraversalMaxTime();
        abilityBar.maxValue = player.gameObject.GetComponent<CharacterScript>().AbilityMaxTime();
        traversalRechargeStartTime = 0f;
        abilityRechargeStartTime = 0f;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        possess_timer += Time.deltaTime;
        if(Time.deltaTime - traversalRechargeStartTime < movementBar.maxValue)
        {
            movementBar.value = Time.deltaTime - traversalRechargeStartTime;
        }
        else
        {
            movementBar.value = movementBar.maxValue;
        }
        if (Time.deltaTime - abilityRechargeStartTime < abilityBar.maxValue)
        {
            abilityBar.value = Time.deltaTime - abilityRechargeStartTime;
        }
        else
        {
            abilityBar.value = abilityBar.maxValue;
        }

        //player movement
        //if the player is pressing the WASD keys, call a function on the CharacterScript of whatever character the player is controlling
        //if ((Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0) && player && receiveInput) { player.SendMessage("MovePlayer"); }
        //if (player && receiveInput) { player.SendMessage("MovePlayer"); }
        //if (Input.GetButton("Jump") && player && receiveInput) { player.SendMessage("JumpPlayer"); }
        if (player && receiveInput) { player.SendMessage("RotatePlayer"); }

        //attack and traversal ability
        //if the player is pressing the appropriate keys, call a function on the CharacterScript of whatever character the player is controlling
        //if (Input.GetAxis("Attack") != 0 && player && receiveInput) { player.SendMessage("Attack"); }
        //else if (Input.GetAxis("TraversalAbility") != 0 && player && receiveInput) { player.SendMessage("TraversalAbility"); }
        //if (Input.GetAxis("Attack") != 0 && player && receiveInput) { player.SendMessage("Attack"); }
        if (Input.GetButtonDown("TraversalAbility") && player && receiveInput) { player.SendMessage("TraversalAbility"); }
        if (Input.GetButtonDown("Ability") && player && receiveInput) { player.SendMessage("Ability"); }

        //possession
        //if (Input.GetAxis("Possess") != 0 && player && !possessing)
        //if pressed, start timer
        if (Input.GetMouseButtonDown(1) && player && receiveInput)
        {
            possess_timer = 0f;
            reticle.SendMessage("Possessing");

            startingPossessing = true;
        }
        /*
        if (Input.GetMouseButtonUp(1) || possess_timer >= possession_rate)
        {
            startingPossessing = false;
        }
        if (startingPossessing)
        {
            Time.timeScale = 0.5f;
        }
        else
        {
            Time.timeScale = 1f;
        }
        */

        //ZA WARUDO! TOMARE TOKI WO!
        //^^^if ya want the slo-mos, un-comment that and also speed up the time it takes to possess someone (possession_rate in this script) and the animation on the reticle (literally just open the animator, select the reticle in the heirarchy, and change "speed" in the animator)

        //if released after enough time has passed, trigger possession
        if (possess_timer >= possession_rate && Input.GetMouseButtonUp(1) && player && receiveInput)
        {
            //do a raycast from the main camera
            mainCam = GameObject.Find("Main Camera").GetComponent<Camera>();
            RaycastHit hit; //this will contain a path to a reference to whatever GameObject got hit
            int layerMask = 1 << 2;
            layerMask = ~layerMask; //the raycast will ignore anything on this layer

            if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.collider.gameObject.tag == "Possessable")
                {
                    receiveInput = false;
                    //set the player to the new character
                    player.layer = 0; //i would put this in AssignPlayer but it's a hassle so do it here
                    AssignPlayer(hit.collider.gameObject);
                    movementBar.maxValue = player.gameObject.GetComponent<CharacterScript>().TraversalMaxTime();
                    abilityBar.maxValue = player.gameObject.GetComponent<CharacterScript>().AbilityMaxTime();
                    //transition the camera
                    mainCam.SendMessage("PossessionTransitionStarter", hit.collider.gameObject);

                    //for each character, assign the new player
                    foreach (GameObject character in characters)
                    {
                        character.SendMessage("AssignPlayer", hit.collider.gameObject);
                    }
                }
            }
        }

        if (timer >= fire_rate && Input.GetButton("Attack") && num_shots >= 0)
        {
            if (num_shots > 0)
            {
                timer = 0f;
                player.SendMessage("Attack");
                num_shots -= 1;
            }
            else
            {
                timer = -1 * reload_speed;
                num_shots = 15;
            }
        }
        if (Input.GetButtonUp("Attack"))
        {
            player.SendMessage("StopAttack");
        }
    }

    public void AssignPlayer(GameObject myPlayer)
    {
        player = myPlayer;
        player.layer = 2; //ignore raycast //should probably eventually change to custom layer
    }

    public void PopulateCharacterList(GameObject myCharacter)
    {
        characters.Add(myCharacter);
    }
    public void RemoveCharacterFromList(GameObject myCharacter) //call this from the gameobject when it dies
    {
        characters.Remove(myCharacter);
    }

    public void TookDamage() //plz capitalize every word in your function names as per the standard many thank
    {
        playerhealth -= 1;
        healthBar.value = playerhealth;
        if (playerhealth <= 0)
        {
            GameOver();
        }
        //player.SendMessage("TakeDamage");
    }

    public void GameOver()
    {
        player.SendMessage("Die");
        SceneManager.LoadScene("GameOver");
    }

    public void SetReceiveInputTrue()
    {
        receiveInput = true;
    }

    void RechargeTraversal()
    {
        traversalRechargeStartTime = Time.deltaTime;
    }

    void RechargeAbility()
    {
        abilityRechargeStartTime = Time.deltaTime;
    }
}