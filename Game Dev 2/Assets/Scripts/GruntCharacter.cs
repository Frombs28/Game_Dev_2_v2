using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this is a subclass inheriting from CharacterScript
//this will overload behavior specific to this class, such as attacking and movement abilities
//much of this character's behavior will be handled by CharacterScript's base

public class GruntCharacter : CharacterScript
{
    public float ammo_count = 20f;
    public float max_ammo = 20f;
    public float reload = 2f;
    public float fire_rate = 0.4f;
    public int my_health = 5;
    public int max_health = 5;


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
        if (!amPlayer) { gameObject.SendMessage("FireEnemyGun", "grunt"); }
        else if (ammo_count > 0 && !reloading && timer >= fire_rate)
        {
            base.Attack();
            gameObject.SendMessage("FireGruntGun");
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
        return 0;
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
        return 0;
    }

    public override int Type()
    {
        return 0;
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
