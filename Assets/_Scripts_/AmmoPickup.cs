using System;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Serialization;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField] private int ammo;
    [SerializeField] private FireMode fireMode;
    [SerializeField, Tag] private string pickupTag;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(pickupTag)) return;
        
        

        PlayerAttack player = other.gameObject.GetComponent<PlayerAttack>();
        
        player.AddReserveAmmo(ammo,fireMode);
        Destroy(gameObject);
    }
}