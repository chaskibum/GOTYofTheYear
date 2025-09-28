using UnityEngine;

namespace PlayerScripts
{
    public class PlayerWeapon : MonoBehaviour
    {
        public bool isAttacking;
        private SpriteRenderer _spriteRenderer;
        private PolygonCollider2D _collider;
        
        // PARA BORRAR DESPUÉS
        [SerializeField] private PlayerVisuals playerVisuals;
        
        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<PolygonCollider2D>();
            
            _spriteRenderer.enabled = false;
            _collider.enabled = false;
        }
        
        // PARA BORRAR DESPUÉS
        private void Update()
        {
            FlipAttackPosition();
        }

        public void Attack()
        {
            if (isAttacking) return;
            isAttacking = true;
            _spriteRenderer.enabled = true;
            _collider.enabled = true;
            Invoke(nameof(StopAttacking), 1f);
        }

        private void StopAttacking()
        {
            isAttacking = false;
            _spriteRenderer.enabled = false;
            _collider.enabled = false;
        }

        // PARA BORRAR DESPUÉS
        private void FlipAttackPosition()
        {
            if (isAttacking) return;

            if (playerVisuals.facingRight)
                transform.parent.localRotation = Quaternion.Euler(0, 0, 0);
            else if (!playerVisuals.facingRight)
                transform.parent.localRotation = Quaternion.Euler(0, 0, 180);
        }
    }
}
