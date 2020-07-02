using System;
using IntelligentCake.Player;
using UnityEngine;

namespace IntelligentCake.Combat
{
    public class WeaponOffset : MonoBehaviour
    {
                
        public float posOffset = 0.1f;
        private PlayerMovement playerMovement;
        private Vector3 currentVelocity;
        public float speed = 0.2f;

        private void Start()
        {
            playerMovement = GetComponentInParent<PlayerMovement>();
        }
        
        private void Update()
        {
            Vector3 offset = playerMovement.FindVelRelativeToLook() * posOffset;
            Vector3 desiredPos = Vector3.zero - new Vector3(offset.x, transform.localPosition.y, offset.y);
            transform.localPosition = Vector3.SmoothDamp(desiredPos, Vector3.zero, ref currentVelocity, speed * Time.deltaTime);
        }
    }
}
