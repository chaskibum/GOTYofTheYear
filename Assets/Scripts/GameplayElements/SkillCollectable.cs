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
        private Vector3 _startPosition;

        [SerializeField] private GameObject panel;

        [SerializeField] private Button closeButton;
    
        // PARA BORRAR DESPUÉS
        [SerializeField] private bool dashCollectable = false;
        [SerializeField] private bool doubleJumpCollectable = false;
        [SerializeField] private bool immunityCollectable = false;

        private GameManager _gm;

        private void Start()
        {
            _sprite = GetComponentInChildren<SpriteRenderer>();
            _collider = GetComponent<BoxCollider2D>();
            _gm = GameManager.Instance;
            _uiManager = _gm.GetUIManager;
            _startPosition = transform.position;
        
            HideSkill();

            _gm.GetGameRestarted?.AddListener(ResetCollectable);
        }

        private void OnDestroy()
        {
            _gm.GetGameRestarted?.RemoveListener(ResetCollectable);
        }

        public void Activate(bool jump = false)
        {
            gameObject.SetActive(true);
            _sprite.enabled = true;
            if (jump)
            {
                transform.DOJump(_gm.GetPlayer.GetPlayerTarget, 1.5f, 1, 1f)
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
                _gm.GetDashUnlocked?.Invoke();
            }
            else if (doubleJumpCollectable)
            {
                PlayerPrefs.SetString("DoubleJumpUnlocked", name);
                _gm.GetDoubleJumpUnlocked?.Invoke();
            }
            else if (immunityCollectable)
            {
                PlayerPrefs.SetString("ImmunityUnlocked", name);
                _gm.GetImmunityUnlocked?.Invoke();
            }
        
            HideSkill();
            panel.SetActive(true);
            Time.timeScale = 0;
            closeButton.Select();
            _uiManager.inMenu = true;
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
            _uiManager.inMenu = false;
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
