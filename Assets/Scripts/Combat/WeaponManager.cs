using System.Collections;
using Mirror;
using UnityEngine;

namespace IntelligentCake.Combat
{
    public class WeaponManager : NetworkBehaviour

    {
        [SerializeField] private string weaponLayerName = "Weapon";
        [SerializeField] private Transform weaponHolder;
        private PlayerWeapon _currentWeapon;
        [SerializeField] private PlayerWeapon[] weapons;
        private GameObject[] _instanceWeapons;
        private WeaponGraphics _currentGraphics;

        public bool isReloading;

        private void Start()

        {
            _instanceWeapons = new GameObject[weapons.Length];

            CmdSetupWeapons();

            //On Spawn, equip primary weapon

            CmdEquipWeapon(0);
        }

        [Command]
        public void CmdSetupWeapons()
        {
            RpcSetupWeapons();
        }

        [ClientRpc]
        public void RpcSetupWeapons()
        {
            for (int i = 0; i < _instanceWeapons.Length; i++)

            {
                _instanceWeapons[i] = Instantiate(weapons[i].graphics, weaponHolder.position, weaponHolder.rotation);

                _instanceWeapons[i].transform.SetParent(weaponHolder);
            }
        }

        public PlayerWeapon GetCurrentWeapon()

        {
            return _currentWeapon;
        }

        public WeaponGraphics GetCurrentGraphics()

        {
            return _currentGraphics;
        }

        private void Update()
        {
            if (isReloading) return;
            
            if (Input.GetKey(KeyCode.Alpha1))
            {
                ChangeWeapon(0);
            }
            else if (Input.GetKey(KeyCode.Alpha2))
            {
                ChangeWeapon(1);
            }
        }
        public void ChangeWeapon(int newWeapon)

        {
            CmdEquipWeapon(newWeapon);
        }

        [Command]
        void CmdEquipWeapon(int index)
        {
            RpcEquipWeapon(index);
        }

        [ClientRpc]
        void RpcEquipWeapon(int index)

        {
            _currentWeapon = weapons[index];
            
            //Enable newWeapon, disable all others
            for (int i = 0; i < weapons.Length; i++)

            {
                if (i == index)
                {
                    _instanceWeapons[i].gameObject.SetActive(true);
                }
                else
                {
                    _instanceWeapons[i].gameObject.SetActive(false);
                }
            }

            Debug.Log(_currentWeapon.name + " has been activated. ");
            _currentGraphics = _instanceWeapons[index].GetComponent<WeaponGraphics>();
            if (_currentGraphics == null)

            {
                Debug.LogError("No WeaponGFX component on the weapon: " + _instanceWeapons[index].name);
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