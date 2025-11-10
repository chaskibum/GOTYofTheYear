using UnityEngine;

namespace PlayerScripts
{
    public class PlayerVisuals : MonoBehaviour
    {
        [SerializeField] private Transform weaponLight;
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

        /*private void HandleFlipPlayer()
        {
            float moveInput = _playerController.GetMovementInput;

            if (_playerController.GetIsAttacking) return;
            
            if (moveInput > 0f)
            {
                _spriteRenderer.flipX = false;
                facingRight = true;
                weaponLight.localPosition = new Vector3(-0.88f, 2f, 0);
            }
            else if (moveInput < 0f)
            {
                _spriteRenderer.flipX = true;
                facingRight = false;
                var vector3 = weaponLight.localPosition;
                vector3.x = vector3.x * -1;
                weaponLight.localPosition = vector3;
            }
        }*/
        
        private void HandleFlipPlayer()
        {
            float moveInput = _playerController.GetMovementInput;

            if (_playerController.GetIsAttacking) return;

            if (moveInput > 0f && !facingRight)
                Flip();
            else if (moveInput < 0f && facingRight)
                Flip();
        }

        private void Flip()
        {
            facingRight = !facingRight;
            _spriteRenderer.flipX = !facingRight;
            weaponLight.localPosition = new Vector3(-weaponLight.localPosition.x, weaponLight.localPosition.y, 0);
        }
        
        public SpriteRenderer GetSpriteRenderer => _spriteRenderer;
    }
}
