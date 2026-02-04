using System;
using Managers;
using UnityEngine;
using Utils;

namespace GameplayElements
{
    public class SaveSpotManager : MonoBehaviour, IInteractable
    {
        private Vector3 _lastSavedRespawnPosition;
        private Animator _animator; 
        private static event Action<SaveSpotManager> OnCollisionEvent;

        [SerializeField] private bool isMainRespawn;
        [SerializeField] private AudioSource activatingSound;
        [SerializeField] private AudioSource torchSound;
        [SerializeField] private GameObject interactPrompt;
    
        private bool _isInRange;
        
        private GameManager _gm;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _gm = GameManager.Instance;
           _gm.GetGameRestarted?.AddListener(ResetAnimations);
            _gm.GetPlayer.GetPlayerRevived?.AddListener(ResetAnimations);
            Invoke(nameof(ActivateRespawn), 0.1f);
        }

        private void ActivateRespawn()
        {
            if (_isInRange)
            {
                Interact();
            }
        }

        private void OnEnable()
        {
            OnCollisionEvent += HandleAnimations;
        }

        private void OnDestroy()
        {
            OnCollisionEvent -= HandleAnimations;
            _gm.GetGameRestarted?.RemoveListener(ResetAnimations);
            _gm.GetPlayer.GetPlayerRevived?.RemoveListener(ResetAnimations);
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
            if (isMainRespawn) ActivateMainRespawn();
            else ActivateBonfireRespawn();
        
            OnCollisionEvent?.Invoke(this);
        }

        private void ActivateMainRespawn()
        {
            _lastSavedRespawnPosition = transform.position;
            _gm.GetPlayer.SetMainRespawnPosition(_lastSavedRespawnPosition);
            PlayActivatingSound();
            _gm.GetPlayer.SetRespawnPosition(_lastSavedRespawnPosition);
        }

        private void ActivateBonfireRespawn()
        {
            _lastSavedRespawnPosition = transform.position;
            _gm.GetPlayer.SetRespawnPosition(_lastSavedRespawnPosition);
        }

        private void HandleAnimations(SaveSpotManager activated)
        {
            if (!_animator) return;
        
            bool isActive = activated == this;

            if (isMainRespawn && _animator.GetBool("Activated")) return;
        
            _animator.SetBool("Activated", isActive);
        }

        public void PlayActivatingSound()
        {
            if (!activatingSound) return;
            activatingSound.Play();
            if (torchSound) torchSound.Play();
        }
    
        private void ResetAnimations()
        {
            if (!_animator) return;
        
            _animator.SetBool("Activated", false);
            Invoke(nameof(ActivateRespawn), 0.1f);
        }
    }
}
