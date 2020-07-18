using System.Collections;
using Mirror;
using UnityEngine;

namespace IntelligentCake.Combat
{
    [RequireComponent(typeof(WeaponManager))]
    public class PlayerShoot : NetworkBehaviour
    {
        private const string PlayerTag = "Player";
        
        private PlayerWeapon _currentWeapon;

        [SerializeField] public Camera fpsCam;

        [SerializeField] private LayerMask mask;

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

        [Client]
        private void Shoot()
        {
            _currentWeapon.currentAmmo--;
            RaycastHit hit;
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, _currentWeapon.range, mask))
            {
                if (hit.collider.CompareTag(PlayerTag))
                {
                    CmdPlayerShot(hit.collider.name, _currentWeapon.damage);
                }
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