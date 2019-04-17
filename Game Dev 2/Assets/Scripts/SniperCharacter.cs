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
    public float max_ammo = 1f;
    public float reload = 4f;
    public float fire_rate = 0.1f;
    public int my_health = 12;
    public int max_health = 12;

    private bool dashing = false;
    //remember to override movespeed in the inspector!

    private bool perchInRange = false;
    private Vector3 perchPosition;
    

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
        if (!amPlayer) { gameObject.SendMessage("FireEnemyGun", "sniper"); }
        else if (ammo_count > 0 && !reloading && timer >= fire_rate)
        {
            base.Attack();
            gameObject.SendMessage("FireSniperGun");
            timer = 0f;
            ammo_count--;
        }
        else if (ammo_count == 0 && !reloading)
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
        myAnimator.SetBool("reload", true);
        StartCoroutine("ReloadTime");
    }

    IEnumerator ReloadTime()
    {
        for (int i = 0; i < reload_circles.Count; i++)
        {
            reload_circle.sprite = reload_circles[i];
            reload_circle.enabled = true;
            yield return new WaitForSeconds(reload / 8);
        }
        reloading = false;
        reload_circle.enabled = false;
        ammo_count = max_ammo;
        myAnimator.SetBool("reload", false);
        inputManager.SendMessage("SetAmmoText");
    }

    public override float TraversalMaxTime()
    {
        return jumpCoolDown;
    }

    public override void Die()
    {
        if (!amPlayer)
        {
            inputManager.SendMessage("ChangeTime", 5);
        }
        base.Die();
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
            //cam.SendMessage("SlowCam");
            StartCoroutine("Slow");
        }
    }

    IEnumerator Jump()
    {
        //moveDirection.y += newJumpSpeed;
        float myJumpSpeed = jumpSpeed;
        jumpSpeed = newJumpSpeed;
        while ((Time.time - jumpStartTime) <= jumpTime)
        {
            //HasJumped();
            //myAnimator.SetBool("jumping", true);
            //moveDirection = transform.TransformDirection(moveDirection);
            yield return null;
        }
        jumpSpeed = myJumpSpeed;
        jumpEndTime = Time.time;
        //dashing = false;
    }

    IEnumerator Slow()
    {
        while ((Time.time - slowStartTime) <= slowTime)
        {
            Time.timeScale = 0.25f;
            yield return null;
        }
        Time.timeScale = 1f;
        slowEndTime = Time.time;
        //cam.SendMessage("NormCam");
        Debug.Log("Done slow!");
    }

    public override bool MakeDistanceHelperOne()
    {
        lookAtPlayer = false;
        //put a lerp here to actually face away from the player smoothly
        lookAwayFromPlayer = true;
        return false;
    }
    public override bool MakeDistanceHelperTwo()
    {
        if (amPlayer) { return false; }
        if (!player)
        {
            aggro = false;
            return true;
        }
        float myDist = Vector3.Distance(player.transform.position, transform.position);
        if ((myDist >= 20 && !perchInRange) || hittingWall || (perchInRange && (new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(perchPosition.x, 0, perchPosition.z)).magnitude <= 0.5f))
        {
            navAgent.ResetPath();
            lookAwayFromPlayer = false;
            lookAtPlayer = true;
            return false;
        }
        if (!perchInRange)
        {
            Vector3 myVect = 2 * transform.position - player.transform.position;
            navAgent.SetDestination(myVect);
        }
        else
        {
            navAgent.SetDestination(perchPosition);
        }
        return true;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Perch" && !perchInRange)
        {
            perchInRange = true;
            perchPosition = other.transform.position;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Perch")
        {
            perchInRange = false;
        }
    }
}
