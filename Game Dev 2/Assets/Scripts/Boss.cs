using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{

    public GameObject door;
    public Transform spot;
    int health = 80;
    public Slider health_bar;

    // Use this for initialization
    void Start()
    {
        health_bar.maxValue = health;
        health_bar.value = health;
        door = GameObject.Find("door2");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.tag == "Projectile")
        {
            if (collider.gameObject.layer == 9)
            {
                health -= collider.gameObject.GetComponent<Bullet>().GetDamage();
                health_bar.value = health;
                if(health <= 0)
                {
                    Destroy(door);
                }
            }
        }
    }
}
