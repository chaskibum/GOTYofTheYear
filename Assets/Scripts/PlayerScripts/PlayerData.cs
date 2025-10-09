using UnityEngine;

namespace PlayerScripts
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "GOTYofTheYear/PlayerData")]
    public class PlayerData : ScriptableObject
    {
        public int baseHp;
        public int startingLives;
        public int baseLives;
        public float moveSpeed;
        public float jumpForce;
        public float regularGravity;
        public float fallGravity;
        public float pushForce;
        public float deathAnimationTime;
        
        [Header("Jump")]
        public float coyoteTime = 1f;
        public float jumpBufferTime = 0.1f;
        public float jumpDuration = 1f;

        [Header("Combat")] 
        public float attackSpeed = 1f;
        public float immunityTime = 1f;
    }
}
