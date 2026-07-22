using Managers;
using PlayerScripts;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;
using TMPro;

namespace GameplayElements
{
    public class Totem : MonoBehaviour, IInteractable
    {
        private Animator _animator;
        private bool _isInRange;
        private PlayerController _player;
        [SerializeField] private bool startActivated;
        [SerializeField] private GameObject teletransport;
        [SerializeField] private GameObject interactPrompt;
        private TextMeshProUGUI _interactPromptText;
        [SerializeField] private AudioSource activeSound;

        private void Start()
        {
            _interactPromptText = interactPrompt.GetComponentInChildren<TextMeshProUGUI>();
            _interactPromptText.text = GetInteractionPrompt();

            _animator = GetComponent<Animator>();
            _player = GameManager.Instance.GetPlayer;
            AddListeners(true);
            if (startActivated || PlayerPrefs.GetString(name) == name)
                _animator.SetBool("Active", true);
        }

        public string GetInteractionPrompt()
        {
            return "Presiona [E] para usar tótem";
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
            }
            else
            {
                GameManager.Instance.GetGameRestarted?.RemoveListener(ResetAnimations);
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
                if (_animator.GetBool("Active"))
                {
                    // CheckInteractInput();
                    interactPrompt.SetActive(true);
                }
            }
            else
            {
                interactPrompt.SetActive(false);
            }
        }

        /*private void CheckInteractInput()
        {
            if (Input.GetButtonDown("GameInteract"))
            {
                Interact();
            }
        }*/

        public void OnInteract(InputAction.CallbackContext ctx)
        {
            if (_isInRange)
            {
                if (_animator.GetBool("Active"))
                    if (ctx.performed) Interact();
            }
        }

        public void Interact()
        {
            _player.transform.position = teletransport.transform.position;
            teletransport.TryGetComponent(out Animator animator);
            animator.SetBool("Active", true);
            activeSound.Play();
        }

        public void SaveActivatedStatus()
        {
            if (!startActivated)
            {
                PlayerPrefs.SetString(name, name);
            }
        }

        private void ResetAnimations()
        {
            if (!_animator) return;

            PlayerPrefs.SetString(name, "");
            if (!startActivated)
                _animator.SetBool("Active", false);
        }
    }
}
