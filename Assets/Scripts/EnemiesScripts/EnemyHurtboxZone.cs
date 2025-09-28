using UnityEngine;

namespace EnemiesScripts
{
    public class EnemyHurtboxZone : MonoBehaviour
    {
        [SerializeField] private EnemyController enemy;

        private void OnTriggerEnter2D(Collider2D other)
        {
            enemy.GetHit();
        }
    }
}
