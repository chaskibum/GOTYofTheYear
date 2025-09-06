using UnityEngine;

namespace PlayerScripts
{
    public class PlayerWeapon : MonoBehaviour
    {
        public bool isAttacking;
        private SpriteRenderer _spriteRenderer;
        private PolygonCollider2D _collider;
        
        // PA BORRAR DESPUES
        [SerializeField] private PlayerVisuals _playerVisuals;
        
        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<PolygonCollider2D>();
            
            _spriteRenderer.enabled = false;
            _collider.enabled = false;
        }
        
        // PA BORRAR DESPUES
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
            Invoke("StopAttacking", 0.5f);
        }

        private void StopAttacking()
        {
            isAttacking = false;
            _spriteRenderer.enabled = false;
            _collider.enabled = false;
        }

        // PA BORRAR DESPUES
        private void FlipAttackPosition()
        {
            if (isAttacking) return;

            if (_playerVisuals.facingRight)
            {
                transform.localPosition = new Vector3(1.2f, 1f, 0f);
                transform.localRotation = Quaternion.Euler(0, 0, 270);
            }
            else if (!_playerVisuals.facingRight)
            {
                transform.localPosition = new Vector3(-1.2f, 1f, 0f);
                transform.localRotation = Quaternion.Euler(0, 0, 90);
            }
        }
    }
}
