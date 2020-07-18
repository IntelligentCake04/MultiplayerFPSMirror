using System;
using IntelligentCake.Player;
using UnityEngine;

namespace IntelligentCake.Combat
{
    public class WeaponOffset : MonoBehaviour
    {
                
        public float posOffset = 0.1f;
        private PlayerMovement _playerMovement;
        private Vector3 _currentVelocity;
        public float speed = 0.2f;

        private void Start()
        {
            _playerMovement = GetComponentInParent<PlayerMovement>();
        }
        
        private void Update()
        {
            Vector3 offset = _playerMovement.FindVelRelativeToLook() * posOffset;
            Vector3 desiredPos = Vector3.zero - new Vector3(offset.x, transform.localPosition.y, offset.y);
            transform.localPosition = Vector3.SmoothDamp(desiredPos, Vector3.zero, ref _currentVelocity, speed * Time.deltaTime);
        }
    }
}
