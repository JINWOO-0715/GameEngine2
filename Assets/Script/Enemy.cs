using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;
    public Transform target;
    public BoxCollider meleeArea;
    public GameObject bullet; 
    public bool isChase;
    public bool isAttack;

    public enum Type
    {
        A,
        B,
        C
    };

    public Type enemyType;


    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;
    NavMeshAgent nav;
    Animator anim;
    
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        
        Invoke("ChaseStart",2);
    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }
    void Update()
    {
        if (nav.enabled)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }
    }
    void FreezeVelocity()
    {
        if (isChase) // 추격하고 있을때만 
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }

    }

    void Targeting()
    {
        float targetRadius = 0; 
        float targetRange = 0; // 공격 범위

        switch (enemyType)
        {
            case Type.A:
                 targetRadius = 1.5f;
                 targetRange = 3f;
                 break;
            case Type.B:
                targetRadius = 1f;
                targetRange = 12f;
                break;
            case Type.C:
                 targetRadius = 0.5f; 
                 targetRange = 25f; // 공격 범위

                break;;

        }
        
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position,
            targetRadius,
            transform.forward,
            targetRange,
            LayerMask.GetMask("Player"));

        if (rayHits.Length > 0 && !isAttack)
        {
            StartCoroutine(Attack());
        }
        

    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack",true);

        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;
        
                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;
       
                yield return new WaitForSeconds(1f);        
    
                break;
            case Type.B:
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward *20,ForceMode.Impulse);
                meleeArea.enabled = true;
                
                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;
                    
                yield return new WaitForSeconds(2f);
                break;
            case Type.C:
                yield return new WaitForSeconds(0.5f);
                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);

                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20;
                
                yield return new WaitForSeconds(2f);
                break;

        }
        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack",false);

    }
    void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
     
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee") // 맞은게 망치
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(Ondamage(reactVec ,false));

        }
        else if (other.tag == "Bullet")// 맞은게 총알
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject); 
            StartCoroutine(Ondamage(reactVec,false));
        }
    }

    public void HitByGrenade(Vector3 explosionPos) // 폭탄일경우
    {
        curHealth -= 100;
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(Ondamage(reactVec,true));
    }
    IEnumerator Ondamage(Vector3 reactVec , bool isGrenade)
    {
        mat.color=Color.red;
        yield return new WaitForSeconds(0.1f);

        if (curHealth > 0)
        {
            mat.color = Color.white;
        }
        else // 사망시 액션
        {
            mat.color=Color.gray;
            gameObject.layer = 14;
            anim.SetTrigger("doDie");
            isChase = false;
            nav.enabled = false;
            
            if (isGrenade)
            {
           
                reactVec = reactVec.normalized;
                reactVec += Vector3.up*3;
                
          
                
                rigid.freezeRotation = false;
                rigid.AddForce(reactVec*5,ForceMode.Impulse);
                rigid.AddTorque(reactVec*15,ForceMode.Impulse);
            }
            else 
            {   
           
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;
                rigid.AddForce(reactVec*5,ForceMode.Impulse);
            }
    
            
            Destroy(gameObject, 4);
        }
    }
}
