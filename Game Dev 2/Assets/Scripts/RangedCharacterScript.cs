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

    private bool dashing = false;
    //remember to override movespeed in the inspector!

    public override void Attack()
    {
        gameObject.SendMessage("FireShortGun");
    }

    public override float TraversalMaxTime()
    {
        return dashCoolDown;
    }

    public override float AbilityMaxTime()
    {
        return shieldCoolDown;
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
            StartCoroutine("Shield");
        }
    }

    public override bool IsCharging()
    {
        Debug.Log(dashing);
        return dashing;
    }

    void StopCharging()
    {
        StopCoroutine("Charge");
        interruptMovement = false;
        cam.SendMessage("NormCam");
        dashEndTime = Time.time;
        dashing = false;
    }

    IEnumerator Charge()
    {
        while ((Time.time - dashStartTime) <= dashTime)
        {
            invincible = true;
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
        while ((Time.time - shieldStartTime) <= shieldTime)
        {
            invincible = true;
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
        shieldEndTime = Time.time;
        Debug.Log("Done Shielding!");
    }
}
