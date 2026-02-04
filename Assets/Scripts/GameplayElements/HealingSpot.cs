using Managers;
using UnityEngine;
using Utils;

namespace GameplayElements
{
    public class HealingSpot : MonoBehaviour, IInteractable
    {
        [SerializeField] private AudioSource healSound;
        
        private Animator _animator;
        private bool _isInRange;
        
        private GameManager _gm;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _gm = GameManager.Instance;
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
                _gm.GetGameRestarted?.AddListener(ResetAnimations);
                _gm.GetPlayerRespawn?.AddListener(ResetAnimations);
                _gm.GetPlayer.GetPlayerRevived?.AddListener(ResetAnimations);
            }
            else
            {
                _gm.GetGameRestarted?.RemoveListener(ResetAnimations);
                _gm.GetPlayerRespawn?.RemoveListener(ResetAnimations);
                _gm.GetPlayer.GetPlayerRevived?.RemoveListener(ResetAnimations);
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
                CheckInteractInput();
                _animator.SetBool("OnRange", true);
            }
            else
            {
                _animator.SetBool("OnRange", false);
            }
        }

        private void CheckInteractInput()
        {
            if (Input.GetButtonDown("GameInteract"))
            {
                Interact();
            }
        }

        public void Interact()
        {
            if (_animator.GetBool("Used")) return;
            healSound.Play();
            _gm.GetPlayer.Heal();
            _animator.SetBool("Used", true);
        }
    
        private void ResetAnimations()
        {
            if (!_animator) return;
        
            _animator.SetBool("Used", false);
        }
    }
}
