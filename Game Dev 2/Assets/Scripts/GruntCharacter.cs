using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this is a subclass inheriting from CharacterScript
//this will overload behavior specific to this class, such as attacking and movement abilities
//much of this character's behavior will be handled by CharacterScript's base

public class GruntCharacter : CharacterScript
{
    public float ammo_count = 20f;
    public float reload = 2f;
    public float fire_rate = 1.2f;
    public int my_health = 5;


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
            timer = 0f;
            gameObject.SendMessage("FireGruntGun");
            ammo_count--;
        }
        else if (ammo_count == 0)
        {
            Reload();
        }
    }

    public override void Reload()
    {
        //do reload animation
        timer = -1 * reload;
        ammo_count = 20f;
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
