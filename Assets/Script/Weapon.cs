using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] int damage;

    Collider coll;

    private void Awake()
    {
        coll = GetComponent<Collider>();
    }
    public void EnableWeapon()
    {
        coll.enabled = true;
    }

    public void DisableWeapon()
    {
        coll.enabled=false;
    }

    private void OnTriggerEnter(Collider other)
    {
        IHittable hittable = other.GetComponent<IHittable>();
        hittable?.TakeHit(damage);
    }
}
