using Mirror;
using UnityEngine;

namespace IntelligentCake.UI
{
    public class PauseMenu : MonoBehaviour
    {
        private NetworkManager _manager;
    
        public static bool IsOn = false;

        private void Start()
        {
            _manager = NetworkManager.singleton;
        }

        public void LeaveRoom()
        {
            _manager.StopHost();
        }
    }
}
