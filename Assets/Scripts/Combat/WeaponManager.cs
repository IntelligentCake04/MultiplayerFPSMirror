using System;
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
        private void Start()
        {
            EquipWeapon(primaryWeapon);
        }
        
        public PlayerWeapon GetCurrentWeapon()
        {
            return _currentWeapon;
        }

        private void EquipWeapon(PlayerWeapon weapon)
        {
            _currentWeapon = weapon;

            GameObject weaponIns = (GameObject)Instantiate(weapon.graphics, weaponHolder.position, weaponHolder.rotation);
            weaponIns.transform.SetParent(weaponHolder);
            if (isLocalPlayer)
            {
                weaponIns.layer = LayerMask.NameToLayer(weaponLayerName);
            }
        }
    }
}
