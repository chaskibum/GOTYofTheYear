using Managers;
using PlayerScripts;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

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
        private bool _isSpanish = true;
        [SerializeField] private AudioSource activeSound;

        private void Start()
        {
            _interactPromptText = interactPrompt.GetComponentInChildren<TextMeshProUGUI>();
            _interactPromptText.text = GetInteractionPrompt();

            LocalizationSettings.SelectedLocaleChanged += LanguageChanged;

            _animator = GetComponent<Animator>();
            _player = GameManager.Instance.GetPlayer;
            AddListeners(true);
            if (startActivated || PlayerPrefs.GetString(name) == name)
                _animator.SetBool("Active", true);
        }

        public string GetInteractionPrompt()
        {
            if (_isSpanish)
            {
                if (GameManager.Instance.GetControllerConnected)
                {
                    return "Presiona <color=yellow>(Y)</color> para usar tótem";
                }
                else
                {
                    return "Presiona [E] para usar tótem";
                }
            }
            else
            {
                if (GameManager.Instance.GetControllerConnected)
                {
                    return "Press <color=yellow>(Y)</color> to use totem";
                }
                else
                {
                    return "Press [E] to use totem";
                }
            }
        }

        private void LanguageChanged(Locale lang)
        {
            if (lang.ToString() != "Spanish (es)")
            {
                _isSpanish = false;
            }
            else
            {
                _isSpanish = true;
            }
        }

        private void OnDestroy()
        {
            LocalizationSettings.SelectedLocaleChanged -= LanguageChanged;
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

        private void OnTriggerEnter2D(Collider2D other)
        {
            _isInRange = true;
            _interactPromptText.text = GetInteractionPrompt();
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
