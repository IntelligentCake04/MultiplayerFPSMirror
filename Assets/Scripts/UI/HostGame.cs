using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace IntelligentCake.UI
{
    public class HostGame : MonoBehaviour
    {
        private NetworkManager _manager;
    
        public InputField ipAddress;

        public InputField username;

        private void Awake()
        {
            _manager = FindObjectOfType<NetworkManager>();
            PlayerPrefs.DeleteAll();
        }

        private void Update()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void Host()
        {
            SetUsername(username.text);
            _manager.StartHost();
        }

        public void ConnectToServer()
        {
            _manager.networkAddress = ipAddress.text;
            SetUsername(username.text);
            if (_manager.networkAddress != null)
            {
                _manager.StartClient();
            }
        }

        private void SetUsername(string username)
        {
            PlayerPrefs.SetString("Username", username);
            PlayerPrefs.Save();
        }
    }
}
