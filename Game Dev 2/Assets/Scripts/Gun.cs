using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public int damage = 1;
    public Camera cam;
    public GameObject bullet;
    public GameObject barrel;
    public float rifle_bullet_speed = 150f;
    public float short_bullet_speed = 100f;
    public float sniper_bullet_speed = 200f;
    public float grunt_bullet_speed = 100f;
    public int rifle_damage = 2;
    public int short_damage = 4;
    public int sniper_damage = 8;
    public int grunt_damage = 2;
    float start_time = 0f;
    public float grunt_burst_rate = 0.2f;
    public float melee_burst_rate = 0.15f;
    public float ranged_burst_rate = 1.15f;
    public float sniper_burst_rate = 2f;
    public int grunt_burst_num = 5;
    public int melee_burst_num = 10;
    public int ranged_burst_num = 3;
    public int sniper_burst_num = 2;
    public float gruntInaccuracy = 10f;
    public float meleeInaccuracy = 20f;
    public float rangedInacuracy = 10f;
    public float sniperInaccuracy = 3f;
    public float gruntRecoil = 4f;
    public float meleeRecoil = 2f;
    public float rangedRecoil = 6f;
    public float sniperRecoil = 8f;
    int i;
    public float short_range;
    AudioSource laser;
    float pitch;

    void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        barrel = transform.GetChild(0).gameObject;
        laser = gameObject.GetComponent<AudioSource>();
        pitch = 1;
    }

    void Update()
    {

    }

    private void FireGruntGun()
    {

        GameObject cur_bullet;
        Vector3 myDirection;
        RaycastHit hit;
        int layerMask = 1 << 2;
        layerMask = ~layerMask; //the raycast will ignore anything on this layer
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit/*, layerMask*/))
        {
            myDirection = hit.point - barrel.transform.position;
        }
        else
        {
            myDirection = cam.transform.forward;
        }
        cur_bullet = Instantiate(bullet, barrel.transform.position, barrel.transform.rotation);
        //cur_bullet.GetComponent<Rigidbody>().velocity = cam.transform.TransformDirection(Vector3.forward * bullet_speed);
        cur_bullet.GetComponent<Rigidbody>().velocity = myDirection.normalized * rifle_bullet_speed;
        cur_bullet.layer = 9;
        cur_bullet.gameObject.GetComponent<Bullet>().SetDamage(grunt_damage);
        Destroy(cur_bullet, 5);
        laser.pitch = Random.Range(1.0f, 1.5f);
        laser.Play();
        cam.SendMessage("Recoil", gruntRecoil);
    }

    private void FireRifleGun()
    {
        
        GameObject cur_bullet;
        Vector3 myDirection;
        RaycastHit hit;
        int layerMask = 1 << 2;
        layerMask = ~layerMask; //the raycast will ignore anything on this layer
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit/*, layerMask*/))
        {
            myDirection = hit.point - barrel.transform.position;
        }
        else
        {
            myDirection = cam.transform.forward;
        }
        cur_bullet = Instantiate(bullet, barrel.transform.position, barrel.transform.rotation);
        //cur_bullet.GetComponent<Rigidbody>().velocity = cam.transform.TransformDirection(Vector3.forward * bullet_speed);
        cur_bullet.GetComponent<Rigidbody>().velocity = myDirection.normalized * rifle_bullet_speed;
        cur_bullet.layer = 9;
        cur_bullet.gameObject.GetComponent<Bullet>().SetDamage(rifle_damage);
        Destroy(cur_bullet, 5);
        laser.pitch = Random.Range(1.0f, 1.5f);
        laser.Play();
        cam.SendMessage("Recoil", meleeRecoil);
    }

    private void FireShortGun()
    {

        GameObject cur_bullet;
        Vector3 myDirection;
        RaycastHit hit;
        int layerMask = 1 << 2;
        layerMask = ~layerMask; //the raycast will ignore anything on this layer
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit/*, layerMask*/))
        {
            myDirection = hit.point - barrel.transform.position;
        }
        else
        {
            myDirection = cam.transform.forward;
        }
        cur_bullet = Instantiate(bullet, barrel.transform.position, barrel.transform.rotation);
        //cur_bullet.GetComponent<Rigidbody>().velocity = cam.transform.TransformDirection(Vector3.forward * bullet_speed);
        cur_bullet.GetComponent<Rigidbody>().velocity = myDirection.normalized * short_bullet_speed;
        cur_bullet.layer = 9;
        cur_bullet.gameObject.GetComponent<Bullet>().SetDamage(short_damage);
        Destroy(cur_bullet, 0.25f);
        
        for (float i = 0f; i < (2 * Mathf.PI); i+=(Mathf.PI / 4))
        {
            GameObject cur_bullet2;
            Vector3 myDirection2;
            RaycastHit hit2;
            Vector3 targetTransform;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit2))
            {
                targetTransform = new Vector3(hit2.point.x + 3 * Mathf.Cos(i), hit2.point.y + 3 * Mathf.Sin(i), hit2.point.z);
                myDirection2 = targetTransform - barrel.transform.position;
            }
            else
            {
                targetTransform = new Vector3(cam.transform.forward.x + 2 * Mathf.Cos(i), cam.transform.forward.y + 2 * Mathf.Sin(i), (transform.InverseTransformDirection(Vector3.forward).z));
                myDirection2 = targetTransform;
            }
            cur_bullet2 = Instantiate(bullet, barrel.transform.position, barrel.transform.rotation);
            //cur_bullet.GetComponent<Rigidbody>().velocity = cam.transform.TransformDirection(Vector3.forward * bullet_speed);
            cur_bullet2.GetComponent<Rigidbody>().velocity = myDirection2.normalized * short_bullet_speed;
            cur_bullet2.layer = 9;
            cur_bullet2.gameObject.GetComponent<Bullet>().SetDamage(short_damage);
            Destroy(cur_bullet2, 0.25f);
        }
        cam.SendMessage("Recoil", rangedRecoil);
    }
    
    private void FireSniperGun()
    {

        GameObject cur_bullet;
        Vector3 myDirection;
        RaycastHit hit;
        int layerMask = 1 << 2;
        layerMask = ~layerMask; //the raycast will ignore anything on this layer
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit/*, layerMask*/))
        {
            myDirection = hit.point - barrel.transform.position;
        }
        else
        {
            myDirection = cam.transform.forward;
        }
        cur_bullet = Instantiate(bullet, barrel.transform.position, barrel.transform.rotation);
        cur_bullet.GetComponent<Rigidbody>().velocity = myDirection.normalized * sniper_bullet_speed;
        cur_bullet.layer = 9;
        cur_bullet.gameObject.GetComponent<Bullet>().SetDamage(sniper_damage);
        Destroy(cur_bullet, 7);
        laser.pitch = Random.Range(1.0f, 1.5f);
        laser.Play();
        cam.SendMessage("Recoil", sniperRecoil);
    }

    private void FireEnemyGun(string characterType)
    {
        if (characterType == "grunt")
        //fire a short burst
        {
            StartCoroutine("FireEnemyGrunt");
        }
        else if (characterType == "melee")
        {
            StartCoroutine("FireEnemyMelee");
        }
        else if (characterType == "ranged")
        {
            StartCoroutine("FireEnemyRanged");
        }
        else
        {
            StartCoroutine("FireEnemySniper");
        }
    }

    IEnumerator FireEnemyGrunt()
    {
        for(i = 0; i < grunt_burst_num; i++)
        {
            GameObject cur_bullet;
            cur_bullet = Instantiate(bullet, barrel.transform.position, barrel.transform.rotation);
            Vector3 myDirection = new Vector3(Random.Range(-gruntInaccuracy, gruntInaccuracy), Random.Range(-gruntInaccuracy, gruntInaccuracy), 100f);
            myDirection.Normalize();
            cur_bullet.GetComponent<Rigidbody>().velocity = transform.TransformDirection(myDirection * (rifle_bullet_speed * 0.5f));
            cur_bullet.gameObject.GetComponent<Bullet>().SetDamage(grunt_damage);
            Destroy(cur_bullet, 3);
            yield return new WaitForSeconds(grunt_burst_rate);
        }
    }

    IEnumerator FireEnemyMelee()
    {
        for (i = 0; i < melee_burst_num; i++)
        {
            GameObject cur_bullet;
            cur_bullet = Instantiate(bullet, barrel.transform.position, barrel.transform.rotation);
            Vector3 myDirection = new Vector3(Random.Range(-meleeInaccuracy, meleeInaccuracy), Random.Range(-meleeInaccuracy, meleeInaccuracy), 100f);
            myDirection.Normalize();
            cur_bullet.GetComponent<Rigidbody>().velocity = transform.TransformDirection(myDirection * (rifle_bullet_speed * 0.5f));
            cur_bullet.gameObject.GetComponent<Bullet>().SetDamage(rifle_damage);
            Destroy(cur_bullet, 3);
            yield return new WaitForSeconds(melee_burst_rate);
        }
    }

    IEnumerator FireEnemyRanged()
    {
        for (i = 0; i < ranged_burst_num; i++)
        {
            GameObject cur_bullet;
            cur_bullet = Instantiate(bullet, barrel.transform.position, barrel.transform.rotation);
            Vector3 myDirection = new Vector3(Random.Range(-rangedInacuracy, rangedInacuracy), Random.Range(-rangedInacuracy, rangedInacuracy), 100f);
            myDirection.Normalize();
            cur_bullet.GetComponent<Rigidbody>().velocity = transform.TransformDirection(myDirection * (short_bullet_speed * 0.5f));
            cur_bullet.gameObject.GetComponent<Bullet>().SetDamage(short_damage);
            Destroy(cur_bullet, 3);
            yield return new WaitForSeconds(ranged_burst_rate);
        }
    }

    IEnumerator FireEnemySniper()
    {
        for (i = 0; i < sniper_burst_num; i++)
        {
            GameObject cur_bullet;
            cur_bullet = Instantiate(bullet, barrel.transform.position, barrel.transform.rotation);
            Vector3 myDirection = new Vector3(Random.Range(-sniperInaccuracy, sniperInaccuracy), Random.Range(-sniperInaccuracy, sniperInaccuracy), 100f);
            myDirection.Normalize();
            cur_bullet.GetComponent<Rigidbody>().velocity = transform.TransformDirection(myDirection * (sniper_bullet_speed * 0.5f));
            cur_bullet.gameObject.GetComponent<Bullet>().SetDamage(sniper_damage);
            Destroy(cur_bullet, 3);
            yield return new WaitForSeconds(sniper_burst_rate);
        }
    }
}
