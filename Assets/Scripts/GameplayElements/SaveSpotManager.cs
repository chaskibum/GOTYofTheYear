using System;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;
using TMPro;

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
        private TextMeshProUGUI _interactPromptText;

        private bool _isInRange;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _interactPromptText = interactPrompt.GetComponentInChildren<TextMeshProUGUI>();
            _interactPromptText.text = GetInteractionPrompt();

            GameManager.Instance.GetGameRestarted?.AddListener(ResetAnimations);
            GameManager.Instance.GetPlayer.GetPlayerRevived?.AddListener(ResetAnimations);
            Invoke(nameof(ActivateRespawn), 0.1f);
        }

        public string GetInteractionPrompt()
        {
            return "Presiona [E] para guardar";
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
            GameManager.Instance.GetGameRestarted?.RemoveListener(ResetAnimations);
            GameManager.Instance.GetPlayer.GetPlayerRevived?.RemoveListener(ResetAnimations);
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
            if (isMainRespawn) ActivateMainRespawn();
            else ActivateBonfireRespawn();

            OnCollisionEvent?.Invoke(this);
        }

        private void ActivateMainRespawn()
        {
            _lastSavedRespawnPosition = transform.position;
            GameManager.Instance.GetPlayer.SetMainRespawnPosition(_lastSavedRespawnPosition);
            PlayActivatingSound();
            GameManager.Instance.GetPlayer.SetRespawnPosition(_lastSavedRespawnPosition);
        }

        private void ActivateBonfireRespawn()
        {
            _lastSavedRespawnPosition = transform.position;
            GameManager.Instance.GetPlayer.SetRespawnPosition(_lastSavedRespawnPosition);
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
