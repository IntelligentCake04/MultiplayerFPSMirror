using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class SceneBrain : NetworkBehaviour
{
    //floor
    public GameObject sceneObjectToChangeColour;
    private Material sceneMaterialClone;
    //this will be the players prefab
    public TanksAdvance tankAdvance;
    public Text canvasPlayerMessageText;

///hooook
    [SyncVar(hook = nameof(OnChangedSceneColor))]
    public Color sceneColor = Color.white;

    void OnChangedSceneColor(Color _Old, Color _New)
    {
        sceneMaterialClone = new Material(sceneObjectToChangeColour.GetComponent<Renderer>().material);
        sceneMaterialClone.color = _New;
        sceneObjectToChangeColour.GetComponent<Renderer>().material = sceneMaterialClone;
    }
///end hooook


    // Network identity/behaviour scene scripts are disabled until the player is ready (usually when presses host/join button)
    void Start()
    {
        Debug.Log("isServer: " + isServer);
    }

    //only server/host can use these, you could hide the canvas buttons completely in start for non isServer players
    public void ButtonServerChangeScene()
    {
        if (isServer)
        {
            NetworkManager.singleton.ServerChangeScene(SceneManager.GetActiveScene().name);
        }
        else { Debug.Log("Access Denied isServer: " + isServer); }
    }

    //Host and Client can both use
    //however server doesn't have to go the long route through player authority objects as it has rpc control over this network script.
    public void ButtonSceneColor()
    {
        if (isServer)
        {
            RandomiseSceneColor();
        }
        else
        {
            if (tankAdvance) { tankAdvance.CmdChangeSceneColor(); }
        }
    }

    public void ButtonPlayerMessage()
    {
        if (tankAdvance) { tankAdvance.CmdSendPlayerMessage(); }
    }
    
    [ClientRpc]
    public void RpcSendMessagePlayer(string _value)
    {
        //Debug.Log("RpcSendMessagePlayer " + _value);
        canvasPlayerMessageText.text = _value;
    }
    
    public void RandomiseSceneColor()
    {
        //Debug.Log("RandomiseSceneColor: " + isServer);
        sceneColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
    }
}
