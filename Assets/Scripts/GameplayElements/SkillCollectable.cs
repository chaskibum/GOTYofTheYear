using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace GameplayElements
{
    public class SkillCollectable : MonoBehaviour
    {
        private SpriteRenderer _sprite;
        private BoxCollider2D _collider;
        private UIManager _uiManager;

        [SerializeField] private GameObject panel;

        [SerializeField] private Button closeButton;
        // [SerializeField] private GameObject text;
    
        // PARA BORRAR DESPUÉS
        [SerializeField] private bool dashCollectable = false;
        [SerializeField] private bool doubleJumpCollectable = false;
        [SerializeField] private bool immunityCollectable = false;

        private void Start()
        {
            _sprite = GetComponentInChildren<SpriteRenderer>();
            _collider = GetComponent<BoxCollider2D>();
            _uiManager = GameManager.Instance.GetUIManager;
        
            if (PlayerPrefs.GetString("DashUnlocked") == name)
            {
                _sprite.enabled = false;
                _collider.enabled = false;
            }
            else if (PlayerPrefs.GetString("DoubleJumpUnlocked") == name)
            {
                _sprite.enabled = false;
                _collider.enabled = false;
            }
            else if (PlayerPrefs.GetString("ImmunityUnlocked") == name)
            {
                _sprite.enabled = false;
                _collider.enabled = false;
            }
        
            GameManager.Instance.GetGameRestarted?.AddListener(ResetCollectable);
        }

        private void Update()
        {
            if (panel.activeInHierarchy)
            {
                CheckCloseInput();
            }
        }

        private void OnDestroy()
        {
            GameManager.Instance.GetGameRestarted?.RemoveListener(ResetCollectable);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (dashCollectable)
            {
                // text.GetComponent<TextMeshProUGUI>().text = "Dash desbloqueado!";
                PlayerPrefs.SetString("DashUnlocked", name);
                GameManager.Instance.GetDashUnlocked?.Invoke();
            }
            else if (doubleJumpCollectable)
            {
                // text.GetComponent<TextMeshProUGUI>().text = "Doble salto desbloqueado!";
                PlayerPrefs.SetString("DoubleJumpUnlocked", name);
                GameManager.Instance.GetDoubleJumpUnlocked?.Invoke();
            }
            else if (immunityCollectable)
            {
                // text.GetComponent<TextMeshProUGUI>().text = "Amuleto de inmunidad desbloqueado!";
                PlayerPrefs.SetString("ImmunityUnlocked", name);
                GameManager.Instance.GetImmunityUnlocked?.Invoke();
            }
        
            _sprite.enabled = false;
            _collider.enabled = false;
            panel.SetActive(true);
            Time.timeScale = 0;
            closeButton.Select();
            _uiManager.inMenu = true;
            // Invoke(nameof(HideText), 3f);
        }
        
        private void CheckCloseInput()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                ClosePanel();
            }
        }
        
        public void ClosePanel()
        {
            Time.timeScale = 1;
            panel.SetActive(false);
            _uiManager.inMenu = false;
        }

        /*private void HideText()
        {
            panel.SetActive(false);
        }*/

        private void ResetCollectable()
        {
            _sprite.enabled = true;
            _collider.enabled = true;
            PlayerPrefs.SetString("DashUnlocked", "");
            PlayerPrefs.SetString("DoubleJumpUnlocked", "");
            PlayerPrefs.SetString("ImmunityUnlocked", "");
        }
    }
}
