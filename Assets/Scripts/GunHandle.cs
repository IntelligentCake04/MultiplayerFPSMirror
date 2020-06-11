/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GunHandle : NetworkBehaviour
{
    public ParticleSystem muzzleFlash;
    public GameObject bullet;

    public Gun _gun;

    private void Start()
    {
        _playerEquip = GetComponent<PlayerEquip>();
    }
    [Command]
    public void CmdShowGunParticles()
    {
        RpcShowGunParticles();
    }

    [ClientRpc]
    void RpcShowGunParticles()
    {
        muzzleFlash.Play();
        GameObject bulletGO = Instantiate(bullet, muzzleFlash.transform.position, _gun.fpsCam.transform.rotation);
        Destroy(bulletGO, 10f);
    }
}
*/