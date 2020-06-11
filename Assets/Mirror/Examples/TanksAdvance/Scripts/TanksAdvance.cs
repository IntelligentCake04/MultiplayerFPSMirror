using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

// Version 2 - www.StephenAllenGames.co.uk - JesusLuvsYooh
// Made by a 'still learning' Mirror user.
// What you see may not be the best way, but it is a way to help get started.

//the first section of this is mostly unchanged from the original 'tank' example
//scroll to bottom for new stuff


public class TanksAdvance : NetworkBehaviour
{
    [Header("Components")]
    public NavMeshAgent agent;
    public Animator animator;

    [Header("Movement")]
    public float rotationSpeed = 100;

    [Header("Firing")]
    public KeyCode shootKey = KeyCode.Space;
    public GameObject projectilePrefab;
    public Transform projectileMount;


    void Update()
    {
        // movement for local player
        if (isDead) return;  //jesus edit
        if (!isLocalPlayer) return;

        // rotate
        float horizontal = Input.GetAxis("Horizontal");
        transform.Rotate(0, horizontal * rotationSpeed * Time.deltaTime, 0);

        // move
        float vertical = Input.GetAxis("Vertical");
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        agent.velocity = forward * Mathf.Max(vertical, 0) * agent.speed;
        animator.SetBool("Moving", agent.velocity != Vector3.zero);

        // shoot
        if (Input.GetKeyDown(shootKey))
        {
            CmdFire();
        }
    }

    // this is called on the server
    [Command]
    void CmdFire()
    {
        GameObject projectile = Instantiate(projectilePrefab, projectileMount.position, transform.rotation);
        NetworkServer.Spawn(projectile);
        RpcOnFire();
    }

    // this is called on the tank that fired for all observers
    [ClientRpc]
    void RpcOnFire()
    {
        animator.SetTrigger("Shoot");
    }


    ///////////////////////////////// ///////////////////////////////// /////////////////////////////////
    // Edits from the original tank will mostly go under here       


    [Header("Advance")]
    public GameObject[] hideOnDeath;
    public TextMesh textMesh;

    [SyncVar(hook = nameof(OnNameChanged))]
    public string playerName;
    void OnNameChanged(string _Old, string _New) { UpdatePlayerInfo(); }

    [SyncVar(hook = nameof(OnHPChanged))]
    public int currentHP = 10;
    void OnHPChanged(int _Old, int _New) { UpdatePlayerInfo(); }

    [SyncVar(hook = nameof(SetDeathState))]
    public bool isDead = false;


    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        //Your player data would most likely be set in another scene, got from static variables, singleton, playerprefs etc
        //set random player name and health amount for now

        //telling the central scene script which our player object is, as it has authority on, for Cmd calls
        if (sceneBrain) { sceneBrain.tankAdvance = this; }

        // string name, int health
        CmdSetupPlayer("Player" + Random.Range(100, 999), Random.Range(5, 21));

        //hide this camera section if you dont want camera attached to player
        Camera.main.transform.SetParent(transform);
        Camera.main.transform.localPosition = new Vector3(0f, 3f, -2f);
        Camera.main.transform.localEulerAngles = new Vector3(45f, 0f, 0f);
        CmdChangeSceneColor();
    }


    void UpdatePlayerInfo()
    {
        //called from sync var hook, to update player info on screen for all players
        textMesh.text = playerName + "\nHP: " + currentHP;
    }


    void SetDeathState(bool oldValue, bool newValue)
    {
        if (newValue == true)
        {
            foreach (GameObject obj in hideOnDeath) { obj.SetActive(false); }
            // this.GetComponent<MeshRenderer>().enabled = false; if your player has mesh on main object
            //collider etc
        }
        else
        {
            foreach (GameObject obj in hideOnDeath) { obj.SetActive(true); }
        }
    }


    private void OnGUI()
    {
        if (!isLocalPlayer) return; // or  if (!hasAuthority) return;

        if (isDead == false && GUI.Button(new Rect(5, 200, 100, 30), "Damage"))
        {
            //damage self, good for testing
            CmdTakeDamage(1);
        }
        if (isDead && GUI.Button(new Rect(5, 240, 100, 30), "Respawn"))
        {
            //shows only when dead
            CmdRespawn();
        }
    }

    [Command]
    void CmdTakeDamage(int damage)
    {
        currentHP -= damage;
        if (sceneBrain) { sceneBrain.RpcSendMessagePlayer(playerName + " took " + damage + " damage"); }
        if (currentHP <= 0)
        { 
            isDead = true;
            if (sceneBrain) { sceneBrain.RpcSendMessagePlayer(playerName + " was defeated."); }
        }
    }

    [Command]
    public void CmdRespawn()
    {
        // set player back to default settings
        isDead = false;
        currentHP = 10; //or code in an original hp variable thats set at begining, then set to that and not 10
        if (sceneBrain) { sceneBrain.RpcSendMessagePlayer(playerName + " respawned."); }
    }

    [Command]
    public void CmdSetupPlayer(string _name, int _health)
    {
        //player info sent to server, then server handles it all
        playerName = _name;
        currentHP = _health;
        RpcAlertJoin();
        if (sceneBrain) { sceneBrain.RpcSendMessagePlayer(playerName + " joined the game."); }
    }

    [ClientRpc]
    void RpcAlertJoin()
    {
        // server sends this to all players
        StartCoroutine(AlertJoin());
    }

    private IEnumerator AlertJoin()
    {
        // add sparkles, message onscreen etc
        // delay added to allow player sync vars and other info to finish
        yield return new WaitForSeconds(1.0f);
        //Debug.Log(playerName + " joined the game.");
    }




    ///////////////////////////////// ///////////////////////////////// /////////////////////////////////
    // These are for SceneBrain

    public SceneBrain sceneBrain;
    
    void Awake()
    {
        sceneBrain = GameObject.Find("SceneBrain").GetComponent<SceneBrain>();
    }
    
    [Command]
    public void CmdChangeSceneColor()
    {
        int sender = connectionToClient.connectionId;
        Debug.Log("Change Scene color sent by ID: " + sender);
        if (sceneBrain) { sceneBrain.RandomiseSceneColor(); }
        else
        {
            Debug.Log("no brain");
        }
    }
    
    [Command]
    public void CmdSendPlayerMessage()
    {
        if (sceneBrain) { sceneBrain.RpcSendMessagePlayer(playerName + " says hello " + Random.Range(10, 99)); }
        else { Debug.Log("no brain"); }
    }
}