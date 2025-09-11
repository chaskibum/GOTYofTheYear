using System;
using PlayerScripts;
using UnityEngine;

namespace EnemiesScripts.ArmorGhost
{
    public class GhostAttackZone : MonoBehaviour
    {
        [SerializeField] private EnemyData data;
        
        public bool isAttacking;
        public ArmorGhostBehaviour armorGhost;
        private SpriteRenderer _spriteRenderer;
        private PolygonCollider2D _collider;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<PolygonCollider2D>();
            
            _spriteRenderer.enabled = false;
            _collider.enabled = false;
        }
    
        public void Attack()
        {
            if (isAttacking) return;
            isAttacking = true;
            _spriteRenderer.enabled = true;
            _collider.enabled = true;
            Invoke("StopAttacking", 1f);
        }
    
        private void StopAttacking()
        {
            isAttacking = false;
            _spriteRenderer.enabled = false;
            _collider.enabled = false;
            armorGhost.SetState(ArmorGhostBehaviour.State.Chase);
        }
    
        private void FlipAttackPosition()
        {
            if (isAttacking) return;

            if (armorGhost.facingRight)
            {
                transform.localPosition = new Vector3(1.2f, 1f, 0f);
                transform.localRotation = Quaternion.Euler(0, 0, 270);
            }
            else if (!armorGhost.facingRight)
            {
                transform.localPosition = new Vector3(-1.2f, 1f, 0f);
                transform.localRotation = Quaternion.Euler(0, 0, 90);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerController.Instance.GetPlayerHitEvent?.Invoke(data.damage);
        }
    }
}
