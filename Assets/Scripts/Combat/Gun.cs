using UnityEngine;

namespace IntelligentCake.Combat
{
    [System.Serializable]
    public class Gun
    {
        public int damage = 10;
        public float fireRate = 15f;

        public bool isAutomatic;
        public float range = 100f;
        public float nextTimeToFire;
    }
}