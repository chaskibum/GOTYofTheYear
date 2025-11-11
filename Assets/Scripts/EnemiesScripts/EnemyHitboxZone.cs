using PlayerScripts;
using UnityEngine;

namespace EnemiesScripts
{
    public class EnemyHitboxZone : MonoBehaviour
    {
        [SerializeField] private EnemyData data;

        [SerializeField] private EnemyController enemy;
        [SerializeField] private SpriteRenderer visuals;
        [SerializeField] private GameObject particles;
        
        private int _rotation;
        private int _particlesRotation;

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
            _particlesRotation = visuals.flipX ? 135 : -45;

            transform.parent.localRotation = Quaternion.Euler(0, 0, _rotation);
            if (particles) particles.transform.localRotation = Quaternion.Euler(0, 0, _particlesRotation);
        }
    }
}
