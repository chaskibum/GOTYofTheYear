using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace GameplayElements
{
    public class HealingSpot : MonoBehaviour, IInteractable
    {
        [SerializeField] private AudioSource healSound;
        
        private Animator _animator;
        private bool _isInRange;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            ResetAnimations();
            AddListeners(true);
        }

        private void OnDestroy()
        {
            AddListeners(false);
        }

        private void AddListeners(bool add)
        {
            if (add)
            {
                GameManager.Instance.GetGameRestarted?.AddListener(ResetAnimations);
                GameManager.Instance.GetPlayerRespawn?.AddListener(ResetAnimations);
                GameManager.Instance.GetPlayer.GetPlayerRevived?.AddListener(ResetAnimations);
            }
            else
            {
                GameManager.Instance.GetGameRestarted?.RemoveListener(ResetAnimations);
                GameManager.Instance.GetPlayerRespawn?.RemoveListener(ResetAnimations);
                GameManager.Instance.GetPlayer.GetPlayerRevived?.RemoveListener(ResetAnimations);
            }
        }
    
        private void OnTriggerStay2D(Collider2D other)
        {
            _isInRange = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _isInRange = false;
        }
    
        void Update()
        {
            if (_isInRange)
            {
                // CheckInteractInput();
                _animator.SetBool("OnRange", true);
            }
            else
            {
                _animator.SetBool("OnRange", false);
            }
        }

        public void OnInteract(InputAction.CallbackContext ctx)
        {
            if (_isInRange)
            {
                if (ctx.performed) Interact();
            }
        }
        public void Interact()
        {
            if (_animator.GetBool("Used")) return;
            healSound.Play();
            GameManager.Instance.GetPlayer.Heal();
            _animator.SetBool("Used", true);
        }
    
        private void ResetAnimations()
        {
            if (!_animator) return;
        
            _animator.SetBool("Used", false);
        }
    }
}
