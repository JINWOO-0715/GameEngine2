using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    float hAxis;
    float vAxis;
    bool wDown;
    
    Vector3 moveVec;
    // Start is called before the first frame update

    Animator anim;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        
        moveVec =new Vector3(hAxis,0,vAxis).normalized;


        if (wDown)
        {
            transform.position += moveVec * speed * 0.3f * Time.deltaTime;
            
        }
        else
        {
            transform.position += moveVec * speed *  Time.deltaTime;
        }
        
        transform.position += moveVec * speed * Time.deltaTime;
        
        anim.SetBool("isRun", moveVec !=Vector3.zero);
        anim.SetBool("isWalk", wDown);

        transform.LookAt(transform.position + moveVec);

    }
}
