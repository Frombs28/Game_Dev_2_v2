using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//the base character class
//inherited by MeleeCharacterScript, RangedCharacterScript, and SniperCharacter
//controls AI behavior as well as player behavior

public class CharacterScript : MonoBehaviour
{
    public bool grounded;

    public GameObject player;
    public bool amPlayer;
    public GameObject inputManager;

    public NavMeshAgent navAgent;

    public Animator myAnimator;

    public CharacterController controller;
    public Vector3 moveDirection;
    public bool interruptMovement = false;
    public bool zeroMovement = true;
    public float moveSpeed = 20f; //how fast the character can move //this should be overridden
    public float jumpSpeed = 50f;
    public float gravity = 20f;
    int num_jumps = 0;
    public int enemyhealth = 3;
    public int cur_bullet = 0;
    public float enemySpeed = 20f;
    public bool invincible = false;
    GameObject marker;
    bool marker_bool = true;

    public bool reloading;

    public Material notPossessed;
    public Material possessed;
    Material material;

    GameObject figure;

    public Camera cam; //player character rotation is based on camera rotation //this is the MAIN CAMERA,  *not*  your personal VIRTUAL CAMERA

    public string state = "none";
    public bool lookAtPlayer = false;
    public bool lookAwayFromPlayer = false;
    public bool hittingWall = false;
    public bool aggro = false; //true if you're within a certain distance of the player or just got hit //resets after a few seconds
    private bool canStartAggroTimer = true;
    public float distanceToAggro = 25f;
    public float timer = 100f;

    private void Awake()
    //this function is mostly used to get references to components on this game object
    //also used to set initial values of some variables
    {
        //get a reference to the main camera
        //you'll need to do this every time you change cameras in the future
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        controller = GetComponent<CharacterController>();
        navAgent = GetComponent<NavMeshAgent>();
        inputManager = GameObject.Find("InputManager");
        navAgent.speed = enemySpeed;
        myAnimator = gameObject.GetComponentInChildren<Animator>();
        figure = gameObject.transform.GetChild(3).gameObject;
        marker = GameObject.Find("Marker");
        material = GetComponent<Material>();
        SetEnemyHealth();
        reloading = false;
    }

    public void AssignPlayer(GameObject myPlayer)
    //called from InputManager on every character whenever a possession takes place
    //called from InstantiateScript on this object once it is instantiated
    //this function sets the value of "player," which points towards the character game object currently being controlled
    //this function also sets the value of "amPlayer," the boolean that tells us whether or not this gameobject is being controlled
    {
        player = myPlayer;
        amPlayer = (gameObject == player);
        if (amPlayer)
        {
            state = "none";
            navAgent.enabled = false;
            myAnimator.SetBool("walk", false);
            material = possessed;
        }
        else {
            navAgent.enabled = true;
            //myAnimator.SetBool("walk", true);
            material = notPossessed;
        }
    }

    public virtual int Type()
    {
        return 0;
    }

