using PlayerScripts;
using UnityEngine;

namespace EnemiesScripts
{
    public class EnemyHitboxZone : MonoBehaviour
    {
        [SerializeField] private EnemyData data;

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerController.Instance.GetPlayerHitEvent?.Invoke(data.damage);
        }
    }
}
