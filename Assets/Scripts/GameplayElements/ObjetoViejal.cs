using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace GameplayElements
{
    public class ObjetoViejal : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private GameObject interactPrompt;
        [SerializeField] private Button closeButton;

        private bool _inRange;
        private bool _justClosed;
        private UIManager _uiManager;

        private void Start()
        {
            _uiManager = GameManager.Instance.GetUIManager;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            interactPrompt.SetActive(true);
            _inRange = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            interactPrompt.SetActive(false);
            panel.SetActive(false);
            _inRange = false;
        }

        private void Update()
        {
            if (_justClosed)
            {
                _justClosed = false;
                return;
            }
        
            if (_inRange) CheckInteractInput();
            // if (_inRange && panel.activeInHierarchy) CheckCloseInput();
        }

        private void CheckInteractInput()
        {
            if (Input.GetButtonDown("Interact"))
            {
                Interact();
            }
        }

        private void CheckCloseInput()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                ClosePanel();
            }
        }
    
        public void Interact()
        {
            if (!panel.activeInHierarchy)
            {
                _uiManager.inMenu = true;
                panel.SetActive(true);
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
            panel.SetActive(false);
            _justClosed = true;
            _uiManager.inMenu = false;
            gameObject.SetActive(false);
        }
    }
}
