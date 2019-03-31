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
                                            //also set false when the player gets dead and then set back to true when they respawn
    private int playerhealth=10;
    float timer = 0f;
    float possess_timer = 0f;
    public float possession_rate = 0.5f;
    private bool startingPossessing = false; //flag for slomo
    public Slider healthBar;
    public Slider movementBar;
    public Slider abilityBar;
    public Text ammo_num;
    float traversalRechargeStartTime;
    float abilityRechargeStartTime;
    public bool attack_mode;

    public GameObject reticle;

    public GameObject CheckpointManager;
    public Canvas myCanvas;
    public bool playerIsAlive;
    public GameObject instantiateManager;

    void Start()
    {
        /*
        healthBar.value = playerhealth;
        movementBar.maxValue = player.gameObject.GetComponent<CharacterScript>().TraversalMaxTime();
        abilityBar.maxValue = player.gameObject.GetComponent<CharacterScript>().AbilityMaxTime();
        */
        //^^^not doing bars rn
        traversalRechargeStartTime = 0f;
        abilityRechargeStartTime = 0f;
        attack_mode = true;
        SetAmmoText();
        playerIsAlive = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        
        timer += Time.deltaTime;
        possess_timer += Time.deltaTime;
        /*
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
        */
        //^^^ stuff having to do with UI things that i'm not bothering to re-implement bc i didn't make them and i'm tryna do that AI rn

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
        if (Input.GetButtonDown("TraversalAbility") && player && receiveInput && !PauseScript.paused) { player.SendMessage("TraversalAbility"); }
        if (Input.GetButtonDown("Ability") && player && receiveInput && !PauseScript.paused) { player.SendMessage("Ability"); }

        //possession
        //if (Input.GetAxis("Possess") != 0 && player && !possessing)
        //if pressed, start timer
        if (Input.GetButtonDown("Attack") && player && receiveInput && !attack_mode && !PauseScript.paused)
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
        if (possess_timer >= possession_rate && Input.GetButtonUp("Attack") && player && receiveInput && !attack_mode && !PauseScript.paused)
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
                    SetAmmoText();
                    //movementBar.maxValue = player.gameObject.GetComponent<CharacterScript>().TraversalMaxTime();
                    //abilityBar.maxValue = player.gameObject.GetComponent<CharacterScript>().AbilityMaxTime();
                    //transition the camera
                    mainCam.SendMessage("PossessionTransitionStarter", hit.collider.gameObject);
                    timer = Time.deltaTime;

                    //for each character, assign the new player
                    foreach (GameObject character in characters)
                    {
                        character.SendMessage("AssignPlayer", hit.collider.gameObject);
                    }
                }
            }
        }

        if (Input.GetButton("Attack") && attack_mode && !PauseScript.paused)
        {
            player.SendMessage("Attack");
            SetAmmoText();
        }

        if (Input.GetButtonDown("Aim") && !PauseScript.paused)
        {
            mainCam.SendMessage("AimCam");
        }

        if (Input.GetButtonUp("Aim"))
        {
            mainCam.SendMessage("NormCam");
        }

        if (Input.GetButtonUp("Attack") || !attack_mode)
        {
            player.SendMessage("StopAttack");
        }

        if (Input.GetButtonDown("Switch"))
        {
            if (attack_mode)
            {
                attack_mode = false;
            }
            else
            {
                attack_mode = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && !playerIsAlive)
        //respawn
        {
            Debug.Log("HRRRNNNNNGGGGHHHH i'm trying to respawn but i can't do it :(");
            Vector3 respawnPoint = CheckpointManager.GetComponent<CheckpointManagerScript>().GetLastCheckpoint();
            Debug.Log("Here it is: ");
            Debug.Log(respawnPoint.x);
            Debug.Log(" ");
            Debug.Log(respawnPoint.y);
            Debug.Log(" ");
            Debug.Log(respawnPoint.z);
            instantiateManager.SendMessage("Respawn", respawnPoint);
            myCanvas.SendMessage("DeActivateRespawnUI");

        }
    }

    public void AssignPlayer(GameObject myPlayer)
    {
        player = myPlayer;
        player.layer = 2; //ignore raycast //should probably eventually change to custom layer
        NewHealth(myPlayer.GetComponent<CharacterScript>().GetHealth());
        //healthBar.maxValue = myPlayer.GetComponent<CharacterScript>().GetMaxHealth();
        receiveInput = true;
        playerIsAlive = true;
    }

    public void NewHealth(int new_health)
    {
        playerhealth = new_health;
    }

    public void PopulateCharacterList(GameObject myCharacter)
    {
        characters.Add(myCharacter);
    }
    public void RemoveCharacterFromList(GameObject myCharacter) //call this from the gameobject when it dies
    {
        characters.Remove(myCharacter);
    }

    public void TookDamage(int damage) //plz capitalize every word in your function names as per the standard many thank
    {
        playerhealth -= damage;
        //healthBar.value = playerhealth;
        if (playerhealth <= 0)
        {
            GameOver();
        }
        //player.SendMessage("TakeDamage");
    }

    public void GameOver()
    {
        player.SendMessage("Die");
        //SceneManager.LoadScene("GameOver");
        myCanvas.SendMessage("ActivateRespawnUI");
        receiveInput = false;
        playerIsAlive = false;
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

    public void SetAmmoText()
    {
        ammo_num.text = player.gameObject.GetComponent<CharacterScript>().GetCurAmmo() + " / " + player.gameObject.GetComponent<CharacterScript>().GetMaxAmmo();
    }
}