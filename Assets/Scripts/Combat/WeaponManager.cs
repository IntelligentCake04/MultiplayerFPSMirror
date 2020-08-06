using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace IntelligentCake.Combat
{
    public class WeaponManager : NetworkBehaviour
    {
        [SerializeField]
        private string weaponLayerName = "Weapon";

        [SerializeField] private Transform weaponHolder;

        [SerializeField] private PlayerWeapon[] weapons;
        
        private WeaponGraphics _currentGraphics;
        private PlayerWeapon _currentWeapon;
        private int _currentWeaponSlot = -1;
        private GameObject _weaponIns;
        private readonly List<PlayerWeapon> inventory = new List<PlayerWeapon>();

        private AudioSource _audioSource;

        public bool isReloading = false;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            //When we spawn the character, give the player weapons.
            foreach (var weapon in weapons)
            {
                AddWeaponToPlayersInventory(weapon);
            }
        }

        //Returns the current weapon.
        public PlayerWeapon GetCurrentWeapon()
        {
            return _currentWeapon;
        }

        //Returns the current weapon graphics.
        public WeaponGraphics GetCurrentGraphics()
        {
            return _currentGraphics;
        }

        public AudioSource GetAudioSource()
        {
            return _audioSource;
        }

        //Add weapon to backend inventory.
        private void AddWeaponToPlayersInventory(PlayerWeapon weapon)
        {
            foreach (var gun in inventory)
            {
                //Does the gun already exist in inventory?
                if (gun.name == weapon.name)
                {
                    // Add Some Ammo
                    return;
                }
            }
            inventory.Add(weapon);
            InstantiateNewWeapon(weapon);
        }

        //Instantiate a new weapon in the players hands when its picked up
        private void InstantiateNewWeapon(PlayerWeapon weapon)
        {
            //Remove the current weapon gfx
            if (_weaponIns != null) weaponHolder.transform.GetChild(_currentWeaponSlot).gameObject.SetActive(false);
            
            //Assign the current weapon
            _currentWeapon = weapon;
            
            //Instantiate the weapon graphics associated with the current weapon class
            _weaponIns = Instantiate(weapon.graphics, weaponHolder.position, weaponHolder.rotation);
            _weaponIns.transform.SetParent(weaponHolder);

            //Ensure that there is some graphics assigned!
            _currentGraphics = _weaponIns.GetComponent<WeaponGraphics>();
            if (_currentGraphics == null) Debug.Log("No Weapon Graphics Component On the Object : " + _weaponIns.name);
            _currentWeaponSlot += 1;
            
            //Set the layer mask if this is the local player, so the weapon camera can render it.
            if (isLocalPlayer)
            {
                Util.SetLayerRecursively(_weaponIns, LayerMask.NameToLayer(weaponLayerName));
            }
        }

        //Request a weapon switch
        [Command]
        public void CmdRequestWeaponSwitch(int requestedSlot)
        {
            RpcSwitchWeapon(requestedSlot);
        }

        //Switch Weapons on all clients
        [ClientRpc]
        private void RpcSwitchWeapon(int requestedSlot)
        {
            if (inventory.Count - 1 < requestedSlot)
            {
                Debug.Log("No Weapon Found In Slot!");
                return;
            }

            weaponHolder.transform.GetChild(_currentWeaponSlot).gameObject.SetActive(false);
            weaponHolder.transform.GetChild(requestedSlot).gameObject.SetActive(true);
            _currentWeaponSlot = requestedSlot;

            // Set current weapon and current graphics
            _currentWeapon = inventory[requestedSlot]; 
            _currentGraphics = weaponHolder.transform.GetChild(requestedSlot).gameObject.GetComponent<WeaponGraphics>();
            
            CmdOnReload();
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
            _audioSource.PlayOneShot(_currentWeapon.reload);
        }
    }
}