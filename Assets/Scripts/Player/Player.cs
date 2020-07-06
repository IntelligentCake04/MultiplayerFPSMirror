using System;
using System.Collections;
using Mirror;
using UnityEngine;

namespace IntelligentCake.Player
{
    public class Player : NetworkBehaviour
    {
        [SyncVar]
        private bool _isDead = false;
        public bool isDead
        {
            get => _isDead;
            protected set => _isDead = value;
        }

        [SerializeField]
        private int maxHealth = 100;
        
        [SyncVar]
        public float currentHealth;

        [SerializeField]
        private Behaviour[] disableOnDeath;
        private bool[] _wasEnabled;
        public void Setup()
        {
            _wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < _wasEnabled.Length; i++)
            {
                _wasEnabled[i] = disableOnDeath[i].enabled;
            }
            
            SetDefaults();
        }

        private void Start()
        {
            GetComponent<Animator>().enabled = true;
            SetRigidbodyState(true);
            SetColliderState(false);
        }

        private void Update()
        {
            if (!isLocalPlayer)
                    return;
            
            if (Input.GetKeyDown(KeyCode.K))
                RpcTakeDamage(9999);
        }

        [ClientRpc]
        public void RpcTakeDamage(int amount)
        {
            if(isDead)
                return;
            
            currentHealth -= amount;
            
            Debug.Log(transform.name + " now has " + currentHealth + " health.");

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            isDead = true;

            for (int i = 0; i < disableOnDeath.Length; i++)
            {
                disableOnDeath[i].enabled = false;
            }
            
            Collider col = GetComponent<Collider>();
            if (col != null)
                col.enabled = true;
            
            Debug.Log(transform.name + " is DEAD!");

            StartCoroutine(Respawn());
            
            GetComponent<Animator>().enabled = false;
            SetRigidbodyState(false);
            SetColliderState(true);
        }

        private IEnumerator Respawn()
        {
            yield return new WaitForSeconds(GameManager.Instance.matchSettings.respawnTime);
            SetDefaults();
            Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
            SetRigidbodyState(true);
            SetColliderState(false);
            GetComponent<Animator>().enabled = true;
            Debug.Log(transform.name + " respawned.");
        }

        public void SetDefaults()
        {
            isDead = false;
            
            currentHealth = maxHealth;
            for (int i = 0; i < disableOnDeath.Length; i++)
            {
                disableOnDeath[i].enabled = _wasEnabled[i];
            }

            Collider col = GetComponent<Collider>();
            if (col != null)
                col.enabled = true;
        }

        private void SetRigidbodyState(bool state)
        {
            Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

            foreach (Rigidbody rigidbody in rigidbodies)
            {
                rigidbody.isKinematic = state;
                rigidbody.AddForce(Vector3.down * (Time.deltaTime * 10));
            }

            GetComponent<Rigidbody>().isKinematic = !state;
        }
        
        private void SetColliderState(bool state)
        {
            Collider[] colliders = GetComponentsInChildren<Collider>();

            foreach (Collider collider in colliders)
            {
                collider.enabled = state;
            }
            
            GetComponent<Collider>().enabled = !state;
        }
    }
}