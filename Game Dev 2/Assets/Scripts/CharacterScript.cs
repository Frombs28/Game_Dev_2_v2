using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//the base character class
//all character types will inherit from this class
//sub-classes will override functions specific to thier own behavior, such as attacking and using movement abilities
//this base class will contain behavior that every character would use, such as movement and AI
//this class also allows the character to be controlled by the player

public class CharacterScript : MonoBehaviour
{
    public bool grounded;

    public GameObject player;
    public bool amPlayer;
    public GameObject inputManager;

    private NavMeshAgent navAgent;

    public CharacterController controller;
    public Vector3 moveDirection;
    public bool interruptMovement = false;
    public bool zeroMovement;
    public float moveSpeed = 20f; //how fast the character can move //this should be overridden
    public float jumpSpeed = 50f;
    public float gravity = 20f;
    int num_jumps = 0;
    public int enemyhealth = 3;
    public float enemySpeed = 20f;
    public bool invincible = false;

    public Camera cam; //player character rotation is based on camera rotation //this is the MAIN CAMERA,  *not*  your personal VIRTUAL CAMERA
    

    public int Enemyhealth
    {
        get
        {
            return enemyhealth;
        }
        set
        {
            enemyhealth = value;
        }
    }

    private void Awake()
    {
        //get a reference to the main camera
        //you'll need to do this every time you change cameras in the future
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        controller = GetComponent<CharacterController>();
        navAgent = GetComponent<NavMeshAgent>();
        inputManager = GameObject.Find("InputManager");
        navAgent.speed = enemySpeed;
    }

    public void AssignPlayer(GameObject myPlayer)
    {
        player = myPlayer;
        amPlayer = (gameObject == player);
        if (amPlayer)
        {
            navAgent.enabled = false;
        }
        else { navAgent.enabled = true; }
    }

    //movement if this character is possessed by the player
    //this function gets called from InputManager
    public void MovePlayer()
    {
        
        //if (controller.isGrounded && !interruptMovement) //okay so apprently it's never grounded? idk fam //except when i'm pressing a WASD button
        //if (!interruptMovement && controller.isGrounded)
        //{
        //    zeroMovement = false;
        //    if (moveDirection.y > 0)
        //    {
        //        moveDirection.y -= (gravity * Time.deltaTime);
        //        moveDirection = new Vector3(Input.GetAxis("Horizontal"), moveDirection.y, Input.GetAxis("Vertical")).normalized;
        //    }
        //    else
        //    {
        //        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
        //    }
        //    moveDirection = transform.TransformDirection(moveDirection);
        //    moveDirection *= moveSpeed;
        //}
    }

    public void JumpPlayer()
    {
        //if (controller.isGrounded)
        //{
        //    moveDirection.y = jumpSpeed;
        //}
    }

    //rotation based on camera rotation if this character is possessed by the player
    //this fucntion gets called from InputManager
    public virtual void RotatePlayer()
    {
        //this isn't perfect but it works for now
        transform.rotation = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0);
    }

    //insert a bunch of functions to receive from the AI controller
    //movement if this character is not possessed by the player
    //have this stuff affect moveDirection and just call move() in update regardless of whether you're the player or not I THINK I DON'T KNOW HOW MUCH WE'RE GONNA USE CHARACTER CONTROLLERS FOR THE AI but rn it seems like a good idea

    private void Update()
    {
        //debuggin
        grounded = controller.isGrounded;

        if (!interruptMovement && amPlayer)
        {
            if (controller.isGrounded)
            {
                zeroMovement = false;
                moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection *= moveSpeed;
                num_jumps = 0;
            }
            if (Input.GetButtonDown("Jump") && num_jumps < 2)
            {
                moveDirection.y += jumpSpeed;
                num_jumps += 1;
            }
        }


        if (zeroMovement) { moveDirection = new Vector3(0f, moveDirection.y, 0f); }
        moveDirection.y -= (gravity * Time.deltaTime);
        controller.Move(moveDirection * Time.deltaTime);

        //if (!amPlayer)
        //{
        //    navAgent.SetDestination(player.transform.position);

        //    //put some stuff here about firing le gun
        //    if (Random.Range(0f, 100) <= 1)
        //    {
        //        gameObject.SendMessage("FireEnemyGun");
        //    }
        //}
    }
    private void LateUpdate()
    {
        //zeroMovement = true;
    }
    
    //the virtual stuff that must be overloaded by the subclasses
    public virtual void Attack() { }
    public virtual bool IsCharging() { return false; }
    public virtual void TraversalAbility() { }
    public virtual void Ability() { }
    public virtual float TraversalMaxTime() { return 0f; }
    public virtual float AbilityMaxTime() { return 0f; }
    public virtual void TakeDamage(int damage)
    {
        enemyhealth -= damage;
        if (enemyhealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        inputManager.SendMessage("RemoveCharacterFromList", gameObject);
        Destroy(gameObject, 0.01f);
    }

    void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.tag == "Projectile" && !invincible)
        {
            if(collider.gameObject.layer == 9)
            {
                TakeDamage(1);
            }
            else if(amPlayer)
            {
                inputManager.SendMessage("TookDamage");
            }
            Destroy(collider.gameObject);
        }
        if(collider.gameObject.layer == 2 && !invincible)
        {
            collider.gameObject.SendMessage("IsCharging", gameObject);
            if (collider.gameObject.GetComponent<RangedCharacterScript>())
            {
                if (collider.gameObject.GetComponent<RangedCharacterScript>().IsCharging())
                {
                    if (!amPlayer)
                    {
                        TakeDamage(6);
                    }
                }
            }
        }
        if(gameObject.GetComponent<RangedCharacterScript>() && collider.gameObject.tag != "Possessable" && collider.gameObject.tag != "Proejectile")
        {
            gameObject.SendMessage("StopCharging");
        }
    }
}