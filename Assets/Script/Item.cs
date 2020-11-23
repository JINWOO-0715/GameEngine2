using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class item : MonoBehaviour
{
    // Start is called before the first frame update

    public enum Type
    {
        Ammo,
        Coin,
        Grenade,
        Heart,
        Weapon
    };

    public Type type;
    public int value;

    void Update()
    {
        transform.Rotate(Vector3.up * 20 *Time.deltaTime);
    }
}
