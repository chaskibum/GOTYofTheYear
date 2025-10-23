using UnityEngine;

namespace PlayerScripts
{
    public class PlayerVisuals : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private PlayerController _playerController;
        
        // PA BORRAR DESPUÉS
        public bool facingRight = true;

        private void Start()
        {
            _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            _playerController = gameObject.GetComponentInParent<PlayerController>();
        }

        private void Update()
        {
            HandleFlipPlayer();
        }

        private void HandleFlipPlayer()
        {
            float moveInput = _playerController.GetMovementInput;

            if (_playerController.GetIsAttacking) return;
            
            if (moveInput > 0f)
            {
                _spriteRenderer.flipX = false;
                facingRight = true;
            }
            else if (moveInput < 0f)
            {
                _spriteRenderer.flipX = true;
                facingRight = false;
            }
        }
        
        public SpriteRenderer GetSpriteRenderer => _spriteRenderer;
    }
}