    public virtual void RotatePlayer()
    //this function is only called if this gameobject is currently possessed
    //this fucnction is called from InputManager in Update
    //this function rotates this gameobject to face in the same direction as the camera is currently pointing
    {
        transform.rotation = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0);
    }

    public virtual void Update()
    {
        grounded = controller.isGrounded;
        timer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.R) && !PauseScript.paused)
        {
            Reload();
        }

        if (!interruptMovement && amPlayer && !PauseScript.paused)
        //player movement
        {
            if (grounded)
            //set moveDirection based on keyboard input
            {
                moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection *= moveSpeed;
                num_jumps = 0;
                myAnimator.SetBool("jumping", false);
            }
            if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 || num_jumps > 0))
            //stop the player from moving this frame if there is no keyboard input
            {
                zeroMovement = false;
            }
            else
            {
                zeroMovement = true;
            }

            //RaycastHit hit;
            //Debug.DrawRay(transform.position, cam.gameObject.transform.position - transform.position);
            //if (Physics.Raycast(transform.position, cam.gameObject.transform.position - transform.position, out hit))
            //{
            //    if (hit.collider.gameObject != cam)
            //    {
            //        Debug.Log(hit.collider.gameObject);
            //        cam.transform.position = hit.point;
            //    }
            //}
            //else
            //{
            //    Debug.Log("Nothing?");
            //}

            //if(Input.GetAxis("Vertical") > 0 && Input.GetAxis("Horizontal") == 0)
            //{
            //    figure.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            //}
            //else if (Input.GetAxis("Vertical") > 0 && Input.GetAxis("Horizontal") > 0)
            //{
            //    figure.transform.rotation = Quaternion.Euler(new Vector3(0, 45, 0));
            //}
            //else if (Input.GetAxis("Vertical") == 0 && Input.GetAxis("Horizontal") > 0)
            //{
            //    figure.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
            //}
            //else if (Input.GetAxis("Vertical") < 0 && Input.GetAxis("Horizontal") > 0)
            //{
            //    figure.transform.rotation = Quaternion.Euler(new Vector3(0, 135, 0));
            //}
            //else if (Input.GetAxis("Vertical") < 0 && Input.GetAxis("Horizontal") == 0)
            //{
            //    figure.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            //}
            //else if (Input.GetAxis("Vertical") < 0 && Input.GetAxis("Horizontal") < 0)
            //{
            //    figure.transform.rotation = Quaternion.Euler(new Vector3(0, 225, 0));
            //}
            //else if (Input.GetAxis("Vertical") == 0 && Input.GetAxis("Horizontal") < 0)
            //{
            //    figure.transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
            //}
            //else if (Input.GetAxis("Vertical") > 0 && Input.GetAxis("Horizontal") < 0)
            //{
            //    figure.transform.rotation = Quaternion.Euler(new Vector3(0, 315, 0));
            //}

            if (Input.GetButtonDown("Jump") && num_jumps < 1)
            {
                moveDirection.y += jumpSpeed;
                num_jumps += 1;
                myAnimator.SetBool("jumping", true);
            }
        }

        if (zeroMovement)
        {
            moveDirection = new Vector3(0f, moveDirection.y, 0f);
            myAnimator.SetBool("run", false);
        }
        else
        {
            myAnimator.SetBool("run", true);
        }
        if (!amPlayer)
        {
            myAnimator.SetBool("run", false);
        }
        moveDirection.y -= (gravity * Time.deltaTime);

        controller.Move(moveDirection * Time.deltaTime);

        if (!amPlayer)
        {
            //navAgent.SetDestination(marker.transform.position);

        //    //put some stuff here about firing le gun
        //    if (Random.Range(0f, 100) <= 1)
        //    {
        //        gameObject.SendMessage("FireEnemyGun");
        //    }
        }
        
        //some AI stuff
        //initializes the AI behavior tree if this gameobject has just been possessed out of
        if (!amPlayer && state == "none")
        {
            state = "idle";
            StartCoroutine("Idle");
        }

        //more AI stuff
        //header for AggroTimer
        float myDist = Vector3.Distance(player.transform.position, transform.position);
        if (myDist <= distanceToAggro)
        {
            aggro = true;
            if (canStartAggroTimer) { StartCoroutine("AggroTimer"); }
        }

        if (lookAtPlayer || amPlayer) { lookAwayFromPlayer = false; }
        if (lookAwayFromPlayer || amPlayer) { lookAtPlayer = false; }
        if (lookAtPlayer)
        {
            transform.LookAt(player.transform);
        }
        if (lookAwayFromPlayer)
        {
            Vector3 myVect = 2 * transform.position - player.transform.position;
            transform.LookAt(myVect);
        }
    }
    private void LateUpdate()
    {
        //zeroMovement = true;
        if (!amPlayer)
        {
            if(gameObject.transform.position.z >= 21.0f)
            {
                marker = GameObject.Find("Marker");
            }
            else if(gameObject.transform.position.z <= 1.0f)
            {
                marker = GameObject.Find("Marker2");
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //AI stuff//
    IEnumerator AggroTimer()
    //called from Update if we're close to the player
    //called from OnCollisionEnter with a projectile
    //allows this character to de-aggro if they have been aggro for x seconds and no longer meet the conditions for aggro
    {
        canStartAggroTimer = false;
        float startTime = Time.time;
        while (Time.time - startTime < 3f)
        {
            yield return null;
        }
        aggro = false;
        canStartAggroTimer = true;
    }

    IEnumerator Idle()
    {
        navAgent.ResetPath();
        while (!aggro)
        {
            yield return null;
        }
        state = "facingPlayer";
        StartCoroutine("FacePlayer");
    }

    IEnumerator FacePlayer()
    {
        /*
        Quaternion initialRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(player.transform.position - transform.position);
        float startTime = Time.time;
        float t = 0;

        while (Vector3.Angle(targetRotation.eulerAngles, (player.transform.position - transform.position)) > 0.01f)
        {
            t = (Time.time - startTime) / 1f;
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, t);
            yield return null;
        }
        */
        //^^^idk why that's not working
        yield return null; //comment this out if you get the stuff up there working
        lookAtPlayer = true;
        if (!amPlayer)
        {
            if (aggro)
            {
                state = "makingDistance";
                StartCoroutine("MakeDistance");
            }
            else
            {
                state = "idle";
                StartCoroutine("Idle");
            }
        }
    }
    
    
    IEnumerator MakeDistance()
    {
        //call a virtual function
        //stay in this coroutine until that function returns true
        while (MakeDistanceHelperOne())
        {
            yield return null;
        }
        
        while (MakeDistanceHelperTwo())
        {
            yield return null;
        }
        if (!amPlayer)
        {
            if (aggro)
            {
                state = "circling";
                StartCoroutine("Circle");
            }
            else
            {
                state = "idle";
                StartCoroutine("Idle");
            }
        }
    }
    
    IEnumerator Circle()
    {
        bool strafingRight = (Random.value >= 0.5f);
        float startTime = Time.time;
        while (Time.time - startTime <= 1.5f)
        {
            if (strafingRight)
            {
                Vector3 myVect = transform.TransformDirection(Vector3.right);
                myVect *= 10;
                myVect += transform.position;
                navAgent.SetDestination(myVect);
            }
            else
            {
                Vector3 myVect = transform.TransformDirection(-Vector3.right);
                myVect *= 10;
                myVect += transform.position;
                navAgent.SetDestination(myVect);
            }
            yield return null;
        }
        if (!amPlayer)
        {
            if (aggro)
            {
                state = "firing";
                StartCoroutine("Fire");
            }
            else
            {
                state = "idle";
                StartCoroutine("Idle");
            }
        }
    }

    IEnumerator Fire()
    {
        int i = 0;
        while (i < 5)
        {
            Attack();
            yield return new WaitForSeconds(0.5f);
            i++;
        }
        if (!amPlayer)
        {
            if (aggro)
            {
                state = "makingDistance";
                StartCoroutine("MakeDistance");
            }
            else
            {
                state = "idle";
                StartCoroutine("Idle");
            }
        }
    }
    //AI stuff//
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    //the virtual stuff that must be overloaded by the subclasses
    public virtual bool MakeDistanceHelperOne() { return true; } //turns the character to face in the desired direction, returns true as long as this has not been successful
    public virtual bool MakeDistanceHelperTwo() { return true; } //moves the character in the desired direction, returns true as long as distance has not been made
    public virtual void Attack()
    {
        myAnimator.SetBool("firing", true);
    }
    public virtual void StopAttack()
    {
        myAnimator.SetBool("firing", false);
    }
    public virtual bool IsCharging() { return false; }
    public virtual void TraversalAbility() { }
    public virtual void Reload() { }
    public virtual void Ability() { }
    public virtual float TraversalMaxTime() { return 0f; }
    public virtual float AbilityMaxTime() { return 0f; }
    public virtual void SetEnemyHealth() { }
    public virtual float GetMaxAmmo() { return 0f; }
    public virtual float GetCurAmmo() { return 0f; }
    public virtual int GetMaxHealth() { return 0; }
    public virtual void TakeDamage(int damage)
    {
        enemyhealth -= damage;
        if (enemyhealth <= 0 && !amPlayer)
        {
            Die();
        }
        else if(enemyhealth <=0 && amPlayer)
        {
            
        }
    }

    public virtual void Die()
    {
        inputManager.SendMessage("RemoveCharacterFromList", gameObject);
        myAnimator.SetBool("die", true);
        Destroy(gameObject, 0.5f);
    }

    public int GetHealth()
    {
        return enemyhealth;
    }

    public virtual void HasJumped()
    {
        num_jumps += 1;
    }

    void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.tag == "Projectile" && !invincible)
        {
            if(collider.gameObject.layer == 9)
            {
                TakeDamage(collider.gameObject.GetComponent<Bullet>().GetDamage());
                aggro = true;
                if (canStartAggroTimer) { StartCoroutine("AggroTimer"); }
            }
            else if(amPlayer)
            {
                inputManager.SendMessage("TookDamage", collider.gameObject.GetComponent<Bullet>().GetDamage());
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
        if(gameObject.GetComponent<RangedCharacterScript>() && collider.gameObject.tag != "Possessable" && collider.gameObject.tag != "Projectile") //projectile was spelled wrong
        {
            gameObject.SendMessage("StopCharging");
        }
        if (collider.gameObject.tag == "Wall")
        {
            hittingWall = true;
        }
    }
    void OnCollisionExit(Collision collider)
    {
        if (collider.gameObject.tag == "Wall")
        {
            hittingWall = false;
        }
    }
}