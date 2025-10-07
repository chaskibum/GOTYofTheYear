using TMPro;
using UnityEngine;

public class Collectable : MonoBehaviour
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
        
        GameManager.Instance.GetGameRestarted?.AddListener(ResetCollectable);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isGameWonCollectable)
        {
            GameManager.Instance.canWin = true;
            text.GetComponent<TextMeshProUGUI>().text = "Item final conseguido!";
            panel.SetActive(true);
        }
            
        _sprite.enabled = false;
        _collider.enabled = false;
        panel.SetActive(true);
        AudioManager.Instance.PlayClip(AudioManager.AudioList.ExtraLifeGrabbed);
        Invoke(nameof(HideText), 1f);
        GameManager.Instance.GetCollectablePicked?.Invoke();
    }

    private void HideText()
    {
        panel.SetActive(false);
    }

    private void ResetCollectable()
    {
        _sprite.enabled = true;
        _collider.enabled = true;
    }
}
