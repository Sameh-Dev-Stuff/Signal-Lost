using System;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Serialization;

public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Bullet bullet;
    [SerializeField] private InputManager input;

    [Header("Ammo & Magazine Settings")]
    [SerializeField] private float damage;
    [SerializeField] private AmmoContainer singleAmmoContainer;
    [SerializeField] private AmmoContainer burstAmmoContainer;
    [SerializeField] private AmmoContainer fullAutoAmmoContainer;
    [SerializeField] private FireMode currentFireMode; 
    private bool _isShooting;
    private bool _isReloading;

    [Serializable]
    private class AmmoContainer
    {
        public int magazineCount = 3;            // Number of magazines
        public int maxMagazineAmmoCount = 12;   // Maximum ammo per magazine
        public int currentMagazineAmmo = 12;   // Current ammo in the magazine
        public float damage = 1;              // Damage per shot
    }
    
    private AmmoContainer CurrentContainer
    {
        get
        {
            // Returns the ammo container for the current fire mode.
            // Each fire mode has its own magazine count, ammo count, and damage.
            switch (currentFireMode)
            {
                case FireMode.Single:
                {
                    return singleAmmoContainer;
                }
                case FireMode.Burst:
                {
                    return burstAmmoContainer;
                }
                case FireMode.FullAuto:
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
    public float CurrentFireMode() => (float)currentFireMode;
    
    public bool IsReloading() => _isReloading;
    
    private bool CanReload() => CurrentContainer.magazineCount > 0 && CurrentContainer.currentMagazineAmmo < CurrentContainer.maxMagazineAmmoCount && _isShooting == false;

    private void Start()
    {
        singleAmmoContainer.currentMagazineAmmo = singleAmmoContainer.maxMagazineAmmoCount;
        burstAmmoContainer.currentMagazineAmmo = burstAmmoContainer.maxMagazineAmmoCount;
        fullAutoAmmoContainer.currentMagazineAmmo = fullAutoAmmoContainer.maxMagazineAmmoCount;
    }

    private void Update()
    {
        // Check if the player is trying to shoot and has ammo.
        // _isShooting is used to prevent reloading while the player is shooting.
        if (input.AttackInputIsPressed() && HasAmmo())
        {
            _isShooting = true;
        }
        // Automatically start reloading when the magazine is empty.
        else if (CanReload() && !HasAmmo())
        {
            _isReloading = true;
        }

        // Start reloading when the player manually presses the reload button.
        // CanReload() makes sure there is a magazine available and the current
        // magazine is not already full.
        if (CanReload() && input.ReloadInput())
        {
            _isReloading = true;
        }
    }

    // Fire() function just run on attack animation event
    private void Fire()
    {
        if (HasAmmo())
        {
            Bullet spawnedBullet = Instantiate(bullet, firePoint.position, firePoint.rotation);
            
            spawnedBullet.SetDamageInfo(CurrentContainer.damage, transform);

            CurrentContainer.currentMagazineAmmo--;
        }
    }
    
    // Reload() function just run on attack animation event
    private void Reload()
    {
        CurrentContainer.magazineCount--;
        CurrentContainer.currentMagazineAmmo = CurrentContainer.maxMagazineAmmoCount;
    }

    // Ammo pickup logic
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
    
    public void IsShootingFalse() => _isShooting = false;    // IsShootingFalse() function just run on attack animation event
    public void IsReloadFalse() => _isReloading = false;    // IsReloadFalse() function just run on reload animation event
} 