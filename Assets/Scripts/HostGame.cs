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

    public Button connect;

    private void Awake()
    {
        _manager = FindObjectOfType<NetworkManager>();
    }

    private void Update()
    {
        Cursor.visible = true;
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
