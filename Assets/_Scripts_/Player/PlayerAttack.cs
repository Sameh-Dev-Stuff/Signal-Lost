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
    [SerializeField] private AmmoContainer singleAmmoContainer;
    [SerializeField] private AmmoContainer burstAmmoContainer;
    [SerializeField] private AmmoContainer fullAutoAmmoContainer;
    [SerializeField] private FireMode currentAmmoType;
    [SerializeField,ReadOnly] private bool isShooting;
    [SerializeField,ReadOnly] private bool isReloading;

    [Serializable]
    private class AmmoContainer
    {
        public int magazineCount = 3; // ظرفیت هر خشاب
        public int maxMagazineAmmoCount = 12;   // تیرهای داخل خشاب فعلی
        public int currentMagazineAmmo = 12;   // تیرهای داخل خشاب فعلی
    }
    
    private AmmoContainer CurrentContainer
    {
        get
        {
            switch (currentAmmoType)
            {
                case FireMode.Single :
                {
                    return singleAmmoContainer;
                }
                case FireMode.Burst :
                {
                    return burstAmmoContainer;
                }
                case FireMode.FullAuto :
                {
                    return fullAutoAmmoContainer;
                }
                default:
                {
                    return singleAmmoContainer;
                }
            }
        }
    }
    
    public bool HasAmmo() => CurrentContainer.currentMagazineAmmo > 0;
    
    // This is just for returning correct data for PlayerAnimation class
    public float CurrentFireMode() => (float)currentAmmoType;
    
    public bool IsReloading() => isReloading;
    
    private bool CanReload() => CurrentContainer.magazineCount > 0 && CurrentContainer.currentMagazineAmmo < CurrentContainer.maxMagazineAmmoCount && isShooting == false;

    private void Start()
    {
        singleAmmoContainer.currentMagazineAmmo = singleAmmoContainer.maxMagazineAmmoCount;
        burstAmmoContainer.currentMagazineAmmo = burstAmmoContainer.maxMagazineAmmoCount;
        fullAutoAmmoContainer.currentMagazineAmmo = fullAutoAmmoContainer.maxMagazineAmmoCount;
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
        
            CurrentContainer.currentMagazineAmmo--;
        }
    }
    
    // Reload() function just run on attack animation event
    private void Reload()
    {
        CurrentContainer.magazineCount--;
        CurrentContainer.currentMagazineAmmo = CurrentContainer. maxMagazineAmmoCount;
    }

    public void AddReserveAmmo(int magazineCount, FireMode fireMode)
    {
        switch (fireMode)
        {
            case FireMode.Single:
            {
                singleAmmoContainer.magazineCount += magazineCount;
                break;
            }
            case FireMode.Burst:
            {
                burstAmmoContainer.magazineCount += magazineCount;
                break;
            }
            case FireMode.FullAuto:
            {
                fullAutoAmmoContainer.magazineCount += magazineCount;
                break;
            }
        }
    } 
    
    public void IsShootingFalse() => isShooting = false;    // IsShootingFalse() function just run on attack animation event
    public void IsReloadFalse() => isReloading = false;    // IsReloadFalse() function just run on reload animation event
} 