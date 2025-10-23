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
        public float regularGravity;
        public float fallGravity;
        public float pushForce;
        public float deathAnimationTime;
        
        [Header("Jump")]
        public float jumpForce;
        public float coyoteTime = 1f;
        public float jumpBufferTime = 0.1f;
        public float jumpDuration = 1f;
        
        [Header("Dash")]
        public float dashSpeed;
        public float dashDuration;
        public float dashGravity;
        

        [Header("Combat")] 
        public float attackSpeed = 1f;

        public float attackCooldown = 1f;
        public float immunityTime = 1f;
        public float shieldDuration = 3f;
        public float shieldCooldown = 15f;
    }
}
