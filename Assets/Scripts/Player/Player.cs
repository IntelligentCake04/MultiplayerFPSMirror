using System;
using Mirror;
using UnityEngine;

namespace IntelligentCake.Player
{
    public class Player : NetworkBehaviour
    {
        [SyncVar]
        private bool _isDead = false;

        public bool IsDead
        {
            get => _isDead;
            protected set => _isDead = value;
        }

        [SerializeField]
        private int maxHealth = 100;
        
        [SyncVar]
        public float currentHealth;

        private void Awake()
        {
            SetDefaults();
        }
        
        [ClientRpc]
        public void RpcTakeDamage(int amount)
        {
            currentHealth -= amount;
            
            Debug.Log(transform.name + " now has " + currentHealth + " health.");
        }

        public void SetDefaults()
        {
            currentHealth = maxHealth;
        }
    }
}