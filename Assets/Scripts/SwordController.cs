using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    [HideInInspector] public Collider swordCollider;
    [HideInInspector] public float damage;

    private void OnEnable ()
    {
        swordCollider = GetComponent<Collider>();
        swordCollider.isTrigger = true;
        swordCollider.enabled = false;

    }
    private void OnTriggerEnter (Collider other)
    {
        if (other.CompareTag ("Castle"))
        {
            Debug.Log (damage);
            Debug.Log (other);
            EventManager.OnCastleHit (damage);
            swordCollider.enabled = false;
        }
    }    
}
