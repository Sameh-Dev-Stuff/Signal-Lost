using System;
using UnityEngine;
using NaughtyAttributes;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField, Tag] private string pickupTag;
    [SerializeField] private int ammo;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(pickupTag)) return;
        
        

        PlayerAttack player = other.gameObject.GetComponent<PlayerAttack>();
        
        player.AddReserveAmmo(ammo);
        Destroy(gameObject);
    }
}