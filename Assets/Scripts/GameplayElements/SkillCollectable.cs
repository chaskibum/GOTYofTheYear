using DG.Tweening;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace GameplayElements
{
    public class SkillCollectable : MonoBehaviour
    {
        private SpriteRenderer _sprite;
        private BoxCollider2D _collider;
        private UIManager _uiManager;
        private Vector3 _startPosition;

        [SerializeField] private GameObject panel;

        [SerializeField] private Button closeButton;
    
        // PARA BORRAR DESPUÉS
        [SerializeField] private bool dashCollectable = false;
        [SerializeField] private bool doubleJumpCollectable = false;
        [SerializeField] private bool immunityCollectable = false;

        private void Start()
        {
            _sprite = GetComponentInChildren<SpriteRenderer>();
            _collider = GetComponent<BoxCollider2D>();
            _uiManager = GameManager.Instance.GetUIManager;
            _startPosition = transform.position;
        
            HideSkill();

            GameManager.Instance.GetGameRestarted?.AddListener(ResetCollectable);
        }

        private void OnDestroy()
        {
            GameManager.Instance.GetGameRestarted?.RemoveListener(ResetCollectable);
        }

        public void Activate(bool jump = false)
        {
            gameObject.SetActive(true);
            _sprite.enabled = true;
            if (jump)
            {
                transform.DOJump(GameManager.Instance.GetPlayer.GetPlayerTarget, 1.5f, 1, 1f)
                    .OnComplete(() => _collider.enabled = true);
            }
            else
            {
                _sprite.DOFade(0f, 0f);
                _sprite.DOFade(1f, 2f);
                Invoke(nameof(ActivateCollider), 2f);
            }
        }

        private void ActivateCollider()
        {
            _collider.enabled = true;
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
        
            HideSkill();
            panel.SetActive(true);
            Time.timeScale = 0;
            closeButton.Select();
            _uiManager.SetInMenu();
        }

        public void HideSkill()
        {
            _sprite.enabled = false;
            _collider.enabled = false;
        }
        
        public void ClosePanel()
        {
            Time.timeScale = 1;
            panel.SetActive(false);
            _uiManager.SetInMenu(false);
            GamepadVibration.Instance.Rumble(0.4f, 0.5f, 2f);
        }

        private void ResetCollectable()
        {
            HideSkill();
            transform.position = _startPosition;
            PlayerPrefs.SetString("DashUnlocked", "");
            PlayerPrefs.SetString("DoubleJumpUnlocked", "");
            PlayerPrefs.SetString("ImmunityUnlocked", "");
        }
    }
}
