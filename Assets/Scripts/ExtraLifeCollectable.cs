using TMPro;
using UnityEngine;

public class ExtraLifeCollectable : MonoBehaviour
{
    private SpriteRenderer _sprite;
    private BoxCollider2D _collider;

    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject text;
    
    // PARA BORRAR DESPUÉS
    [SerializeField] private bool isGameWonCollectable = false;

    private void Start()
    {
        _sprite = GetComponentInChildren<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();
        
        if (PlayerPrefs.GetString("Taken") == name)
        {
            _sprite.enabled = false;
            _collider.enabled = false;
        }
        
        GameManager.Instance.GetGameRestarted?.AddListener(ResetCollectable);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isGameWonCollectable)
        {
            GameManager.Instance.canWin = true;
            text.GetComponent<TextMeshProUGUI>().text = "Item final conseguido!";
            _sprite.enabled = false;
            _collider.enabled = false;
            panel.SetActive(true);
            Invoke(nameof(HideText), 1f);
        }
        else
        {
            _sprite.enabled = false;
            _collider.enabled = false;
            panel.SetActive(true);
            AudioManager.Instance.PlayClip(AudioManager.AudioList.ExtraLifeGrabbed, false, 0.9f);
            Invoke(nameof(HideText), 1f);
            GameManager.Instance.GetCollectablePicked?.Invoke();
            PlayerPrefs.SetString("Taken", name);
        }
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
