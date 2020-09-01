using System;
using UnityEngine;

namespace IntelligentCake.Combat
{
    public class WeaponPickup : MonoBehaviour
    {
        public PlayerWeapon pickUpGun;
        public float gunRespawnTime;
        public float spinSpeed = 10;
        private BoxCollider _myCollider;
        private bool _collected;
        [SerializeField] private GameObject gfx;

        void Awake()
        {
            _myCollider = GetComponent<BoxCollider>();
        }

        void FixedUpdate()
        {
            gfx.transform.Rotate(new Vector3(0, spinSpeed, 0));
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.transform.CompareTag("Player") && _collected == false)
            {
                _collected = true;
                col.GetComponent<WeaponManager>().AddWeaponToPlayersInventory(pickUpGun);
                gfx.SetActive(false);
                _myCollider.enabled = false;    
                Invoke("ResetPickUp", gunRespawnTime);
            }
        }

        void ResetPickUp()
        {
            _collected = false;
            gfx.SetActive(true);
            _myCollider.enabled = true;
        }
    }
}
        