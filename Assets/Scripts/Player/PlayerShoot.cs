using IntelligentCake.Combat;
using Mirror;
using UnityEngine;
using System.Collections;

namespace IntelligentCake.Player
{
    public class PlayerShoot : NetworkBehaviour
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

            gun.currentAmmo = gun.maxAmmo;
        }

        private void Update()
        {
            if (!hasAuthority) return;
            
            if(gun.isReloading)
                return;
            
            if (gun.currentAmmo <= 0)
            {
                StartCoroutine(Reload());
                return;
            }
            
            if (gun.isAutomatic == false)
            {
                SemiAutomaticShoot();
            }

            if (gun.isAutomatic)
            {
                AutomaticShoot();
            }
        }

        private void SemiAutomaticShoot()
        {
            if (Input.GetButtonDown("Fire1") && Time.time >= gun.nextTimeToFire)
            {
                gun.nextTimeToFire = Time.time + 1f / gun.fireRate;
                Shoot();
            }
        }
        private void AutomaticShoot()
        {
            if (Input.GetButton("Fire1") && Time.time >= gun.nextTimeToFire)
            {
                gun.nextTimeToFire = Time.time + 1f / gun.fireRate;
                Shoot();
            }
        }

        IEnumerator Reload()
        {
            gun.isReloading = true;
            Debug.Log("Reloading");
            
            yield return new WaitForSeconds(gun.reloadTime);
            
            gun.currentAmmo = gun.maxAmmo;
            gun.isReloading = false;
        }

        [Client]
        private void Shoot()
        {
            gun.currentAmmo--;
            RaycastHit hit;
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, gun.range, mask))
            {
                if (hit.collider.CompareTag(PlayerTag))
                {
                    CmdPlayerShot(hit.collider.name, gun.damage);
                }
            }
        }    

        [Command]
        void CmdPlayerShot(string playerId, int damage)
        {
            Debug.Log(playerId + " has been shot.");

            Player player = GameManager.GetPlayer(playerId);
            player.RpcTakeDamage(damage);
        }
    }
}