using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utils;
using TMPro;

namespace GameplayElements
{
    public class ShowMap : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameObject map;
        [SerializeField] private GameObject interactPrompt;
        private TextMeshProUGUI _interactPromptText;
        [SerializeField] private Button closeButton;

        private bool _inRange;
        private bool _justClosed;
        private UIManager _uiManager;

        private void Start()
        {
            _interactPromptText = interactPrompt.GetComponentInChildren<TextMeshProUGUI>();
            _interactPromptText.text = GetInteractionPrompt();

            _uiManager = GameManager.Instance.GetUIManager;
        }

        public string GetInteractionPrompt()
        {
            return "Presiona [E] para ver mapa";
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            interactPrompt.SetActive(true);
            _inRange = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            interactPrompt.SetActive(false);
            map.SetActive(false);
            _inRange = false;
        }

        private void Update()
        {
            if (_justClosed)
            {
                _justClosed = false;
                return;
            }

            // if (_inRange) CheckInteractInput();
            // if (_inRange && map.activeInHierarchy) CheckCloseInput();
        }

        public void OnInteract(InputAction.CallbackContext ctx)
        {
            if (_inRange)
            {
                if (ctx.performed) Interact();
            }
        }

        /*private void CheckCloseInput()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                ClosePanel();
            }
        }*/

        public void Interact()
        {
            if (!map.activeInHierarchy)
            {
                _uiManager.SetInMenu();
                map.SetActive(true);
                Time.timeScale = 0;
                closeButton.Select();
            }
            else
            {
                ClosePanel();
            }
        }

        public void ClosePanel()
        {
            Time.timeScale = 1;
            map.SetActive(false);
            _justClosed = true;
            _uiManager.SetInMenu(false);
        }
    }
}
