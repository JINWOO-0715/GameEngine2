﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    
    float hAxis;
    float vAxis;
    
    bool wDown;
    bool jDown;
    bool iDown;

    bool sDown1;
    bool sDown2;
    bool sDown3;
    
    bool isJump;
    bool isDodge;
    
    Vector3 moveVec;
    // Start is called before the first frame update
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    GameObject nearObject;
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Dodge();
        Interation();
        Swap();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        iDown = Input.GetButtonDown("Interation");
        
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3 ");
    }

    void Move()
    {
        moveVec =new Vector3(hAxis,0,vAxis).normalized;

        if(isDodge)
            moveVec = dodgeVec;
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
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }

    void Jump()
    {
        if(jDown && moveVec == Vector3.zero && !isJump && !isDodge){
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }
    void Dodge()
    {
        if(jDown && moveVec != Vector3.zero && !isJump && !isDodge){
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.4f);
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }

    void Swap()
    {
        if(sDown1)
    }
    void Interation()
    {
       // 점프중 무기 스왑 바꾸려면 isjimp랑 dodge바꾸기
            if (iDown && nearObject != null && !isJump && !isDodge)
            {
                if (nearObject.tag == "Weapon")
                {
                    Item item = nearObject.GetComponent<Item>();
                    int weaponIndex = item.value;
                    hasWeapons[weaponIndex] = true;
                    


                    Destroy(nearObject);
                }
                
            }
       
    }
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor"){
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

     void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
        {
            nearObject = other.gameObject;
        }
       
    }
     void OnTriggerExit(Collider other)
     {
         if (other.tag == "Weapon")
         {
             nearObject = null;
         }
       
     }
}
