using System;
using Mirror;
using UnityEngine;

namespace IntelligentCake.Player
{
    [RequireComponent(typeof(Player))]
    public class PlayerSetup : NetworkBehaviour
    {
        [SerializeField] private Behaviour[] componentsToDisable;

        private Camera _sceneCamera;

        [SerializeField] private string remoteLayerName = "RemotePlayer";

        [SerializeField] private string dontDrawLayerName = "DontDraw";
        [SerializeField] private GameObject playerGraphics;

        private void Start()
        {
            if (!isLocalPlayer)
            {
                DisableComponents();
                AssignRemoteLayer();
            }
            else
            {
                _sceneCamera = Camera.main;
                if (_sceneCamera != null)
                {
                    _sceneCamera.gameObject.SetActive(false);
                }
                
                // Disable player graphics for local player
                SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));
            }
            
            GetComponent<Player>().Setup();
        }

        private void SetLayerRecursively(GameObject obj, int newLayer)
        {
            obj.layer = newLayer;

            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, newLayer);
            }
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
            if (_sceneCamera != null)
            {
                _sceneCamera.gameObject.SetActive(true);
            }
            GameManager.UnRegisterPlayer(transform.name);
        }
    }
}
