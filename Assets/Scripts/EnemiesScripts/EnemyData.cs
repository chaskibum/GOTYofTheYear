using UnityEngine;

namespace EnemiesScripts
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "GOTYofTheYear/EnemyData")]
    public class EnemyData : ScriptableObject
    {
        [Header("Properties")]
        public int baseHp;
        public int damage;
        public float moveSpeed;
        
        [Header("Combat")]
        public float attackAnimationTime;
        public float attackCooldown;
        public float detectionRange;
        public float attackRange;

        [Header("Physics")] 
        public float pushForce;
        public float pushTime;
    }
}
