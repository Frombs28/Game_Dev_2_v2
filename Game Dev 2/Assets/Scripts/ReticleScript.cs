using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleScript : MonoBehaviour {
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Possessing()
    {
        anim.SetBool("Possess", true);
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            anim.SetBool("Possess", false);
        }
    }
}
