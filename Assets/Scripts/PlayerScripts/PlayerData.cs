using UnityEngine;

namespace PlayerScripts
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "GOTYofTheYear/PlayerData")]
    public class PlayerData : ScriptableObject
    {
        public int baseHp;
        public float moveSpeed;
        public float jumpForce;
        public float regularGravity;
        public float fallGravity;
        public float pushForce;
        
        [Header("CoyoteTime")]
        public float coyoteTime = 1f;
        
        [Header("JumpBuffering")]
        public float jumpBufferTime = 0.1f;
    }
}
