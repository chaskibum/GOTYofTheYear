using System;
using UnityEngine;

namespace PlayerScripts
{
    public class PlayerWeapon : MonoBehaviour
    {
        public bool isAttacking;
        private int _rotation;
        private PlayerController _player;
        
        [SerializeField] private PlayerVisuals playerVisuals;


        private void Start()
        {
            _player = GameManager.Instance.GetPlayer;
        }
        
        public void FlipAttackPosition()
        {
            // if (_player.GetIsAttacking) return;

            _rotation = playerVisuals.facingRight ? 0 : 180;

            transform.parent.localRotation = Quaternion.Euler(0, 0, _rotation);
        }
    }
}
