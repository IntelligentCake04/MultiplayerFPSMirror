using UnityEngine;

namespace IntelligentCake.Combat
{
    public class Gun : MonoBehaviour
    {
        public RaycastHit hit;
    
        public float damage = 10f;
        public float fireRate = 15;
        public Camera fpsCam;

        public bool isAutomatic;
        public float range = 100f;
        public float nextTimeToFire;
    
        private void Start()
        {
            fpsCam = Camera.main;
        }
    }
}