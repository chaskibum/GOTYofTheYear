using UnityEngine;

namespace PlayerScripts
{
    public class PlayerWeapon : MonoBehaviour
    {
        private float _position;
        
        [SerializeField] private PlayerVisuals playerVisuals;
        
        public void FlipAttackPosition()
        {
            _position = playerVisuals.facingRight ? 2.8f : -2.8f;
            
            var vector3 = transform.parent.localPosition;
            vector3.x = _position;
            transform.parent.localPosition = vector3;
        }
    }
}
