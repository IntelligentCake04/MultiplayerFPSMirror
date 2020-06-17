using System;
using Mirror;
using UnityEngine;

namespace IntelligentCake.Player
{
    public class PlayerSetup : NetworkBehaviour
    {
        /*[SerializeField] private Behaviour[] componentsToDisable;*/

        [SerializeField] private string remoteLayerName = "RemotePlayer";

        private void Start()
        {
            if (!isLocalPlayer)
            {
                /*DisableComponents();*/
                AssignRemoteLayer();
            }
            
            RegisterPlayer();
        }

        private void RegisterPlayer()
        {
            string id = "Player " + GetComponent<NetworkIdentity>().netId;
            transform.name = id;
        }

        private void AssignRemoteLayer()
        {
            gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
        }

        /*private void DisableComponents()
        {
            for (int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }
        }*/
    }
}
