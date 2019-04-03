using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this is a subclass inheriting from CharacterScript
//this will overload behavior specific to this class, such as attacking and movement abilities
//much of this character's behavior will be handled by CharacterScript's base

public class RangedCharacterScript : CharacterScript
{
    public float dashDistance = 10f;
    public float dashSpeed = 2f;
    public float dashTime = 1f;
    public float dashCoolDown = 3f;
    private Vector3 dashDirection;
    private float dashStartTime;
    private float dashEndTime = 0f;
    private Vector3 startPos;

    public float shieldTime = 3f;
    public float shieldCoolDown = 6f;
    public float shieldSpeed = 4f;
    private float shieldStartTime;
    private float shieldEndTime = 0f;
    public float ammo_count = 5f;
    public float max_ammo = 5f;
    public float reload = 3f;
    public float fire_rate = 2f;
    public int my_health = 40;
    public int max_health = 40;

    public GameObject shield;
    public Transform shieldPos;

    private bool waitingForTimer = false;

    private bool dashing = false;
    //remember to override movespeed in the inspector!

    public override void SetEnemyHealth()
    {
        enemyhealth = my_health;
    }

    public override float GetMaxAmmo()
    {
        return max_ammo;
    }

    public override int GetMaxHealth()
    {
        return max_health;
    }

    public override float GetCurAmmo()
    {
        return ammo_count;
    }

    public override void Attack()
    {
        if (!amPlayer) { gameObject.SendMessage("FireEnemyGun"); }
        else if ((ammo_count > 0 || reloading) && timer >= fire_rate)
        {
            base.Attack();
            if (reloading)
            {
                reloading = false;
                ammo_count = max_ammo;
                myAnimator.SetBool("reload", false);
                inputManager.SendMessage("SetAmmoText");

            }
            gameObject.SendMessage("FireShortGun");
            timer = 0f;
            ammo_count--;
        }
        else if(ammo_count==0 && !reloading)
        {
            Reload();
        }
    }

    public override void ResetHealth()
    {
        enemyhealth = max_health;
    }

    public override void Reload()
    {
        //do reload animation
        timer = -1 * reload;
        reloading = true;
    }

    public override float TraversalMaxTime()
    {
        return dashCoolDown;
    }

    public override float AbilityMaxTime()
    {
        return shieldCoolDown;
    }

    public override int Type()
    {
        return 1;
    }

    public override void TraversalAbility() //i have a problem in the form of collisions not happening
    {
        if ((Time.time - dashEndTime) >= dashCoolDown && !dashing && controller.isGrounded)
        {
            dashing = true;
            startPos = transform.position;
            dashStartTime = Time.time;
            dashDirection = new Vector3(0, 0, 1);
            Vector3.Normalize(dashDirection);
            cam.SendMessage("ChargeCam");
            myAnimator.SetBool("charge", true);
            inputManager.SendMessage("RechargeTraversal");
            StartCoroutine("Charge");
        }
    }

    public override void Ability()
    {
        if ((Time.time - shieldEndTime) >= shieldCoolDown && controller.isGrounded)
        {
            shieldStartTime = Time.time;
            inputManager.SendMessage("RechargeAbility");
            myAnimator.SetBool("shield", true);
            StartCoroutine("Shield");
        }
    }

    public override bool IsCharging()
    {
        //Debug.Log(dashing); //commenting this out so i can test some other stuff
        return dashing;
    }

    void StopCharging()
    {
        StopCoroutine("Charge");
        interruptMovement = false;
        cam.SendMessage("NormCam");
        dashEndTime = Time.time;
        dashing = false;
        myAnimator.SetBool("charge", false);

    }

    IEnumerator Charge()
    {
        while ((Time.time - dashStartTime) <= dashTime)
        {
            //invincible = true;
            zeroMovement = false;
            interruptMovement = true;
            //transform.Translate(dashDirection * dashSpeed * Time.deltaTime);
            moveDirection = dashDirection * dashSpeed;
            moveDirection = transform.TransformDirection(moveDirection);
            yield return null;
        }
        interruptMovement = false;
        cam.SendMessage("NormCam");
        dashEndTime = Time.time;
        dashing = false;
    }

    IEnumerator Shield()
    {
        GameObject newShield;
        newShield = Instantiate(shield, shieldPos);
        while ((Time.time - shieldStartTime) <= shieldTime)
        {
            //invincible = true;
            interruptMovement = true;
            if (controller.isGrounded)
            {
                zeroMovement = false;
                moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection *= shieldSpeed;
            }
            if (Input.GetButtonDown("Jump") && controller.isGrounded)
            {
                moveDirection.y += jumpSpeed;
            }
            yield return null;
        }
        interruptMovement = false;
        Destroy(newShield);
        shieldEndTime = Time.time;
        Debug.Log("Done Shielding!");
        myAnimator.SetBool("shield", false);

    }

    IEnumerator RandomTimer()
    {
        waitingForTimer = true;
        navAgent.speed = enemySpeed * 2;
        navAgent.acceleration *= 2;
        yield return new WaitForSeconds(Random.Range(1f, 4f));
        navAgent.speed = enemySpeed;
        navAgent.acceleration /= 2;
        yield return new WaitForSeconds(Random.Range(1f, 4f));
        waitingForTimer = false;
    }

    public override void Update()
    {
        base.Update();

        //change speed sporadically
        if (aggro)
        {
            if (!waitingForTimer)
            {
                StartCoroutine("RandomTimer");
            }
        }
    }

    public override bool MakeDistanceHelperOne()
    {
        lookAtPlayer = false;
        //put a lerp here to actually face away from the player smoothly
        lookAwayFromPlayer = true;
        return false;
    }
    public override bool MakeDistanceHelperTwo() //need a way to have the guy stop after a certain amount of time has passed / he walked into a wall    //actually flipping a bool to true as long as ya boi is hitting a wall might be helpful
    {
        if (amPlayer) { return false; }
        float myDist = Vector3.Distance(player.transform.position, transform.position);
        if (myDist >= 20 || hittingWall)
        {
            navAgent.ResetPath();
            lookAwayFromPlayer = false;
            lookAtPlayer = true;
            return false;
        }
        Vector3 myVect = 2 * transform.position - player.transform.position;
        navAgent.SetDestination(myVect);
        return true;
    }
}
