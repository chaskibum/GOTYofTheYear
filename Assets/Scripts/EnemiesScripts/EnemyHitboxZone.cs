using PlayerScripts;
using UnityEngine;

namespace EnemiesScripts
{
    public class EnemyHitboxZone : MonoBehaviour
    {
        [SerializeField] private EnemyData data;

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.TryGetComponent<PlayerController>(out PlayerController player)) 
                player.GetPlayerHitEvent?.Invoke(data.damage);
        }
    }
}
