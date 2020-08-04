using System;
using System.Collections;
using UnityEngine;
using Mirror;
using UnityEngine.Serialization;

namespace IntelligentCake.Combat
{
    public class WeaponManager : NetworkBehaviour
    {
        [SerializeField] private string weaponLayerName = "Weapon";

        [SerializeField] private Transform weaponHolder;
        
        [SerializeField] private PlayerWeapon primaryWeapon;

        private PlayerWeapon _currentWeapon;
        private WeaponGraphics _currentGraphics;

        public bool isReloading;
        
        private void Start()
        {
            EquipWeapon(primaryWeapon);
        }
        
        public PlayerWeapon GetCurrentWeapon()
        {
            return _currentWeapon;
        }

        public WeaponGraphics GetCurrentGraphics()
        {
            return _currentGraphics;
        }

        private void EquipWeapon(PlayerWeapon weapon)
        {
            _currentWeapon = weapon;

            GameObject weaponIns = (GameObject)Instantiate(weapon.graphics, weaponHolder.position, weaponHolder.rotation);
            weaponIns.transform.SetParent(weaponHolder);

            _currentGraphics = weaponIns.GetComponent<WeaponGraphics>();
            if (_currentGraphics == null)
            {
                Debug.LogError("No WeaponGraphics component on the weapon object: " + weaponIns.name);
            }
            
            if (isLocalPlayer)
            {
                Util.SetLayerRecursively(weaponIns, LayerMask.NameToLayer(weaponLayerName));
                
            }
        }

        public void Reload()
        {
            if (isReloading)
                return;
        
            StartCoroutine(Reload_Coroutine());
        }

        private IEnumerator Reload_Coroutine()
        {
            Debug.Log("Reloading...");
            isReloading = true;
            
            CmdOnReload();

            yield return new WaitForSeconds(_currentWeapon.reloadTime);
            
            _currentWeapon.bullets = _currentWeapon.maxBullets;
            
            isReloading = false;
        }

        [Command]
        private void CmdOnReload()
        {
            RpcOnReload();
        }

        [ClientRpc]
        private void RpcOnReload()
        {
            Animator anim = _currentGraphics.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetTrigger("reload");
            }
        }
    }
}
