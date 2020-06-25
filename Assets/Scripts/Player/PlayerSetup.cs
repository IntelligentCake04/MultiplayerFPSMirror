using System;
using Mirror;
using UnityEngine;

namespace IntelligentCake.Player
{
    [RequireComponent(typeof(Player))]
    public class PlayerSetup : NetworkBehaviour
    {
        [SerializeField] private Behaviour[] componentsToDisable;

        private Camera sceneCamera;

        [SerializeField] private string remoteLayerName = "RemotePlayer";

        private void Start()
        {
            if (!isLocalPlayer)
            {
                DisableComponents();
                AssignRemoteLayer();
            }
            else
            {
                sceneCamera = Camera.main;
                if (sceneCamera != null)
                {
                    sceneCamera.gameObject.SetActive(false);
                }
            }
            
            GetComponent<Player>().Setup();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            string netId = GetComponent<NetworkIdentity>().netId.ToString();
            Player player = GetComponent<Player>();
            
            GameManager.RegisterPlayer(netId, player);
        }

        private void AssignRemoteLayer()
        {
            gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
        }

        private void DisableComponents()
        {
            for (int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }
        }

        private void OnDisable()
        {
            if (sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(true);
            }
            GameManager.UnRegisterPlayer(transform.name);
        }
    }
}
