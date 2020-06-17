using IntelligentCake.Combat;
using Mirror;
using UnityEngine;

namespace IntelligentCake.Player
{
    public class GunHandle : NetworkBehaviour
    {
        private const string PlayerTag = "Player";
        
        public Gun gun;
        
        [SerializeField]
        public Camera fpsCam;

        [SerializeField] private LayerMask mask;
        
        private void Start()
        {
            fpsCam = Camera.main;
            if (fpsCam == null)
            {
                Debug.LogError("No camera referenced!");
                this.enabled = false;
            }
        }

        private void Update()
        {
            if (!hasAuthority) return;

            if (gun.isAutomatic == false)
            {
                if (Input.GetButtonDown("Fire1") && Time.time >= gun.nextTimeToFire)
                {
                    gun.nextTimeToFire = Time.time + 1f / gun.fireRate;
                    Shoot();
                }
            }

            if (gun.isAutomatic)
            {
                if (Input.GetButton("Fire1") && Time.time >= gun.nextTimeToFire)
                {
                    gun.nextTimeToFire = Time.time + 1f / gun.fireRate;
                    Shoot();
                }
            }
        }

        [Client]
        private void Shoot()
        {
            RaycastHit hit;
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, gun.range, mask))
            {
                /*Debug.Log(hit.transform.name);

            Player player = hit.transform.GetComponent<Target>();
            if (player != null) target.TakeDamage(gun.damage);*/

                if (hit.collider.tag == PlayerTag)
                {
                    CmdPlayerShot(hit.collider.name);
                }
            }
        }    

        [Command]
        void CmdPlayerShot(string id)
        {
            Debug.Log(id + " has been shot.");
        }
    }
}