using PlayerScripts;
using UnityEngine;

namespace EnemiesScripts
{
    public class EnemyHitboxZone : MonoBehaviour
    {
        [SerializeField] private EnemyData data;

        private EnemyController _enemy;
        [SerializeField] private SpriteRenderer visuals;
        
        private int _rotation;

        private void Start()
        {
            TryGetComponent(out _enemy);
        }

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
            if (!_enemy || _enemy.GetCanAttack) return;

            _rotation = visuals.flipX ? 180 : 0;
            
            print("rotating...");
            
            transform.parent.localRotation = Quaternion.Euler(0, 0, _rotation);
        }
    }
}
