using UnityEngine;

namespace EnemiesScripts
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "GOTYofTheYear/EnemyData")]
    public class EnemyData : ScriptableObject
    {
        public int baseHp;
        public int damage;
        public float moveSpeed;
        public float attackAnimationTime;
        public float attackCooldown;

        public float detectionRange;
        public float attackRange;
    }
}
