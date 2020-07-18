using UnityEngine;

namespace IntelligentCake.Combat
{
    [System.Serializable]
    public class PlayerWeapon
    {
        public string name = "USP-45";
        public int damage = 10;
        public float fireRate = 15f;
        
        public GameObject graphics;

        public int maxAmmo = 10;
        public int currentAmmo;
        public float reloadTime = 1f;
        public bool isReloading = false;

        public bool isAutomatic;
        public float range = 100f;
        public float nextTimeToFire;
    }
}