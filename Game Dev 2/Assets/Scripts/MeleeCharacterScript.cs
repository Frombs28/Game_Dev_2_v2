using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this is a subclass inheriting from CharacterScript
//this will overload behavior specific to this class, such as attacking and movement abilities
//much of this character's behavior will be handled by CharacterScript's base

public class MeleeCharacterScript : CharacterScript
{
    public float dashDistance = 10f;
    public float dashSpeed = 10f;
    public float AIDashSpeed = 150f; //ratio between this and enemySpeed should be the same ratio as that between the player's speed and dash speed 
    public float AIDashAcceleration = 700f;
    public float dashTime = 0.1f;
    public float dashCoolDown = 1f;
    private Vector3 dashDirection;
    private float dashStartTime;
    private float dashEndTime = 0;
    private Vector3 startPos;
    //change phase times when were ready to incorporate this
    public float phaseTime = 3f;
    public float phaseCoolDown = 6f;
    public float phaseSpeed = 4f;
    private float phaseStartTime;
    private float phaseEndTime = 0f;
    public float ammo_count = 25f;
    public float reload = 2f;
    public float fire_rate = 1f;
    public int my_health = 9;

    public bool dashing = false;
    //remember to override movespeed in the inspector!

    public override void SetEnemyHealth()
    {
        enemyhealth = my_health;
    }

    public override void Attack()
    {
        if(!amPlayer) { gameObject.SendMessage("FireEnemyGun"); }
        else if (ammo_count > 0 && timer >= fire_rate)
        {
            base.Attack();
            timer = 0f;
            gameObject.SendMessage("FireRifleGun");
            ammo_count--;
        }
        else if(ammo_count==0)
        {
            Reload();
        }
    }

    public override void Reload()
    {
        //do reload animation
        timer = -1 * reload;
        ammo_count = 25f;
    }

    public override float TraversalMaxTime()
    {
        return dashCoolDown;
    }

    public override float AbilityMaxTime()
    {
        return phaseCoolDown;
    }

    public override int Type()
    {
        return 0;
    }

    public override bool TraversalAbility() //i have a problem in the form of collisions not happening
    {
        base.TraversalAbility();
        if ((Time.time - dashEndTime) >= dashCoolDown && !dashing)
        {
            dashing = true;
            startPos = transform.position;
            dashStartTime = Time.time;
            //"flatten" the input axes to be trinarily 1 or 0 or -1 instead of a float between the three
            float myVert = Input.GetAxis("Vertical");
            float myHor = Input.GetAxis("Horizontal");
            if (myVert != 0)
            {
                if (myVert < 0) { myVert = -1; }
                else { myVert = 1; }
            }
            if (myHor != 0)
            {
                if (myHor < 0) { myHor = -1; }
                else { myHor = 1; }
            }
            if (myVert == 0 && myHor == 0) { myVert = 1; }
            dashDirection = new Vector3(myHor, 0, myVert);
            Vector3.Normalize(dashDirection);
            //inputManager.SendMessage("RechargeTraversal"); //commenting it out bcit gives error, probs just a problem with how i set up my scene
            StartCoroutine("Dash");
            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator Dash()
    {
        while ((Time.time - dashStartTime) <= dashTime)
        {
            if (amPlayer)
            {
                zeroMovement = false;
                interruptMovement = true;
                //transform.Translate(dashDirection * dashSpeed * Time.deltaTime);
                if (controller.isGrounded)
                {
                    moveDirection = dashDirection * dashSpeed;
                }
                else
                {
                    moveDirection = dashDirection * 0.5f * dashSpeed;
                }
                moveDirection = transform.TransformDirection(moveDirection);
                yield return null;
            }
            else
            {
                navAgent.speed = AIDashSpeed;
                navAgent.acceleration = AIDashAcceleration;
                Debug.Log("HERE I GO!");
                yield return null;
            }
        }
        navAgent.speed = enemySpeed;
        navAgent.acceleration = enemyAcceleration;
        interruptMovement = false;
        dashEndTime = Time.time;
        dashing = false;
    }

    public override bool MakeDistanceHelperOne()
    {
        //put a lerp here to actually face the player smoothly
        lookAtPlayer = true;
        return false;
    }
    public override bool MakeDistanceHelperTwo()
    {
        if (amPlayer) { return false; }
        float myDist = Vector3.Distance(player.transform.position, transform.position);
        if (myDist <= 10f)
        {
            //navAgent.ResetPath();
            navAgent.SetDestination(transform.position);
            return false;
        }
        if (TraversalAbility())
        {
            return true;
        }
        navAgent.SetDestination(player.transform.position);
        return true;
    }
}
