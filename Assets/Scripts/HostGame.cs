using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class HostGame : MonoBehaviour
{
    private NetworkManager _manager;
    
    public InputField ipAddress;

    private void Awake()
    {
        _manager = GetComponent<NetworkManager>();
    }

    public void Host()
    {
        _manager.StartHost();
    }

    public void ConnectToServer()
    {
        if (_manager.networkAddress == null)
        {
            ConnectToServer();
        }

        _manager.networkAddress = ipAddress.text;
        
        _manager.StartClient();
    }
}
