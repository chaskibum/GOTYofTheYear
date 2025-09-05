using UnityEngine;

namespace PlayerScripts
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "GOTYofTheYear/PlayerData")]
    public class PlayerData : ScriptableObject
    {
        public int hp;
        public float moveSpeed;
        public float jumpForce;
        public float regularGravity;
        public float fallGravity;
        
        [Header("CoyoteTime")]
        public float coyoteTime = 1f;
        public bool canUseCoyoteTime;
        
        [Header("JumpBuffering")]
        public float jumpBufferTime = 0.1f;
        public float lastJumpPressedTime;
    }
}
