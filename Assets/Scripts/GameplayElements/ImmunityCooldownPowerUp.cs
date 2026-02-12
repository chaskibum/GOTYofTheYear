using Managers;
using PlayerScripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utils;

namespace GameplayElements
{
    public class ImmunityCooldownPowerUp : MonoBehaviour, IInteractable
    {
        private SpriteRenderer _sprite;
        private BoxCollider2D _collider;
        
        [SerializeField] private GameObject panel;
        [SerializeField] private GameObject interactPrompt;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject objectLight;

        private bool _inRange;
        private bool _justClosed;
        private UIManager _uiManager;
        private PlayerController _player;
    
        [SerializeField] private bool PowerUp1 = false;
        [SerializeField] private bool PowerUp2 = false;
        [SerializeField] private bool PowerUp3 = false;
    
        private void Start()
        {
            _sprite = GetComponentInChildren<SpriteRenderer>();
            _collider = GetComponent<BoxCollider2D>();
            _uiManager = GameManager.Instance.GetUIManager;
            _player = GameManager.Instance.GetPlayer;
            
            if (PlayerPrefs.GetString("PowerUp1") == name)
                HideObject();
            else if (PlayerPrefs.GetString("PowerUp2") == name)
                HideObject();
            else if (PlayerPrefs.GetString("PowerUp3") == name)
                HideObject();
        
            GameManager.Instance.GetGameRestarted?.AddListener(ResetCollectable);
        }
    
        private void HideObject()
        {
            _sprite.enabled = false;
            _collider.enabled = false;
            objectLight.SetActive(false);
        }
    
        private void OnDestroy()
        {
            GameManager.Instance.GetGameRestarted?.RemoveListener(ResetCollectable);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            interactPrompt.SetActive(true);
            _inRange = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            interactPrompt.SetActive(false);
            _inRange = false;
        }

        private void Update()
        {
            if (_justClosed)
            {
                _justClosed = false;
                return;
            }
        }

        public void OnInteract(InputAction.CallbackContext ctx)
        {
            if (_inRange)
            {
                if (ctx.performed) Interact();
            }
        }

        public void Interact()
        {
            /*if (!panel.activeInHierarchy)
            {
                _uiManager.SetInMenu();
                panel.SetActive(true);
                Time.timeScale = 0;
                closeButton.Select();
            }
            else
            {
                ClosePanel();
            }*/
            _uiManager.SetInMenu();
            panel.SetActive(true);
            Time.timeScale = 0;
            closeButton.Select();
        }

        public void ClosePanel()
        {
            Time.timeScale = 1;
            panel.SetActive(false);
            _justClosed = true;
            _uiManager.SetInMenu(false);
        
            HideObject();
            
            if (PowerUp1)
                PlayerPrefs.SetString("PowerUp1", name);
            else if (PowerUp2)
                PlayerPrefs.SetString("PowerUp2", name);
            else if (PowerUp3)
                PlayerPrefs.SetString("PowerUp3", name);

            _player.GetPlayerData.shieldCooldown -= 3;
            _uiManager.ChangeCooldownSliderMin();
        }
        
        private void ResetCollectable()
        {
            _sprite.enabled = true;
            _collider.enabled = true;
            objectLight.SetActive(true);
            PlayerPrefs.SetString("PowerUp1", "");
            PlayerPrefs.SetString("PowerUp2", "");
            PlayerPrefs.SetString("PowerUp3", "");
        }
    }
}
