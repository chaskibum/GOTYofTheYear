using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GameplayElements
{
    public class ExtraLifeCollectable : MonoBehaviour
    {
        private SpriteRenderer _sprite;
        private BoxCollider2D _collider;
        private Light2D _light;

        [SerializeField] private GameObject panel;
        [SerializeField] private GameObject text;
    
        // PARA BORRAR DESPUÉS
        [SerializeField] private bool isGameWonCollectable = false;

        private void Start()
        {
            _sprite = GetComponentInChildren<SpriteRenderer>();
            _collider = GetComponent<BoxCollider2D>();
            _light = GetComponentInChildren<Light2D>();
        
            if (PlayerPrefs.GetString("Taken") == name)
            {
                Hide();
            }
        
            GameManager.Instance.GetGameRestarted?.AddListener(ResetCollectable);
        }

        private void OnDestroy()
        {
            GameManager.Instance.GetGameRestarted?.RemoveListener(ResetCollectable);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isGameWonCollectable)
            {
                GameManager.Instance.canWin = true;
                text.GetComponent<TextMeshProUGUI>().text = "Item final conseguido!";
                Hide();
                panel.SetActive(true);
                Invoke(nameof(HideText), 3f);
            }
            else
            {
                Hide();
                panel.SetActive(true);
                AudioManager.Instance.PlayClip(AudioManager.AudioList.ExtraLifeGrabbed, false, 0.9f);
                Invoke(nameof(HideText), 3f);
                GameManager.Instance.GetCollectablePicked?.Invoke();
                PlayerPrefs.SetString("Taken", name);
            }
        }

        private void Hide()
        {
            _sprite.enabled = false;
            _collider.enabled = false;
            _light.enabled = false;
        }

        private void HideText()
        {
            panel.SetActive(false);
        }

        private void ResetCollectable()
        {
            _sprite.enabled = true;
            _collider.enabled = true;
            PlayerPrefs.SetString("Taken", "");
        }
    }
}
