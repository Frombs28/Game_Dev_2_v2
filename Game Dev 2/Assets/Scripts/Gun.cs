using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public int damage = 1;
    public Camera cam;
    public GameObject bullet;
    public GameObject barrel;
    public float bullet_speed = 0.1f;
    float start_time = 0f;
    float burst_rate = 60f;
    int burst_num = 4;
    int i;
    public float short_range;

    void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        barrel = transform.GetChild(0).gameObject;
    }

    void Update()
    {

    }

    private void FireRifleGun()
    {
        
        GameObject cur_bullet;
        Vector3 myDirection;
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
        {
            myDirection = hit.point - barrel.transform.position;
        }
        else
        {
            myDirection = cam.transform.forward;
        }
        cur_bullet = Instantiate(bullet, barrel.transform.position, barrel.transform.rotation);
        //cur_bullet.GetComponent<Rigidbody>().velocity = cam.transform.TransformDirection(Vector3.forward * bullet_speed);
        cur_bullet.GetComponent<Rigidbody>().velocity = myDirection.normalized * bullet_speed;
        cur_bullet.layer = 9;
        Destroy(cur_bullet, 5);
    }

    private void FireShortGun()
    {

        GameObject cur_bullet;
        Vector3 myDirection;
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
        {
            myDirection = hit.point - barrel.transform.position;
        }
        else
        {
            myDirection = cam.transform.forward;
        }
        cur_bullet = Instantiate(bullet, barrel.transform.position, barrel.transform.rotation);
        //cur_bullet.GetComponent<Rigidbody>().velocity = cam.transform.TransformDirection(Vector3.forward * bullet_speed);
        cur_bullet.GetComponent<Rigidbody>().velocity = myDirection.normalized * bullet_speed;
        cur_bullet.layer = 9;
        Destroy(cur_bullet, 0.5f);
        
        for (float i = 0f; i < (2 * Mathf.PI); i+=(Mathf.PI / 4))
        {
            GameObject cur_bullet2;
            Vector3 myDirection2;
            //RaycastHit hit2;
            Vector3 targetTransform;
            /*
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit2))
            {
                targetTransform = new Vector3(hit2.point.x + 3 * Mathf.Cos(i), hit2.point.y + 3 * Mathf.Sin(i), hit2.point.z);
                myDirection2 = targetTransform - barrel.transform.position;
            }
            else
            {
            */
                targetTransform = new Vector3(cam.transform.forward.x + 2 * Mathf.Cos(i), cam.transform.forward.y + 2 * Mathf.Sin(i), cam.transform.forward.z);
                myDirection2 = targetTransform;
            //}
            cur_bullet2 = Instantiate(bullet, barrel.transform.position, barrel.transform.rotation);
            //cur_bullet.GetComponent<Rigidbody>().velocity = cam.transform.TransformDirection(Vector3.forward * bullet_speed);
            cur_bullet2.GetComponent<Rigidbody>().velocity = myDirection2.normalized * bullet_speed;
            cur_bullet2.layer = 9;
            Destroy(cur_bullet2, 0.5f);
        }
        
    }

    private void FireEnemyGun()
    {
        StartCoroutine("FireBurst");
        i = 0;
    }

    IEnumerator FireBurst()
    {
        for(i = 0; i < burst_num; i++)
        {
            GameObject cur_bullet;
            cur_bullet = Instantiate(bullet, barrel.transform.position, barrel.transform.rotation);
            cur_bullet.GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.forward * (bullet_speed * 0.5f));
            Destroy(cur_bullet, 3);
            yield return new WaitForSeconds(burst_rate);
        }

    }
}
