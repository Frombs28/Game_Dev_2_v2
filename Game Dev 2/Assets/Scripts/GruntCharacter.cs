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
    public float fire_rate = 1.2f;
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
        if (!amPlayer) { gameObject.SendMessage("FireEnemyGun"); }
        else if ((ammo_count > 0 || reloading) && timer >= fire_rate)
        {
            base.Attack();
            if (reloading)
            {
                reloading = false;
                ammo_count = max_ammo;
                myAnimator.SetBool("relaod", false);

            }
            gameObject.SendMessage("FireGruntGun");
            timer = 0f;
            ammo_count--;
        }
        else if (ammo_count == 0 && !reloading)
        {
            Reload();
        }
    }

    public override void Reload()
    {
        //do reload animation
        timer = -1 * reload;
        reloading = true;
    }

    public override float TraversalMaxTime()
    {
        return 0;
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
