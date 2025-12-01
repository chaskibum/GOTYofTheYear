using UnityEngine;

namespace PlayerScripts
{
    public class PlayerVisuals : MonoBehaviour
    {
        [SerializeField] private Transform weaponLight;
        [SerializeField] private ParticleSystem powerupParticles;
        private SpriteRenderer _spriteRenderer;
        private PlayerController _playerController;

        public bool facingRight = true;

        private void Start()
        {
            _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            _playerController = gameObject.GetComponentInParent<PlayerController>();
            powerupParticles.Stop();
            /*yield return new WaitForSeconds(0.2f);
            powerupParticles.transform.parent.gameObject.SetActive(true);*/
        }

        private void Update()
        {
            HandleFlipPlayer();
        }
        
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

        public void PlayPowerupParticles()
        {
            powerupParticles.Play();
        }
        
        public SpriteRenderer GetSpriteRenderer => _spriteRenderer;
    }
}
