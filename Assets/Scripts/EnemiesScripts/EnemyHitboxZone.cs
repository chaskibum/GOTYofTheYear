using PlayerScripts;
using UnityEngine;

namespace EnemiesScripts
{
    public class EnemyHitboxZone : MonoBehaviour
    {
        [SerializeField] private EnemyData data;

        private void OnTriggerEnter2D(Collider2D other)
        {
            print("Player hit");
            if (TryGetComponent<PlayerController>(out PlayerController player)) 
                player.GetPlayerHitEvent?.Invoke(data.damage);
        }
    }
}
