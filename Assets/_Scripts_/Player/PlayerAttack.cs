using System;
using UnityEngine;
using NaughtyAttributes;

public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform bullet;
    [SerializeField] private InputManager input;

    [Header("Ammo & Magazine Settings")]
    [SerializeField] private int magazineCount = 3; // ظرفیت هر خشاب
    [SerializeField] private int maxMagazineAmmoCount = 12;   // تیرهای داخل خشاب فعلی
    [SerializeField,ReadOnly] private int currentMagazineAmmo = 12;   // تیرهای داخل خشاب فعلی
    [SerializeField,ReadOnly] private bool isShooting;
    [SerializeField,ReadOnly] private bool isReloading;
    
    public bool HasAmmo() => currentMagazineAmmo > 0;
    
    public bool IsReloading() => isReloading;
    
    private bool CanReload() => magazineCount > 0 && currentMagazineAmmo < maxMagazineAmmoCount && isShooting == false;

    private void Start()
    {
        currentMagazineAmmo = maxMagazineAmmoCount;
    }

    private void Update()
    {
        print(CanReload());
        if (input.AttackInputIsPressed() && HasAmmo())
        {
            isShooting = true;
        }
        else if (CanReload() && !HasAmmo())
        {
            isReloading = true;
        }
        
        if (CanReload() && input.ReloadInput())
        {
            isReloading = true;
        }
    }

    // Fire() function just run on attack animation event
    private void Fire()
    {
        if (HasAmmo())
        {
            Instantiate(bullet, firePoint.position, firePoint.rotation);
        
            currentMagazineAmmo--;
        }
    }
    
    // Reload() function just run on attack animation event
    private void Reload()
    {
        magazineCount--;
        currentMagazineAmmo = maxMagazineAmmoCount;
    }

    public void AddReserveAmmo(int amount)
    {
        magazineCount += amount;
    }
    
    public void IsShootingFalse() => isShooting = false;    // IsShootingFalse() function just run on attack animation event
    public void IsReloadFalse() => isReloading = false;    // IsReloadFalse() function just run on reload animation event
} 