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
    public float max_ammo = 25f;
    public float reload = 2f;
    public float fire_rate = 0.2f;
    public int my_health = 9;
    public int max_health = 9;

    private bool dashing = false;
    //remember to override movespeed in the inspector!

    public bool aiDash = false;
    public float aiDashTime = 0.1f;
    private float aiDashEndTime = 0f;
    private float aiDashStartTime = 0f;
    private float aiDashCooldown = 1f;
    public bool triggerInWall = false;

    public bool shield = false;


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
        if(!amPlayer) { gameObject.SendMessage("FireEnemyGun", "melee"); }
        else if (ammo_count > 0 && !reloading && timer >= fire_rate)
        {
            base.Attack();
            gameObject.SendMessage("FireRifleGun");
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
        //timer = -1 * reload;
        timer = 0;
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
        return dashCoolDown;
    }

    public override void Die()
    {
        if (!amPlayer)
        {
            inputManager.SendMessage("ChangeTime", 10);
        }
        base.Die();
    }

    public override float AbilityMaxTime()
    {
        return phaseCoolDown;
    }

    public override int Type()
    {
        return 0;
    }

    public override void TraversalAbility() //i have a problem in the form of collisions not happening
    {
        if ((Time.time - dashEndTime) >= dashCoolDown && !dashing)
        {
            myAnimator.SetBool("dash", true);

            base.TraversalAbility();
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
            inputManager.SendMessage("RechargeTraversal");

            StartCoroutine("Dash");
        }
    }

    public override void Ability()
    {
        if ((Time.time - phaseEndTime) >= phaseCoolDown && controller.isGrounded)
        {
            phaseStartTime = Time.time;
            inputManager.SendMessage("RechargeAbility");
            StartCoroutine("Phase");
        }
    }

    IEnumerator Dash()
    {
        while ((Time.time - dashStartTime) <= dashTime)
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
        interruptMovement = false;
        dashEndTime = Time.time;
        dashing = false;
        myAnimator.SetBool("dash", false);

    }

    IEnumerator Phase()
    {
        while ((Time.time - phaseStartTime) <= phaseTime)
        {
            gameObject.SendMessage("FireRifleGun");
            yield return null;
        }
        interruptMovement = false;
        phaseEndTime = Time.time;
    }

    private bool StartAiDash()
    //returns false if the conditions to start the AI Dash are not met
    //returns true and flips the bool 'aiDash' to true if these conditions are met
    {
        if ((Time.time - aiDashEndTime) < aiDashCooldown || triggerInWall)
        {
            return false;
        }
        aiDash = true;
        aiDashStartTime = Time.time;
        return true;
    }

    public override void Update()
    {
        base.Update();

        //dash stuff
        if ((Time.time - aiDashStartTime) >= aiDashTime || triggerInWall)
        {
            if (aiDash)
            {
                aiDashEndTime = Time.time;
            }
            aiDash = false;
        }
        if (aiDash)
        {
            navAgent.angularSpeed = 10;
            transform.Translate(Vector3.forward * Time.deltaTime * 50f);
        }
        else
        {
            navAgent.angularSpeed = 120;
        }

        //shield stuff
        if (!shield)
        {
            if (my_health / max_health <= 0.3f)
            {
                shield = true;
                Collider myCollider = transform.Find("Shield").gameObject.GetComponent<Collider>();
                myCollider.enabled = true;
                Renderer myRenderer = transform.Find("Shield").gameObject.GetComponent<Renderer>();
                myRenderer.enabled = true;
            }
        }
        else
        {
            //Debug.Log("HEY!!!");
            if ((my_health / max_health) > 0.3f) //<-- this boi right here doesn't feel like being true ever
            {
                shield = false;
                Collider myCollider = transform.Find("Shield").gameObject.GetComponent<Collider>();
                myCollider.enabled = false;
                Renderer myRenderer = transform.Find("Shield").gameObject.GetComponent<Renderer>();
                myRenderer.enabled = false;
            }
        }
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
        if (!player)
        {
            aggro = false;
            return true;
        }
        float myDist = Vector3.Distance(player.transform.position, transform.position);
        if (myDist <= 15f)
        {
            navAgent.SetDestination(transform.position);
            return false;
        }
        if (!aiDash)
        {
            StartAiDash();
        }
        navAgent.SetDestination(player.transform.position);
        return true;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Wall")
        {
            triggerInWall = true;
        }
    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Wall")
        {
            triggerInWall = false;
        }
    }

    public override void RespawnAsThisCharacter()
    {
        instantiateManager.SendMessage("AssignPrefabNum", 0);
    }
}
