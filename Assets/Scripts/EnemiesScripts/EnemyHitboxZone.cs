using PlayerScripts;
using UnityEngine;

namespace EnemiesScripts
{
    public class EnemyHitboxZone : MonoBehaviour
    {
        [SerializeField] private EnemyData data;

        [SerializeField] private EnemyController enemy;
        [SerializeField] private SpriteRenderer visuals;
        
        private int _rotation;

        private void Update()
        {
            FlipAttackPosition();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.TryGetComponent(out PlayerController player))
            {
                player.GetPlayerHitEvent?.Invoke(data.damage);
            }
        }

        private void FlipAttackPosition()
        {
            if (!enemy || enemy.GetCanAttack) return;

            _rotation = visuals.flipX ? 180 : 0;

            transform.parent.localRotation = Quaternion.Euler(0, 0, _rotation);
        }
    }
}
