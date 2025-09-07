using PlayerScripts;
using UnityEngine;

namespace EnemiesScripts
{
    public class EnemyHitboxZone : MonoBehaviour
    {
        [SerializeField] private PlayerController player;

        [SerializeField] private EnemyData data;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!player.isImmune) player.GetHit(data.damage);
        }
    }
}
