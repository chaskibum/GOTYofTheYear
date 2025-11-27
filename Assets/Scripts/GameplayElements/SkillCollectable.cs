using System.Collections;
using DG.Tweening;
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
    
        // PARA BORRAR DESPUÉS
        [SerializeField] private bool dashCollectable = false;
        [SerializeField] private bool doubleJumpCollectable = false;
        [SerializeField] private bool immunityCollectable = false;

        private IEnumerator Start()
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

            yield return new WaitForSeconds(0.5f);
            transform.DOJump(GameManager.Instance.GetPlayer.GetPlayerTarget, 2, 1, 2);
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
                PlayerPrefs.SetString("DashUnlocked", name);
                GameManager.Instance.GetDashUnlocked?.Invoke();
            }
            else if (doubleJumpCollectable)
            {
                PlayerPrefs.SetString("DoubleJumpUnlocked", name);
                GameManager.Instance.GetDoubleJumpUnlocked?.Invoke();
            }
            else if (immunityCollectable)
            {
                PlayerPrefs.SetString("ImmunityUnlocked", name);
                GameManager.Instance.GetImmunityUnlocked?.Invoke();
            }
        
            _sprite.enabled = false;
            _collider.enabled = false;
            panel.SetActive(true);
            Time.timeScale = 0;
            closeButton.Select();
            _uiManager.inMenu = true;
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
