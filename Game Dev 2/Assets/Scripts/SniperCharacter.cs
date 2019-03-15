using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperCharacter : CharacterScript
{
    public float jumpDistance = 10f;
    public float newJumpSpeed = 40f;
    public float jumpTime = 0.1f;
    public float jumpCoolDown = 1f;
    private float jumpStartTime;
    private float jumpEndTime = 0;
    //change phase times when were ready to incorporate this
    public float slowTime = 3f;
    public float slowCoolDown = 6f;
    public float slowSpeed = 4f;
    private float slowStartTime;
    private float slowEndTime = 0f;
    public float ammo_count = 1f;
    public float reload = 4f;
    public float fire_rate = 0.1f;
    public int my_health = 12;

    private bool dashing = false;
    //remember to override movespeed in the inspector!

    public override void SetEnemyHealth()
    {
        enemyhealth = my_health;
    }

    public override void Attack()
    {
        if (!amPlayer) { gameObject.SendMessage("FireEnemyGun"); }
        else if (ammo_count > 0 && timer >= fire_rate)
        {
            base.Attack();
            gameObject.SendMessage("FireSniperGun");
            timer = 0f;
            ammo_count--;
        }
        else if(ammo_count == 0)
        {
            Reload();
        }
    }

    public override void Reload()
    {
        //do reload animation
        timer = -1 * reload;
        ammo_count = 1f;
    }

    public override float TraversalMaxTime()
    {
        return jumpCoolDown;
    }

    public override float AbilityMaxTime()
    {
        return slowCoolDown;
    }

    public override int Type()
    {
        return 2;
    }

    public override void TraversalAbility() //i have a problem in the form of collisions not happening
    {
        if ((Time.time - jumpEndTime) >= jumpCoolDown && !dashing)
        {
            base.TraversalAbility();
            dashing = true;
            jumpStartTime = Time.time;
            inputManager.SendMessage("RechargeTraversal");
            StartCoroutine("Jump");
        }
    }

    public override void Ability()
    {
        if ((Time.time - slowEndTime) >= slowCoolDown && controller.isGrounded)
        {
            slowStartTime = Time.time;
            inputManager.SendMessage("RechargeAbility");
            cam.SendMessage("SlowCam");
            StartCoroutine("Slow");
        }
    }

    IEnumerator Jump()
    {
        moveDirection.y += newJumpSpeed;
        while ((Time.time - jumpStartTime) <= jumpTime)
        {
            HasJumped();
            myAnimator.SetBool("jumping", true);
            moveDirection = transform.TransformDirection(moveDirection);
            yield return null;
        }
        jumpEndTime = Time.time;
        dashing = false;
    }

    IEnumerator Slow()
    {
        while ((Time.time - slowStartTime) <= slowTime)
        {
            Time.timeScale = 0.5f;
            yield return null;
        }
        Time.timeScale = 1f;
        slowEndTime = Time.time;
        cam.SendMessage("NormCam");
        Debug.Log("Done slow!");
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
        navAgent.SetDestination(player.transform.position);
        return true;
    }
}
