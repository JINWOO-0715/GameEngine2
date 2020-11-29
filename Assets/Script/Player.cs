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
    public GameObject[] grenades;
    public int hasGrenades;
    public Camera followCamera;
    
    
    public int ammo;
    public int coin;
    public int health;

    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;

    float hAxis;
    float vAxis;
    
    bool wDown;
    bool jDown;
    bool iDown;
    bool fDown;
    bool rDwon;

    bool sDown1;
    bool sDown2;
    bool sDown3;
    
    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isFireReady = true;
    bool isReload;

    Vector3 moveVec;
    // Start is called before the first frame update
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    GameObject nearObject;
    Weapon equipWeapon;
    int equipWeaponIndex =-1;
    float fireDelay;

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
        Attack();
        Reload();
        Swap();
        Dodge();
        Interation();
       
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        iDown = Input.GetButtonDown("Interation");
        fDown = Input.GetButton("Fire1");
        rDwon = Input.GetButtonDown("Reload");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }

    void Move()
    {
        moveVec =new Vector3(hAxis,0,vAxis).normalized;

        if(isDodge)
            moveVec = dodgeVec;

        /*무기 바꿀때 움직이지 않게하기
        if (isSwap)
            moveVec = Vector3.zero;*/
        
        // // 움직이는 중 근접공격 불가
        if (!isFireReady)
            moveVec = Vector3.zero;

        // 움직이는 동안 리로드 불가
        if(isReload)
            moveVec = Vector3.zero;

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
        // 키보드에 의한 회전
        transform.LookAt(transform.position + moveVec);

        // 마우스에 의한 회전
        if(fDown){
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if(Physics.Raycast(ray, out rayHit, 100)) {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }
    }

    void Jump()
    {
        if(jDown && moveVec == Vector3.zero && !isJump && !isDodge)// &&!isSwap 점프도 안되게만들기
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Attack()
    {
        if(equipWeapon == null)
            return;
        
        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if(fDown && isFireReady && !isDodge && !isSwap && !isReload) {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
        }
    }

    void Reload()
    {
        if(equipWeapon == null)
            return;
        
        if(equipWeapon.type == Weapon.Type.Melee)
            return;
        
        if(ammo == 0)
            return;
        
        if(rDwon && !isJump && !isDodge && !isSwap && isFireReady) {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 2.0f);
        }
    }

    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = equipWeapon.maxAmmo;
        ammo -= reAmmo;
        isReload = false;
    }

    void Dodge()
    {
        if(jDown && moveVec != Vector3.zero && !isJump && !isDodge) // &&!isSwap 점프도 안되게만들기
        {
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
    void SwapOut()
    {
        isSwap = false;
    }
    void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;

       
        int weaponIndex = -1;

        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;
        // 점프중이나 회피중일때 바꿀꺼면 isJump isDodge 없애기
        if ((sDown1 || sDown2 || sDown3) && !isJump && !isDodge)
        {
            if(equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);
            
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);
            
            anim.SetTrigger("doSwap");
            
            isSwap = true;
            
            Invoke("SwapOut", 0.5f);
        }

    
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

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch(item.type){
                case Item.Type.Ammo:
                    ammo += item.value;
                    if(ammo > maxAmmo)
                        ammo = maxAmmo;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if(coin > maxCoin)
                        coin = maxCoin;
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if(health > maxHealth)
                        health = maxHealth;
                    break;
                case Item.Type.Grenade:
                grenades[hasGrenades].SetActive(true);
                    hasGrenades += item.value;
                    if(hasGrenades > maxHasGrenades)
                        hasGrenades = maxHasGrenades;
                    break;
            }
            Destroy(other.gameObject);
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
