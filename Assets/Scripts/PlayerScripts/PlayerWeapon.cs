using UnityEngine;

namespace PlayerScripts
{
    public class PlayerWeapon : MonoBehaviour
    {
        public bool isAttacking;
        private int _rotation;
        
        [SerializeField] private PlayerVisuals playerVisuals;
        
        
        private void Update()
        {
            FlipAttackPosition();
        }
        
        private void FlipAttackPosition()
        {
            if (isAttacking) return;

            _rotation = playerVisuals.facingRight ? 0 : 180;

            transform.parent.localRotation = Quaternion.Euler(0, 0, _rotation);
        }
    }
}
