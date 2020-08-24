using System;
using IntelligentCake.UI;
using Mirror;
using UnityEngine;

namespace IntelligentCake.Player
{
    [RequireComponent(typeof(Player))]
    [RequireComponent(typeof(PlayerMovement))]
    public class PlayerSetup : NetworkBehaviour
    {
        [SerializeField] private Behaviour[] componentsToDisable;

        private Camera _sceneCamera;

        [SerializeField] private string remoteLayerName = "RemotePlayer";

        [SerializeField] private string dontDrawLayerName = "DontDraw";
        [SerializeField] private GameObject playerGraphics;

        [SerializeField] private GameObject playerUIPrefab;
        private GameObject _playerUIInstance;

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
                
                // Create PlayerUI
                _playerUIInstance = Instantiate(playerUIPrefab);
                _playerUIInstance.name = playerUIPrefab.name;
                
                // Configure PlayerUI
                PlayerUI ui = _playerUIInstance.GetComponent<PlayerUI>();
                if (ui == null)
                    Debug.LogError("No PlayerUI component on PlayerUI prefab.");
                
                ui.SetPlayer(GetComponent<Player>());
                
                GetComponent<Player>().SetupPlayer();
                
                CmdSetUsername(transform.name, PlayerPrefs.GetString("Username"));
            }
        }

        [Command]
        void CmdSetUsername(string playerID, string username)
        {
            Player player = GameManager.GetPlayer(playerID);
            if (player != null)
            {
                Debug.Log(username + " has joined!");
                player.username = username;
            }
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
            Destroy(_playerUIInstance);

            if (isLocalPlayer)
            {
                if (_sceneCamera != null)
                {
                    _sceneCamera.gameObject.SetActive(true);
                }
            }
            GameManager.UnRegisterPlayer(transform.name);
        }
    }
}
