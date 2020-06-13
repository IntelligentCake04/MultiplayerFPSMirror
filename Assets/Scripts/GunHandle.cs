using Mirror;
using UnityEngine;

namespace IntelligentCake
{
    public class GunHandle : NetworkBehaviour
    {
        private Gun _gun;
        private void Start()
        {
            _gun = GetComponentInChildren<Gun>();
        }

        private void Update()
        {
            if (!hasAuthority) return;
        
            if (_gun.isAutomatic == false)
                if (Input.GetButtonDown("Fire1") && Time.time >= _gun.nextTimeToFire)
                {
                    _gun.nextTimeToFire = Time.time + 1f / _gun.fireRate;
                    Shoot();
                }

            if (_gun.isAutomatic)
                if (Input.GetButton("Fire1") && Time.time >= _gun.nextTimeToFire)
                {
                    _gun.nextTimeToFire = Time.time + 1f / _gun.fireRate;
                    Shoot();
                }
        }

        [Client]
        private void Shoot()
        {
            if (Physics.Raycast(_gun.fpsCam.transform.position, _gun.fpsCam.transform.forward, out _gun.hit, _gun.range))
            {
                /*Debug.Log(hit.transform.name);

            Player player = hit.transform.GetComponent<Target>();
            if (player != null) target.TakeDamage(_gun.damage);*/

                CmdPlayerShot();
            }
        }

        [Command]
        private void CmdPlayerShot()
        {
            Debug.Log(_gun.hit.transform.name);
        }
    }
}