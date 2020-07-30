using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Networking;

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
