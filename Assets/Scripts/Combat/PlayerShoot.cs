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
        }

        private void Update()
        {
            _currentWeapon = _weaponManager.GetCurrentWeapon();
            if (!hasAuthority) return;
            
            if (PauseMenu.IsOn)
                return;
            
            if (_currentWeapon.bullets < _currentWeapon.maxBullets)
            {
                if (Input.GetButtonDown("Reload"))
                {
                    _weaponManager.Reload();
                    return;
                }
            }
            
            if (_currentWeapon.isAutomatic == false)
            {
                SemiAutomaticShoot();
            }

            if (_currentWeapon.isAutomatic)
            {
                AutomaticShoot();
            }

            if (_weaponManager.isReloading == false )
            {
                if (Input.GetKey(KeyCode.Alpha1))
                {
                    _weaponManager.CmdRequestWeaponSwitch(0);
                }
            
                if (Input.GetKey(KeyCode.Alpha2))
                {
                    _weaponManager.CmdRequestWeaponSwitch(1);
                }
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
            Animator anim = _weaponManager.GetCurrentGraphics().GetComponent<Animator>();
            anim.ResetTrigger("shoot");
            if (anim != null)
            {
                anim.enabled = false;
                anim.enabled = true;
                anim.SetTrigger("shoot");
            }
            _weaponManager.GetCurrentGraphics().muzzleFlash.Play();
            _weaponManager.GetAudioSource().PlayOneShot(_currentWeapon.shoot);
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
            if (!isLocalPlayer || _weaponManager.isReloading) { return; }

            if (_currentWeapon.bullets <= 0)
            {
                _weaponManager.Reload();
                return;
            }

            _currentWeapon.bullets--;
            
            Debug.Log("Remaining ammo: " + _currentWeapon.bullets);
            
            // We are shooting, call the OnShoot on server
            CmdOnShoot();
            
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

            if (_currentWeapon.bullets <= 0)
            {
                _weaponManager.Reload();
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