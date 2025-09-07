using UnityEngine;

namespace EnemiesScripts
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "GOTYofTheYear/EnemyData")]
    public class EnemyData : ScriptableObject
    {
        public int hp;
        public int damage;
        public float moveSpeed;
    }
}
