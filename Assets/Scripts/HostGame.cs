using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace IntelligentCake
{
    public class HostGame : MonoBehaviour
    {
        private NetworkManager _manager;
    
        public InputField ipAddress;

        private void Awake()
        {
            _manager = FindObjectOfType<NetworkManager>();
        }

        private void Update()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void Host()
        {
            _manager.StartHost();
        }

        public void ConnectToServer()
        {
            _manager.networkAddress = ipAddress.text;
            if (_manager.networkAddress != null)
            {
                _manager.StartClient();
            }
        }
    }
}
