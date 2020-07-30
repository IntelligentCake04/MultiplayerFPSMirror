using System.Collections;
using Mirror;
using UnityEngine;

namespace IntelligentCake.Combat
{
    [RequireComponent(typeof(WeaponManager))]
    public class PlayerShoot : NetworkBehaviour
    {
        private const string PlayerTag = "Player";
        
        [SerializeField] public Camera fpsCam;

        [SerializeField] private LayerMask mask;

        private PlayerWeapon _currentWeapon;
        private WeaponManager _weaponManager;
        
        private void Start()
        {
            if (fpsCam == null)
            {
                Debug.LogError("No camera referenced!");
                this.enabled = false;
            }

            _weaponManager = GetComponent<WeaponManager>();
            _currentWeapon.currentAmmo = _currentWeapon.maxAmmo;
        }

        private void Update()
        {
            _currentWeapon = _weaponManager.GetCurrentWeapon();
            if (!hasAuthority) return;
            
            if (PauseMenu.IsOn)
                return;
            
            if(_currentWeapon.isReloading)
                return;
            
            if (_currentWeapon.currentAmmo <= 0)
            {
                StartCoroutine(Reload());
                return;
            }
            
            if (_currentWeapon.isAutomatic == false)
            {
                SemiAutomaticShoot();
            }

            if (_currentWeapon.isAutomatic)
            {
                AutomaticShoot();
            }
            
        }
        
        private void SemiAutomaticShoot()
        {
            if (Input.GetButtonDown("Fire1") && Time.time >= _currentWeapon.nextTimeToFire)
            {
                _currentWeapon.nextTimeToFire = Time.time + 1f / _currentWeapon.fireRate;
                Shoot();
            }
        }
        private void AutomaticShoot()
        {
            if (Input.GetButton("Fire1") && Time.time >= _currentWeapon.nextTimeToFire)
            {
                _currentWeapon.nextTimeToFire = Time.time + 1f / _currentWeapon.fireRate;
                Shoot();
            }
        }

        private IEnumerator Reload()
        {
            _currentWeapon.isReloading = true;
            Debug.Log("Reloading");
            
            yield return new WaitForSeconds(_currentWeapon.reloadTime);
            
            _currentWeapon.currentAmmo = _currentWeapon.maxAmmo;
            _currentWeapon.isReloading = false;
        }

        // Is called on server when player shoots
        [Command]
        void CmdOnShoot()
        {
            RpcDoShootEffect();
        }

        // Is called on clients when we need to 
        // do a shoot effect
        [ClientRpc]
        void RpcDoShootEffect()
        {
            _weaponManager.GetCurrentGraphics().muzzleFlash.Play();
        }

        // Is called on the server when we hit something
        // Takes in the pos and normal of the surface
        [Command]
        void CmdOnHit(Vector3 pos, Vector3 normal)
        {
            RpcDoHitEffect(pos, normal);
        }
        
        // Is called on all clients
        // Here we can spawn in cool effects
        [ClientRpc]
        void RpcDoHitEffect(Vector3 pos, Vector3 normal)
        {
            LineRenderer trail = _weaponManager.GetCurrentGraphics().bulletTrail;
            trail.enabled = true;
            trail.SetPosition(0, _weaponManager.GetCurrentGraphics().muzzleFlash.transform.position);
            trail.SetPosition(1, pos);
            GameObject hitEffect = (GameObject)Instantiate(_weaponManager.GetCurrentGraphics().hitEffectPrefab, pos, Quaternion.LookRotation(normal));
            StartCoroutine(DestroyLine(trail));
            Destroy(hitEffect, 2f);
        }

        IEnumerator DestroyLine(LineRenderer trail)
        {
            yield return 0;
            trail.enabled = false;
        }
        
        [Client]
        private void Shoot()
        {
            if (!isLocalPlayer) { return; }
            
            // We are shooting, call the OnShoot on server
            CmdOnShoot();

            _currentWeapon.currentAmmo--;
            RaycastHit hit;
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, _currentWeapon.range, mask))
            {
                if (hit.collider.CompareTag(PlayerTag))
                {
                    CmdPlayerShot(hit.collider.name, _currentWeapon.damage);
                }
                // We hit something, call the CmdOnHit method on the server
                CmdOnHit(hit.point, hit.normal);
            }
        }    

        [Command]
        void CmdPlayerShot(string playerId, int damage)
        {
            Debug.Log(playerId + " has been shot.");
            Player.Player player = GameManager.GetPlayer(playerId);
            player.RpcTakeDamage(damage);
        }
    }
}